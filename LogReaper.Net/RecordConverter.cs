
using LogReaper.Net.Models;
using System.Text;
using LogReaper.Net.Service;

namespace LogReaper.Net;

public class RecordConverter
{
    private RepresentationData representations = null!;
    
    private FilterData filters = null!;

    private readonly ILocalLogger logger;

    public RecordConverter(ILocalLogger logger)
    {
        this.logger = logger;
    }

    private bool IsFiltered(LogRecord logRecord, LogDictionary dictionary)
    {
        var element = dictionary.Events[logRecord.EventId];
        if (filters.Events.Contains(element)) return true;

        element = representations.Levels[logRecord.Importance];
        if (filters.Levels.Contains(element)) return true;

        element = representations.Levels[logRecord.TransactionStatus];
        if (filters.TransactionStatuses.Contains(element)) return true;
        
        return false;
    }

    public ElkRecord? LogRecordToElkRecord(LogRecord logRecord, LogDictionary dictionary)
    {
        if (IsFiltered(logRecord, dictionary))
        {
            return null;
        }

        ElkRecord elkRecord = new ()
        {
            Datetime = logRecord.Datetime.ToDateTimeString(),
            TransactionStatus = representations.TransactionStatuses[logRecord.TransactionStatus],
            TransactionNumber = logRecord.TransactionNumber.ToTransactionNumber(),
            User = dictionary.Users[logRecord.User],
            Computer = dictionary.Computers[logRecord.Computer],
            Application = representations.Applications[dictionary.Applications[logRecord.Application]],
            Server = dictionary.Servers[logRecord.Server],
            Event = representations.Events[dictionary.Events[logRecord.EventId]],
            Importance = representations.Levels[logRecord.Importance],
            Comment = logRecord.Comment,
            Metadata = dictionary.Metadata[logRecord.Metadata],
            Representation = logRecord.Representation,
            Data = logRecord.Data,
            Session = logRecord.Session
        };

        return elkRecord;
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

        FilterData? filterData = JsonReader<FilterData>.Deserialize(stream);

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
        RepresentationData? representationData = JsonReader<RepresentationData>.Deserialize(stream);

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
