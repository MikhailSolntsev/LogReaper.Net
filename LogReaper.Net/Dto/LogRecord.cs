
namespace LogReaper.Net.Dto;

public class LogRecord
{
    public long Datetime { get; set; }                   // 1
    public string TransactionStatus { get; set; }        // 2
    public string TransactionNumber { get; set; }        // 3
    public uint User { get; set; }                       // 4
    public uint Computer { get; set; }                   // 5
    public uint Application { get; set; }                // 6
    public uint Connection { get; set; }                 // 7
    public uint EventId { get; set; }                    // 8
    // TODO: change importance to enum
    public string Importance { get; set; }               // 9
    public string Comment { get; set; }                  // 10
    public uint Metadata { get; set; }                   // 11
    public string Data { get; set; }                     // 12
    public string Representation { get; set; }           // 13
    public uint Server { get; set; }                     // 14
    public uint Session { get; set; }                    // 17
}


