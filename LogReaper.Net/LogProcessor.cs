using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;

namespace LogReaper.Net
{
    public class LogProcessor
    {
        private readonly ILocalLogger logger;
        private readonly IGetBaseListService getBaseListService;
        private readonly LogReaperConfig config;
        private readonly ProcessBaseDirectoryService reader;

        public LogProcessor(ProcessBaseDirectoryService reader,
            LogReaperConfig config,
            IGetBaseListService getBaseListService,
            ILocalLogger logger)
        {
            this.reader = reader;
            this.config = config;
            this.getBaseListService = getBaseListService;
            this.logger = logger;
        }

        public async Task ProcessLogsAsync()
        {
            
            var baseList = getBaseListService.Read(config.Files.LogDirectory);

            foreach (BaseListRecord record in baseList)
            {
                if (!config.Bases.Contains(record.Name))
                {
                    logger.LogInfo($"База [{record.Name}] пропущена, т.к. не задана в настройках");
                    continue;
                }

                logger.LogInfo($"Обработка журнала базы [{record.Name}]");

                await reader.ProcessDirectoryAsync(record);
            };
        }
    }
}
