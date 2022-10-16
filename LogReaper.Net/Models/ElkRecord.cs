﻿
namespace LogReaper.Net.Models;

public class ElkRecord
{
    public string Datetime { get; set; } = String.Empty;
    public string TransactionStatus = String.Empty;
    public string TransactionNumber { get; set; } = String.Empty;
    public string User { get; set; } = String.Empty;
    public string Computer { get; set; } = String.Empty;
    public string Server { get; set; } = String.Empty;
    public string Application { get; set; } = String.Empty;
    public string Event { get; set; } = String.Empty;
    public string Importance { get; set; } = String.Empty;
    public string Comment { get; set; } = String.Empty;
    public string Metadata { get; set; } = String.Empty;
    public string Representation { get; set; } = String.Empty;
    public string Data { get; set; } = String.Empty;
    public uint Session { get; set; }
}
