
namespace LogReaper.Net.Service;

public class TimeTracker
{
    private DateTime startTime;

    public void StartTracking()
    {
        startTime = DateTime.UtcNow;
    }

    public double StopTracking()
    {
        DateTime stopTime = DateTime.UtcNow;
        double mills = (stopTime - startTime).TotalMilliseconds;
        Console.WriteLine($"Loggins last for {(stopTime - startTime).TotalSeconds} seconds");
        return mills;
    }
}
