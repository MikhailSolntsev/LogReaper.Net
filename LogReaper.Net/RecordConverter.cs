
using LogReaper.Net.Models;
using System;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;
using System.Text;
using System.IO;

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

    public ElkRecord? Convert(LogRecord logRecord, LogDictionary dictionary)
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

    public string ConvertListToMessage(List<ElkRecord> messages, string index)
    {
        StringBuilder builder = new();

        foreach (var message in messages)
        {
            builder.AppendLine($"{{ \"index\" : {{ \"_index\" : \"{index}\"}} }}");
            builder.AppendLine(ElkToJson(message));
        }
        
        return builder.ToString();
    }

    public string ElkToJson(ElkRecord record)
    {
        var jsonRecord = new Dictionary<String, String>();

        jsonRecord["@timestamp"] = record.Datetime;
        jsonRecord["transactionStatus"] = record.TransactionStatus.CutQuotes();
        jsonRecord["transactionNumber"] = record.TransactionNumber.CutQuotes();
        jsonRecord["username"] = record.User.CutQuotes();
        jsonRecord["instance"] = record.Computer.CutQuotes();
        jsonRecord["useragent"] = record.Application;
        jsonRecord["servername"] = record.Server.CutQuotes();
        jsonRecord["event"] = record.Event;
        jsonRecord["level"] = record.Importance;
        jsonRecord["message"] = record.Comment.CutQuotes();
        jsonRecord["metadata"] = record.Metadata.CutQuotes();
        jsonRecord["representation"] = record.Representation.CutQuotes();
        jsonRecord["data"] = record.Data;
        jsonRecord["timestamp"] = Timestamp();
        jsonRecord["session"] = record.Session.ToString();

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.IncludeFields = true;
        options.PropertyNameCaseInsensitive = false;
        options.WriteIndented = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        string result = JsonSerializer.Serialize<Dictionary<String, String>>(jsonRecord, options);

        return result;
    }

    private string Timestamp()
    {
        return DateTime.Now.ToString("YYYY-MM-dd'T'HH:mm:ss.SSS");
    }

    public void ReadConfig(string directory)
    {
        ReadRepresentations(directory);
        ReadFilter(directory);
    }

    private void ReadFilter(string directory)
    {
        string fullFileName = Path.Combine(directory, "filter.json");

        var stream = File.OpenRead(fullFileName);

        JsonSerializerOptions options = SerializerOptions();
        FilterData filterData = JsonSerializer.Deserialize<FilterData>(stream, options);

        eventsFilter = filterData.Events;
        levelsFilter = filterData.Levels;
        transactionFilter = filterData.TransactionStatuses;
    }

    private void ReadRepresentations(string directory)
    {
        string fullFileName = Path.Combine(directory, "representation.json");

        var stream = File.OpenRead(fullFileName);

        JsonSerializerOptions options = SerializerOptions();
        RepresentationData representationData = JsonSerializer.Deserialize<RepresentationData>(stream, options);

        events = representationData.Events;
        levels = representationData.Levels;
        useragents = representationData.Applications;
        transactions = representationData.TransactionStatuses;
    }

    private static JsonSerializerOptions SerializerOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return options;
    }

    private class RepresentationData
    {
        public Dictionary<string, string> Events { get; set; }
        public Dictionary<string, string> Levels { get; set; }
        public Dictionary<string, string> Applications { get; set; }
        public Dictionary<string, string> TransactionStatuses { get; set; }
    }

    private class FilterData
    {
        public List<string> Events { get; set; }
        public List<string> Levels { get; set; }
        public List<string> TransactionStatuses { get; set; }
    }

}
