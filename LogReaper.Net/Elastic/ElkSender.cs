using LogReaper.Net.Service;

namespace LogReaper.Net.Elastic;

public class ElkSender
{
    private readonly string baseUrl;
    private readonly ILocalLogger logger;

    public ElkSender(string baseUrl, ILocalLogger logger)
    {
        this.baseUrl = baseUrl;
        this.logger = logger;
    }

    public async Task BulkPostAsync(string data)
    {
        await DoRequestAsync($"{baseUrl}/_bulk", data);
    }
    
    private async Task DoRequestAsync(string url, string data)
    {
        logger.LogDebug("Sending message...");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        var content = new StringContent(data);
        var response = await client.PostAsync(url, content);

        logger.LogDebug(response.ToString());
    }

}