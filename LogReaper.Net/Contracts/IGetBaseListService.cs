using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    internal interface IGetBaseListService
    {
        IList<BaseListRecord> Read(string directory);
    }
}