using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using Polly;
using Polly.Fallback;

namespace LogReaper.Net.Elastic;

public sealed class SendElasticMessageService : ISendElasticMessageService
{
    private readonly LogReaperConfig config;
    private readonly ILocalLogger logger;
    private readonly HttpClient httpClient;
    private readonly ConvertRecordService convertRecordService;

    private string index = String.Empty;

    public SendElasticMessageService(
        LogReaperConfig config,
        ILocalLogger logger,
        HttpClient httpClient,
        ConvertRecordService convertRecordService)
    {
        this.config = config;
        this.logger = logger;
        this.httpClient = httpClient;
        this.convertRecordService = convertRecordService;
    }

    public Task SendBulkToStorageAsync(List<ElasticRecord> messages)
    {
        return Task.Run(async () =>
        {
            try
            {
                await MakeRequestAsync($"{config.Elastic.Url}/_bulk", convertRecordService.ElasticRecordsToElasticMessage(messages, index));
            }
            catch (Exception e)
            {
                logger.LogError($"Ошибка записи пачки в Elastic: {e.Message}");
                throw;
            }
        });
    }

    public void UseDefaultIndex(string index)
    {
        this.index = index;
    }

    private async Task MakeRequestAsync(string url, string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        logger.LogDebug("Sending message...");

        var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);

        logger.LogDebug($"Status code: {response.StatusCode}");
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            logger.LogError(response.ToString());
            logger.FileLog(data);
        }
    }

    private static AsyncPolicy<TResult> CreateAsyncPolicy<TResult>(int retryCount, Func<int, TimeSpan> sleepDurationProvider)
    {
        var fallback = GetHttpRequestExceptionFallbackPolicy<TResult>();

        var retry = Policy<TResult>.Handle<HttpRequestException>()
            .WaitAndRetryAsync(retryCount, sleepDurationProvider);

        return fallback.WrapAsync(retry);
    }

    private static AsyncFallbackPolicy<TResult> GetHttpRequestExceptionFallbackPolicy<TResult>()
        => Policy<TResult>.Handle<HttpRequestException>()
            .FallbackAsync(
                default(TResult),
                (outcome, context) => throw new ExternalApiUnavailableException(outcome.Exception)
            );

    private class ExternalApiUnavailableException : Exception
    {
        private Exception exception;

        public ExternalApiUnavailableException(Exception exception)
        {
            this.exception = exception;
        }
    }
}