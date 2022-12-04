using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
