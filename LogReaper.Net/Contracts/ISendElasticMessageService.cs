namespace LogReaper.Net.Contracts
{
    public interface ISendElasticMessageService
    {
        Task BulkPostAsync(string data);
    }
}