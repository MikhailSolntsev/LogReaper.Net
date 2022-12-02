using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net.Configuration
{
    public class ElasticConfig
    {
        public string Url { get; set; }
        public int BulkSize { get; set; }
    }
}
