using LogReaper.Net.Service;
using Newtonsoft.Json.Linq;

namespace LogReaper.Net.Tests;

public class SpeedTest
{
    [Fact]
    public void LinqShouldWorkFaster()
    {
        TimeTracker tracker = new();

        tracker.StartTracking();

        int result = 0;
        for (int i = 0; i < 100000; i++)
        {
            result += FindMinLinQ(2, 20, 3, 24);
        }
        var mils = tracker.StopTracking();
        Console.WriteLine($"LINQ result = {result}");

        mils.Should().Be(0);
    }

    [Fact]
    public void RangeShouldWorkFaster()
    {
        TimeTracker tracker = new();

        tracker.StartTracking();

        int result = 0;
        for (int i = 0; i < 100000; i++)
        {
            result += FindMinRange(2, 20, 3, 24);
        }

        var mils = tracker.StopTracking();
        Console.WriteLine($"Range result = {result}");

        mils.Should().Be(0);
    }

    private int FindMinLinQ(int value1, int value2, int value3, int value4)
    {
        int result = Math.Max(Math.Max(value1, value2), Math.Max(value3, value4));
        int[] range = new[] { value1, value2, value3, value4 };
        result = range.Where(v => v >= 0).Min();
        return result;
    }

    private int FindMinRange(int value1, int value2, int value3, int value4)
    {
        int result = Math.Max(Math.Max(value1, value2), Math.Max(value3, value4));

        var range = Enumerable.Range(0, result);

        if (range.Contains(value1)) result = value1;
        if (range.Contains(value2)) result = value2;
        if (range.Contains(value3)) result = value3;
        if (range.Contains(value4)) result = value4;

        return result;
    }
}
