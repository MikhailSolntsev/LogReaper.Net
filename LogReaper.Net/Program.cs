using LogReaper.Net.Models;

namespace LogReaper.Net;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        var startTime = DateTime.UtcNow.Ticks;

        var config = new LogConfig(args[0]);
        var converter = new RecordConverter();

        converter.ReadConfig(args[0]);

        var baseList = new BaseList();
        baseList.Read(config.LogDirectory);

        var bases = baseList.Bases.Where(it => config.Bases.Contains(it.Name)).ToList();

        foreach (BaseListRecord record in bases)
        {
            LogReader reader = new LogReader(config, record, converter);
            await reader.ReadAsync();
        };

        var endTime = DateTime.UtcNow.Ticks;

        Console.WriteLine($"Завершено. Длительность выполнения: {(endTime - startTime) / 1000} секунд\"");
    }
}