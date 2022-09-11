
using LogReaper.Net.Models;
using System.Text;
using LogReaper.Net.Service;

namespace LogReaper.Net;

public class RecordConverter
{
    private Dictionary<string, string> events = new();
    private Dictionary<string, string> levels = new ();
    private Dictionary<string, string> useragents = new();
    private Dictionary<string, string> transactions = new();
    
    private List<string> levelsFilter = new();
    private List<string> eventsFilter = new();
    private List<string> transactionFilter = new();

    private bool IsFiltered(LogRecord logRecord, LogDictionary dictionary)
    {
        var element = dictionary.Events[logRecord.EventId];
        if (eventsFilter.Contains(element)) return true;

        element = levels[logRecord.Importance];
        if (levelsFilter.Contains(element)) return true;

        element = levels[logRecord.TransactionStatus];
        if (transactionFilter.Contains(element)) return true;
        
        return false;
    }

    public ElkRecord? LogRecordToElkRecord(LogRecord logRecord, LogDictionary dictionary)
    {
        if (IsFiltered(logRecord, dictionary))
        {
            return null;
        }

        ElkRecord elkRecord = new ElkRecord()
        {
            Datetime = logRecord.Datetime.ToDateTimeString(),
            TransactionStatus = transactions[logRecord.TransactionStatus],
            TransactionNumber = logRecord.TransactionNumber.ToTransactionNumber(),
            User = dictionary.Users[logRecord.User],
            Computer = dictionary.Computers[logRecord.Computer],
            Application = useragents[dictionary.Applications[logRecord.Application]],
            Server = dictionary.Servers[logRecord.Server],
            Event = events[dictionary.Events[logRecord.EventId]],
            Importance = levels[logRecord.Importance],
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

        var stream = File.OpenRead(fullFileName);

        FilterConfig? filterConfig = JsonReader<FilterConfig>.Deserialize(stream);

        if (filterConfig is null)
        {
            throw new Exception($"Can't read config file {fullFileName}");
        }
        eventsFilter = filterConfig.Events;
        levelsFilter = filterConfig.Levels;
        transactionFilter = filterConfig.TransactionStatuses;
    }

    public void ReadRepresentations(string directory)
    {
        string fullFileName = Path.Combine(directory, "representation.json");

        var stream = File.OpenRead(fullFileName);
        RepresentationData? representationData = JsonReader<RepresentationData>.Deserialize(stream);

        if (representationData is null)
        {
            throw new Exception($"Can't read represenations from file {fullFileName}");
        }

        events = representationData.Events;
        levels = representationData.Levels;
        useragents = representationData.Applications;
        transactions = representationData.TransactionStatuses;
    }
    
    private class RepresentationData
    {
        public Dictionary<string, string> Events { get; set; } = null!;
        public Dictionary<string, string> Levels { get; set; } = null!;
        public Dictionary<string, string> Applications { get; set; } = null!;
        public Dictionary<string, string> TransactionStatuses { get; set; } = null!;
    }

    private class FilterConfig
    {
        public List<string> Events { get; set; } = null!;
        public List<string> Levels { get; set; } = null!;
        public List<string> TransactionStatuses { get; set; } = null!;
    }

}
