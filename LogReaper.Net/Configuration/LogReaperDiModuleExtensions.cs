using Autofac;
using LogReaper.Net.Contracts;
using LogReaper.Net.Elastic;

namespace LogReaper.Net.Configuration
{
    public static class LogReaperDiModuleExtensions
    {
        public static void RegisterLogger(this ContainerBuilder builder)
        {
            LocalLogger logger = new();
            builder.RegisterInstance(logger).As<ILocalLogger>().SingleInstance();
        }

        public static void RegisterConfig(this ContainerBuilder builder, LogReaperConfig config)
        {
            builder.RegisterInstance(config).AsSelf().SingleInstance();
        }

        public static void RegisterHttpClient(this ContainerBuilder builder)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            builder.RegisterInstance(httpClient).As<HttpClient>().SingleInstance();
        }

        public static void RegisterElasticService(this ContainerBuilder builder)
        {
            builder.RegisterType<SendElasticMessageService>()
                .As<ISendElasticMessageService>()
                .SingleInstance();
        }

        public static void RegisterBackupService(this ContainerBuilder builder)
        {
            builder.RegisterType<BackupProcessedFileService>().AsSelf().SingleInstance();
        }
    }
}
