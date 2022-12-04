using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Service;
using Microsoft.Extensions.Logging;

namespace LogReaper.Net
{
    public class FilterRecordsService: IFilterRecordsService
    {
        private FilterData filters = null!;
        private readonly ILocalLogger logger;
        private readonly IRepresentFieldsService representFieldsService;

        public FilterRecordsService(ILocalLogger logger, IRepresentFieldsService representFieldsService)
        {
            this.logger = logger;
            this.representFieldsService = representFieldsService;
        }

        public void ReadFiltersFromDirectory(string rootDirectory)
        {
            string fullFileName = Path.Combine(rootDirectory, "filter.json");

            logger.LogInfo($"Чтение фильтров из файла {fullFileName}");

            var stream = File.OpenRead(fullFileName);

            FilterData? filterData = ReadJsonService<FilterData>.Deserialize(stream);

            if (filterData is null)
            {
                throw new Exception($"Can't read config file {fullFileName}");
            }

            filters = filterData;

            logger.LogInfo("Чтение фильтров завершено");
        }

        public bool IsFiltered(LogRecord logRecord)
        {
            var element = representFieldsService.RepresentEvent(logRecord);
            if (filters.Events.Contains(element)) return true;

            element = representFieldsService.RepresentLevel(logRecord);
            if (filters.Levels.Contains(element)) return true;

            element = representFieldsService.RepresentTransaction(logRecord);
            if (filters.TransactionStatuses.Contains(element)) return true;

            return false;
        }
        private class FilterData
        {
            public List<string> Events { get; set; } = null!;
            public List<string> Levels { get; set; } = null!;
            public List<string> TransactionStatuses { get; set; } = null!;
        }
    }
}
