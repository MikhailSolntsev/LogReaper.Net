using System.Text;
using LogReaper.Net.Service;
using LogReaper.Net.Dto;
using LogReaper.Net.Contracts;

namespace LogReaper.Net;

public class ConvertRecordService
{
    private readonly IFilterRecordsService filterRecordsService;
    private readonly IRepresentFieldsService representFieldsService;

    public ConvertRecordService(IFilterRecordsService filterRecordsService, IRepresentFieldsService representFieldsService)
    {
        this.filterRecordsService = filterRecordsService;
        this.representFieldsService = representFieldsService;
    }

    public ElasticRecord? LogRecordToElasticRecord(LogRecord logRecord, OdinAssLogDictionary dictionary)
    {
        representFieldsService.UseDictionary(dictionary);

        if (filterRecordsService.IsFiltered(logRecord))
        {
            return null;
        }

        ElasticRecord elkRecord = new()
        {
            Datetime = logRecord.Datetime.ToDateTimeString()
        };

        elkRecord.TransactionNumber = logRecord.TransactionNumber.ToTransactionNumber();

        elkRecord.TransactionStatus = representFieldsService.RepresentTransaction(logRecord);

        elkRecord.User = representFieldsService.RepresentUser(logRecord);

        elkRecord.Computer = representFieldsService.RepresentComputer(logRecord);

        elkRecord.Application = representFieldsService.RepresentApplication(logRecord);

        elkRecord.Server = representFieldsService.RepresentServer(logRecord);

        elkRecord.Event = representFieldsService.RepresentEvent(logRecord);

        elkRecord.Importance = representFieldsService.RepresentLevel(logRecord);

        elkRecord.Metadata = representFieldsService.RepresentMetadata(logRecord);
        
        elkRecord.Comment = logRecord.Comment;
        elkRecord.Representation = logRecord.Representation;
        elkRecord.Data = logRecord.Data;
        elkRecord.Session = logRecord.Session;

        return elkRecord;
    }

    public string ElasticRecordsToElasticMessage(List<ElasticRecord> messages, string index)
    {
        StringBuilder builder = new();

        foreach (var message in messages)
        {
            builder.AppendLine($"{{ \"index\" : {{ \"_index\" : \"{index}\"}} }}");
            builder.AppendLine(ElasticRecordToJsonString(message));
        }
        
        return builder.ToString();
    }

    public string ElasticRecordToJsonString(ElasticRecord record)
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
        
        string result = JsonSerializer<Dictionary<string, string>>.Serialize(jsonRecord);

        return result;
    }

}
