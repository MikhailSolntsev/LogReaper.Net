using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    public interface IGetBaseListService
    {
        IReadOnlyCollection<BaseListRecord> Read(string directory);
    }
}