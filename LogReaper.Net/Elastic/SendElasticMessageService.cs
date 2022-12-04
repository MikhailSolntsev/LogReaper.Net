using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using static System.Net.Mime.MediaTypeNames;

namespace LogReaper.Net.Elastic;

public sealed class SendElasticMessageService : ISendElasticMessageService
{
    private readonly string baseUrl;
    private readonly ILocalLogger logger;

    public SendElasticMessageService(LogReaperConfig config, ILocalLogger logger)
    {
        this.baseUrl = config.Elastic.Url;
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
        client.DefaultRequestHeaders
            .Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        logger.LogDebug($"Status code: {response.StatusCode}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            logger.LogError(response.ToString());
        }
    }

}