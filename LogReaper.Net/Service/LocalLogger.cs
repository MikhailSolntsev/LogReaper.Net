
namespace LogReaper.Net.Service;

public class LocalLogger : ILocalLogger
{
    public void LogInfo(string? message)
    {
        Console.WriteLine($"Info {message}");
    }
    public void LogInfo(string message, string database)
    {
        Console.WriteLine($"Info [{database}] {message}");
    }
    public void LogError(string? message)
    {
        Console.WriteLine($"ERROR! {message}");
    }
    public void LogError(string message, string database)
    {
        Console.WriteLine($"ERROR! [{database}] {message}");
    }

    public void LogDebug(string? message)
    {
        Console.WriteLine($"debug {message}");
    }

    public void LogDebug(string message, string database)
    {
        Console.WriteLine($"debug [{database}] {message}");
    }
}
