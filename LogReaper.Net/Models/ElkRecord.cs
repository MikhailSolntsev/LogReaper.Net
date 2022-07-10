using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net.Models;

public class ElkRecord
{
    public string Datetime { get; set; } = "";
    public string User { get; set; } = "";
    public string Computer { get; set; } = "";
    public string Server { get; set; } = "";
    public string Application { get; set; } = "";
    public string Event { get; set; } = "";
    public string Importance { get; set; } = "";
    public string Comment { get; set; } = "";
    public string Metadata { get; set; } = "";
    public string Representation { get; set; } = "";
    public string Data { get; set; } = "";
    public uint Connection { get; set; }
}
