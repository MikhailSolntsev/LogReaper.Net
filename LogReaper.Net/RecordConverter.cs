using System.Text;
using LogReaper.Net.Service;
using System.Transactions;
using LogReaper.Net.Dto;

namespace LogReaper.Net;

public class ConvertRecordService
{
    private RepresentationData representations = null!;
    
    private FilterData filters = null!;

    private readonly ILocalLogger logger;

    public ConvertRecordService(ILocalLogger logger)
    {
        this.logger = logger;
    }

    private bool IsFiltered(LogRecord logRecord, LogDictionary dictionary)
    {
        var element = dictionary.Events[logRecord.EventId];
        if (filters.Events.Contains(element)) return true;

        element = representations.Levels[logRecord.Importance];
        if (filters.Levels.Contains(element)) return true;

        element = representations.TransactionStatuses[logRecord.TransactionStatus];
        if (filters.TransactionStatuses.Contains(element)) return true;
        
        return false;
    }

    public ElkRecord? LogRecordToElkRecord(LogRecord logRecord, LogDictionary dictionary)
    {
        if (IsFiltered(logRecord, dictionary))
        {
            return null;
        }

        ElkRecord elkRecord = new()
        {
            Datetime = logRecord.Datetime.ToDateTimeString()
        };

        RepresentTransaction(logRecord, elkRecord, dictionary);

        RepresentUser(logRecord, elkRecord, dictionary);

        RepresentComputer(logRecord, elkRecord, dictionary);

        RepresentApplication(logRecord, elkRecord, dictionary);

        RepresentServer(logRecord, elkRecord, dictionary);

        RepresentEvent(logRecord, elkRecord, dictionary);

        RepresentLevel(logRecord, elkRecord);

        RepresentMetadata(logRecord, elkRecord, dictionary);
        
        elkRecord.Comment = logRecord.Comment;
        elkRecord.Representation = logRecord.Representation;
        elkRecord.Data = logRecord.Data;
        elkRecord.Session = logRecord.Session;

        return elkRecord;
    }

    private void RepresentMetadata(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        if (logRecord.Metadata == 0)
        {
            elkRecord.Metadata = "";
        }
        else
        {
            dictionary.Metadata.TryGetValue(logRecord.Metadata, out string? metadata);
            if (metadata is null)
            {
                logger.LogDebug($"В словаре метадаты не найдено соответствие для {logRecord.Metadata}");
                elkRecord.Metadata = logRecord.Metadata.ToString();
            }
            else
            {
                elkRecord.Metadata = metadata;
            }
        }
    }

    private void RepresentLevel(LogRecord logRecord, ElkRecord elkRecord)
    {
        representations.Levels.TryGetValue(logRecord.Importance, out string? importance);
        if (importance is null)
        {
            logger.LogDebug($"В соответствии представлений не найдено соответствие для уровня {logRecord.Importance}");
            elkRecord.Importance = logRecord.Importance.ToString();
        }
        else
        {
            elkRecord.Importance = importance;
        }
    }

    private void RepresentEvent(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        dictionary.Events.TryGetValue(logRecord.EventId, out string? eventId);
        if (eventId is null)
        {
            logger.LogDebug($"В словаре событий не найдено соответствие для {logRecord.EventId}");
            elkRecord.Event = logRecord.EventId.ToString();
        }
        else
        {
            representations.Events.TryGetValue(eventId, out string? representation);
            if (representation is null)
            {
                //logger.LogDebug($"В соответствии представлений не найдено соответствие для представления {eventId}");
                elkRecord.Event = eventId;
            }
            else
            {
                elkRecord.Event = representation;
            }
        }
    }

    private void RepresentServer(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        dictionary.Servers.TryGetValue(logRecord.Server, out string? server);
        if (server is null)
        {
            logger.LogDebug($"В словаре серверов не найдено соответствие для {logRecord.Server}");
            elkRecord.Server = logRecord.Server.ToString();
        }
        else
        {
            elkRecord.Server = server;
        }
    }

    private void RepresentApplication(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        dictionary.Applications.TryGetValue(logRecord.Application, out string? application);
        if (application is null)
        {
            logger.LogDebug($"В словаре приложений не найдено соответствие для {logRecord.Application}");
            elkRecord.Application = logRecord.Application.ToString();
        }
        else
        {
            representations.Applications.TryGetValue(application, out string? representation);
            if (representation is null)
            {
                logger.LogDebug($"В соответствии представлений не найдено соответствие для приложения {application}");
                elkRecord.Application = application;
            }
            else
            {
                elkRecord.Application = representation;
            }
        }
    }

    private void RepresentComputer(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        dictionary.Computers.TryGetValue(logRecord.Computer, out string? computer);
        if (computer is null)
        {
            logger.LogDebug($"В словаре компьютеров не найдено соответствие для {logRecord.Computer}");
            elkRecord.Computer = logRecord.Computer.ToString();
        }
        else
        {
            elkRecord.Computer = computer;
        }
    }

    private void RepresentUser(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        dictionary.Users.TryGetValue(logRecord.User, out string? user);
        if (user is null)
        {
            logger.LogDebug($"В словаре пользоватлей не найдено соответствие для {logRecord.User}");
            elkRecord.User = logRecord.User.ToString();
        }
        else
        {
            elkRecord.User = user;
        }
    }

    private void RepresentTransaction(LogRecord logRecord, ElkRecord elkRecord, LogDictionary dictionary)
    {
        representations.TransactionStatuses.TryGetValue(logRecord.TransactionStatus, out string? transactionStatus);
        if (transactionStatus is null)
        {
            logger.LogDebug($"В соответствии представлений не найдено соответствие для статуса транзакций {logRecord.TransactionStatus}");
            elkRecord.TransactionStatus = logRecord.TransactionStatus;
        }
        else
        {
            elkRecord.TransactionStatus = transactionStatus;
        }

        elkRecord.TransactionNumber = logRecord.TransactionNumber.ToTransactionNumber();
    }

    public string ElkRecordListToElkMessage(List<ElkRecord> messages, string index)
    {
        StringBuilder builder = new();

        foreach (var message in messages)
        {
            builder.AppendLine($"{{ \"index\" : {{ \"_index\" : \"{index}\"}} }}");
            builder.AppendLine(ElkRecordToJsonString(message));
        }
        
        return builder.ToString();
    }

    public string ElkRecordToJsonString(ElkRecord record)
    {
        var jsonRecord = new Dictionary<string, string>
        {
            ["@timestamp"] = record.Datetime,
            ["transactionStatus"] = record.TransactionStatus.CutQuotes(),
            ["transactionNumber"] = record.TransactionNumber.CutQuotes(),
            ["username"] = record.User.CutQuotes(),
            ["instance"] = record.Computer.CutQuotes(),
            ["useragent"] = record.Application,
            ["servername"] = record.Server.CutQuotes(),
            ["event"] = record.Event,
            ["level"] = record.Importance,
            ["message"] = record.Comment.CutQuotes(),
            ["metadata"] = record.Metadata.CutQuotes(),
            ["representation"] = record.Representation.CutQuotes(),
            ["data"] = record.Data,
            ["timestamp"] = DateTime.Now.ToString("YYYY-MM-dd'T'HH:mm:ss.SSS"),
            ["session"] = record.Session.ToString()
        };
        
        string result = JsonWriter<Dictionary<string, string>>.Serialize(jsonRecord);

        return result;
    }
    
    public void ReadFilter(string directory)
    {
        string fullFileName = Path.Combine(directory, "filter.json");

        logger.LogInfo($"Чтение фильтров из файла {fullFileName}");

        var stream = File.OpenRead(fullFileName);

        FilterData? filterData = ReadJsonService<FilterData>.Deserialize(stream);

        if (filterData is null)
        {
            throw new Exception($"Can't read config file {fullFileName}");
        }

        filters = filterData;

        logger.LogInfo("Чтение фильтров завершено");
    }

    public void ReadRepresentations(string directory)
    {
        string fullFileName = Path.Combine(directory, "representation.json");

        logger.LogInfo($"Чтение представлений из файла {fullFileName}");

        var stream = File.OpenRead(fullFileName);
        RepresentationData? representationData = ReadJsonService<RepresentationData>.Deserialize(stream);

        if (representationData is null)
        {
            throw new Exception($"Can't read represenations from file {fullFileName}");
        }

        representations = representationData;
        
        logger.LogInfo("Чтение представлений завершено");
    }
    
    private class RepresentationData
    {
        public Dictionary<string, string> Events { get; set; } = null!;
        public Dictionary<string, string> Levels { get; set; } = null!;
        public Dictionary<string, string> Applications { get; set; } = null!;
        public Dictionary<string, string> TransactionStatuses { get; set; } = null!;
    }

    private class FilterData
    {
        public List<string> Events { get; set; } = null!;
        public List<string> Levels { get; set; } = null!;
        public List<string> TransactionStatuses { get; set; } = null!;
    }

}
