
namespace LogReaper.Net.Dto;

public class Configuration
{
    public IList<string> Bases { get; set; }
    public string LogDirectory { get; set; }
    public string BackupDirectory { get; set; }
    public string ElasticUrl { get; set; }
    public int ElasticBulkSize { get; set; }

}
