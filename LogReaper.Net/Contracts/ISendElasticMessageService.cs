using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    public interface ISendElasticMessageService
    {
        Task SendBulkToStorageAsync(List<ElasticRecord> messages);
        void UseDefaultIndex(string index);
    }
}