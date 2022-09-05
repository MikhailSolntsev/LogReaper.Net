
using Microsoft.Extensions.Logging;

namespace LogReaper.Net;

public class ElkSender
{
    private readonly string baseUrl;
    private readonly ILogger logger;

    public ElkSender(string baseUrl, ILogger logger)
    {
        this.baseUrl = baseUrl;
        this.logger = logger;
    }

    public async Task BulkPostAsync(string data)
    {
        await DoRequestAsync($"{baseUrl}/_bulk", data);
    }

    public async Task PostAsync(string index, string data)
    {
        await DoRequestAsync($"{baseUrl}/{index}/_doc", data);
    }

    private async Task DoRequestAsync(string url, string data)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        var content = new StringContent(data);
        var response = await client.PostAsync(url, content);

        logger.LogInformation(response.ToString());
    }

}