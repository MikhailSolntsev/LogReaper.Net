using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    public interface IGetBaseListService
    {
        IList<BaseListRecord> Read(string directory);
    }
}