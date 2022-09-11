
namespace LogReaper.Net.Service;

internal class TimeTracker
{
    private DateTime startTime;

    internal void StartTracking()
    {
        startTime = DateTime.UtcNow;
    }

    internal void StopTracking()
    {
        DateTime stopTime = DateTime.UtcNow;
        Console.WriteLine($"Loggins last for {(stopTime - startTime).TotalSeconds} seconds");
    }
}
