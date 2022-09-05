using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net.Models;

internal class DictionaryRecord
{
    public uint Dictionary { get; set; }
    public string Uid { get; set; } = "";
    public string Representation { get; set; } = "";
    public uint Id { get; set; }
}
