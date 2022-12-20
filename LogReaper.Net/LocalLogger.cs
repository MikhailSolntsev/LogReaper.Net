using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;

namespace LogReaper.Net;

public class LocalLogger : ILocalLogger
{
    private readonly LogReaperConfig config;

    public LocalLogger(LogReaperConfig config)
    {
        this.config = config;
    }

    private BaseListRecord baseRecord;

    public void UseCurrentBase(BaseListRecord baseRecord)
    {
        this.baseRecord = baseRecord;
    }
    public void LogInfo(string? message)
    {
        Console.WriteLine($"Info {message}");
    }
    public void LogError(string? message)
    {
        Console.WriteLine($"ERROR! {message}");
    }
    public void LogDebug(string? message)
    {
        //Console.WriteLine($"debug {message}");
    }
    public void LogBaseInfo(string? message)
    {
        Console.WriteLine($"Info [{baseRecord?.Name}] {message}");
    }
    public void LogBaseError(string? message)
    {
        Console.WriteLine($"Info [{baseRecord?.Name}] {message}");
    }
    public void LogBaseDebug(string? message)
    {
        Console.WriteLine($"Info [{baseRecord?.Name}] {message}");
    }

    public void FileLog(string data)
    {
        var fileName = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH-mm-ss-fff.log");
        var fullFileName = Path.Combine(config.RootDirectory, fileName);
        File.WriteAllText(fullFileName, data);
    }
}
