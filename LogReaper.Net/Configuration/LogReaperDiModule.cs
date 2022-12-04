using Autofac;
using LogReaper.Net.Contracts;

namespace LogReaper.Net.Configuration
{
    public class LogReaperDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GetBaseListService>()
            .As<IGetBaseListService>()
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

            base.Load(builder);
        }
    }
}
