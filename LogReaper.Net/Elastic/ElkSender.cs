using LogReaper.Net.Service;
using static System.Net.Mime.MediaTypeNames;

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