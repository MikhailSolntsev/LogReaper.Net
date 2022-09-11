
namespace LogReaper.Net.Models;

public class Configuration
{
    public IList<string> Bases { get; set; } = null!;
    public string LogDirectory { get; set; } = null!;
    public string BackupDirectory { get; set; } = null!;
    public string ElkUrl { get; set; } = null!;
    public int BulkSize { get; set; }

}
