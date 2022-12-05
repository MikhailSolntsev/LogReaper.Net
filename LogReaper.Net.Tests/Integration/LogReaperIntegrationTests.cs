using Autofac;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Tests.Mock;
using Moq;

namespace LogReaper.Net.Tests.Integration
{
    public partial class LogReaperIntegrationTests
    {
        private readonly IContainer container;

        private readonly BaseListRecord testBaseListRecord;
        private readonly Mocks mockCollection;

        private readonly MockDataAccessor<ElasticRecord> elasticDataAccessor;

        private readonly LogProcessor logProcessor;

        public LogReaperIntegrationTests()
        {
            testBaseListRecord = new BaseListRecord { Name = "TestBase", Uid=Guid.NewGuid().ToString() };

            mockCollection = new Mocks(testBaseListRecord);

            elasticDataAccessor = mockCollection.ElasticMock.SetupForElastic();

            container = CreateContainer();

            var processor = container.Resolve<LogProcessor>();
        }

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            var config = CreateConfig();
            builder.RegisterInstance(config).As<LogReaperConfig>().SingleInstance();

            builder.RegisterType<LocalLogger>().As<ILocalLogger>().SingleInstance();

            builder.RegisterType<ConvertRecordService>().AsSelf().SingleInstance();

            builder.RegisterType<ProcessBaseDirectoryService>().AsSelf().SingleInstance();

            builder.RegisterType<RepresentFieldsService>().As<IRepresentFieldsService>().SingleInstance();

            builder.RegisterType<LogProcessor>().AsSelf().SingleInstance();

            builder.RegisterInstance(mockCollection.FilterService.Object).As<IFilterRecordsService>().SingleInstance();

            builder.RegisterInstance(mockCollection.BaseListService).As<IGetBaseListService>().SingleInstance();

            builder.RegisterInstance(mockCollection.ElasticMock.Object).As<ISendElasticMessageService>().SingleInstance();

            builder.RegisterInstance(mockCollection.BackupService.Object).As<IBackupProcessedFileService>().SingleInstance();

            return builder.Build();
        }

        private LogReaperConfig CreateConfig()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var directoryInfo = Directory.CreateDirectory(path);

            var config = new LogReaperConfig()
            {
                Bases = new[] { testBaseListRecord.Name },
                Elastic = new ElasticConfig { BulkSize = 2, Url = "" },
                Files = new FilesConfig { BackupDirectory = "", LogDirectory = "" },
                RootDirectory = directoryInfo.FullName,
            };
            return config;
        }

        private class Mocks
        {
            public Mock<ISendElasticMessageService> ElasticMock { get; set; }
            public Mock<IBackupProcessedFileService> BackupService { get; set; }
            public Mock<IGetBaseListService> BaseListService { get; set; }
            public Mock<IFilterRecordsService> FilterService { get; set; }

            public Mocks(BaseListRecord testBaseListRecord)
            {
                ElasticMock = new Mock<ISendElasticMessageService>();
                
                BackupService = new Mock<IBackupProcessedFileService>();
                BackupService.Setup(s => s.BackupProcessedFile(It.IsAny<string>(), It.IsAny<BaseListRecord>()));

                BaseListService = new Mock<IGetBaseListService>();
                BaseListService.Setup(s => s.Read(It.IsAny<string>())).Returns(new BaseListRecord[] { testBaseListRecord });

                FilterService = new Mock<IFilterRecordsService>();
                FilterService.Setup(s => s.IsFiltered(It.IsAny<LogRecord>())).Returns(false);
            }
        }
    }
}
