using Autofac;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Elastic;
using LogReaper.Net.Service;
using Microsoft.Extensions.Configuration;

namespace LogReaper.Net;

internal class Program
{
    private static string rootFolder;

    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("Не указан каталог с настройками приложения", nameof(args));
        }

        rootFolder = args[0];

        var timeTracker = new TimeTracker();
        timeTracker.StartTracking();

        var config = ReadConfiguration(args[0]);

        IContainer container = SetupContainer(config);

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

    private static IContainer SetupContainer(LogReaperConfig config)
    {
        var builder = new ContainerBuilder();

        builder.RegisterInstance(config).AsSelf().SingleInstance();

        LocalLogger logger = new();
        builder.RegisterInstance(logger).As<ILocalLogger>().SingleInstance();

        builder.RegisterType<GetBaseListService>()
            .As<IGetBaseListService>()
            .SingleInstance();

        builder.RegisterType<SendElasticMessageService>()
            .As<ISendElasticMessageService>()
            .SingleInstance();

        builder.RegisterType<ConvertRecordService>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<ProcessBaseDirectoryService>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<FilterRecordsService>()
            .As<IFilterRecordsService>()
            .SingleInstance();

        builder.RegisterType<RepresentFieldsService>()
            .As<IRepresentFieldsService>()
            .SingleInstance();

        builder.RegisterType<LogProcessor>()
            .AsSelf()
            .SingleInstance();

        return builder.Build();
    }

}