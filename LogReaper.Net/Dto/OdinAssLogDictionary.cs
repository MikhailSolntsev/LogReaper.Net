namespace LogReaper.Net.Dto;

public class OdinAssLogDictionary
{
    public Dictionary<uint, string> Users { get; set; } = new();
    public Dictionary<uint, string> Computers { get; set; } = new();
    public Dictionary<uint, string> Applications { get; set; } = new();
    public Dictionary<uint, string> Events { get; set; } = new();
    public Dictionary<uint, string> Metadata { get; set; } = new();
    public Dictionary<uint, string> Servers { get; set; } = new();

}
