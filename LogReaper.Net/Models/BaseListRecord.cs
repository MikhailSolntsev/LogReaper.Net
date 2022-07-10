using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net.Models;

internal class BaseListRecord
{
    public string Name { get; set; } = "";
    public string Uid { get; set; } = "";
    public string SqlServer { get; set; } = "";
    public string SqlBase { get; set; } = "";
}
