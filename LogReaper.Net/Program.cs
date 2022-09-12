
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

        Configuration config = ConfigReader.ReadConfig(args[0], logger);

        var baseList = BaseListReader.Read(config.LogDirectory, logger);

        var converter = new RecordConverter(logger);
        converter.ReadRepresentations(args[0]);
        converter.ReadFilter(args[0]);

        logger.LogInfo("Обработка журналов регистрации баз");

        foreach (BaseListRecord record in baseList)
        {
            if (!config.Bases.Contains(record.Name))
            {
                logger.LogInfo($"База {record.Name} пропущена, т.к. не задана в настройках");
                continue;
            }

            LogReader reader = new (config, record, converter, logger);
            await reader.ReadDirectoryAsync();
        };

        timeTracker.StopTracking();
        
    }
}