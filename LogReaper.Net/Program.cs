
using Autofac;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Elastic;
using LogReaper.Net.Service;
using Microsoft.Extensions.Configuration;
using System.Reflection.PortableExecutable;

namespace LogReaper.Net;

internal class Program
{
    private static IContainer container;
    private static string rootFolder;

    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(nameof(args));
        }

        rootFolder = args[0];

        TimeTracker timeTracker = new TimeTracker();
        timeTracker.StartTracking();

        SetupContainer();
        

        var baseListReader = container.Resolve<IGetBaseListService>();

        var logger = container.Resolve<ILocalLogger>();

        var config = container.Resolve<LogReaperConfig>();

        IList<BaseListRecord> baseList = baseListReader.Read(config.Files.LogDirectory);

        var converter = container.Resolve<ConvertRecordService>();
        converter.ReadRepresentations(rootFolder);
        converter.ReadFilter(rootFolder);

        logger.LogInfo("Обработка журналов регистрации баз");

        var reader = container.Resolve<LogReader>();

        foreach (BaseListRecord record in baseList)
        {
            if (!config.Bases.Contains(record.Name))
            {
                logger.LogInfo($"База [{record.Name}] пропущена, т.к. не задана в настройках");
                continue;
            }

            logger.LogInfo($"Обработка журнала базы [{record.Name}]");

            await reader.ReadDirectoryAsync(record);
        };

        timeTracker.StopTracking();
        
    }

    private static void SetupContainer()
    {
        var builder = new ContainerBuilder();

        IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(rootFolder, "appsettings.json"), true, true)
                    .Build();

        var config = configuration.Get<LogReaperConfig>();

        builder.RegisterInstance(config).AsSelf().SingleInstance();

        LocalLogger logger = new();
        builder.RegisterInstance(logger).As<ILocalLogger>().SingleInstance();

        builder.RegisterType<GetBaseListService>().As<IGetBaseListService>().SingleInstance();
        builder.RegisterType<SendElasticMessageService>().As<ISendElasticMessageService>().SingleInstance();
        builder.RegisterType<ConvertRecordService>().AsSelf().SingleInstance();
        builder.RegisterType<LogReader>().AsSelf().SingleInstance();

        container = builder.Build();
    }

}