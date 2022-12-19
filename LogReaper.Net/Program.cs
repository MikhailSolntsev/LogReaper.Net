using Autofac;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Service;
using Microsoft.Extensions.Configuration;

namespace LogReaper.Net;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("Не указан каталог с настройками приложения", nameof(args));
        }

        var timeTracker = new TimeTracker();
        timeTracker.StartTracking();

        var config = ReadConfiguration(args[0]);

        var builder = new ContainerBuilder();

        builder.RegisterModule<LogReaperDiModule>();
        builder.RegisterConfig(config);
        builder.RegisterHttpClient();
        builder.RegisterBackupService();
        builder.RegisterElasticService();

        var container = builder.Build();

        container.Resolve<IRepresentFieldsService>().ReadRepresentationsFromDirectory(config.RootDirectory);
        container.Resolve<IFilterRecordsService>().ReadFiltersFromDirectory(config.RootDirectory);

        var processor = container.Resolve<LogProcessor>();

        await processor.ProcessLogsAsync();

        timeTracker.StopTracking();
        
    }

    private static LogReaperConfig ReadConfiguration(string rootFolder)
    {
        IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(rootFolder, "appsettings.json"), true, true)
                    .Build();

        var config = configuration.Get<LogReaperConfig>();
        if (config is null)
        {
            throw new ArgumentException("Файл конфигурации 'appsettings.json' отсутствует или не не заполнен");
        }
        config.RootDirectory = rootFolder;

        return config;
    }


}