
namespace LogReaper.Net.Configuration
{
    public class LogReaperConfig
    {
        public IReadOnlyCollection<string> Bases { get; set; }

        public FilesConfig Files { get; set; }

        public ElasticConfig Elastic { get; set; }

        public string RootDirectory { get; set; }
    }
}
