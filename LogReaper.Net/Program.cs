
using LogReaper.Net.Models;
using LogReaper.Net.Service;

namespace LogReaper.Net;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No parameters was set");
            return;
        }

        TimeTracker timeTracker = new TimeTracker();
        timeTracker.StartTracking();

        LocalLogger logger = new();

        Configuration config = ConfigReader.ReadConfig(args[0]);

        var baseList = BaseList.Read(config.LogDirectory);

        var converter = new RecordConverter();
        converter.ReadRepresentations(config.LogDirectory);
        converter.ReadFilter(config.LogDirectory);

        foreach (BaseListRecord record in baseList)
        {
            if (!config.Bases.Contains(record.Name))
            {
                continue;
            }

            LogReader reader = new (config, record, converter, logger);
            await reader.ReadAsync();
        };

        timeTracker.StopTracking();
        
    }
}