using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    public interface IFilterRecordsService
    {
        bool IsFiltered(LogRecord logRecord);
        void ReadFiltersFromDirectory(string rootDirectory);
    }
}