using LogReaper.Net.Service;
using LogReaper.Net.Dto;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;

namespace LogReaper.Net;

public class ProcessBaseDirectoryService
{
    private readonly LogReaperConfig config;
    private readonly ILocalLogger logger;
    private readonly ConvertRecordService converter;
    private readonly ISendElasticMessageService elasticService;
    private readonly IRepresentFieldsService representFieldsService;
    private readonly IBackupProcessedFileService backupProcessedFileService;

    private OdinAssLogDictionary dictionary = null!;

    private BaseListRecord baseRecord;

    public ProcessBaseDirectoryService(
        LogReaperConfig config,
        ConvertRecordService converter,
        ILocalLogger logger,
        ISendElasticMessageService elasticService,
        IRepresentFieldsService representFieldsService,
        IBackupProcessedFileService backupProcessedFileService)
    {
        this.config = config;
        this.converter = converter;
        this.logger = logger;
        this.elasticService = elasticService;
        this.representFieldsService = representFieldsService;
        this.backupProcessedFileService = backupProcessedFileService;
        baseRecord = new();
    }

    public async Task ProcessDirectoryAsync(BaseListRecord baseRecord)
    {
        this.baseRecord = baseRecord;
        dictionary = new();

        var directory = Path.Combine(config.Files.LogDirectory, baseRecord.Uid);
        var files = FileHelper.GetUnlockedFiles(directory);

        logger.LogInfo($"[{baseRecord.Name}] Найдено {files.Count} файлов");

        if (files.Count == 0)
        {
            return;
        }

        logger.LogInfo($"[{baseRecord.Name}] Чтение словаря");

        var readLogDictionaryService = new ReadOdinAssLogDictionaryService();
        dictionary = readLogDictionaryService.ReadOdinAssDictionaryInDirectory(directory);

        representFieldsService.UseDictionary(dictionary);

        foreach (var fileName in files)
        {
            logger.LogInfo($"[{baseRecord.Name}] Чтение файла \'{fileName}\'");
            await ProcessFileAsync(fileName);
        }

        logger.LogInfo($"[{baseRecord.Name}] Завершено");
    }

    private async Task ProcessFileAsync(string fileName)
    {

        try
        {
            await ReadFileAsync(fileName);
        }
        catch (Exception e) {
            logger.LogError($"[{baseRecord.Name}] Ошибка чтения файла \'{fileName}\': {e.Message}");
            logger.LogError(e.StackTrace);
            return;
        }

        backupProcessedFileService.BackupProcessedFile(fileName, baseRecord);
    }

    private async Task ReadFileAsync(string fileName)
    {
        var messages = new List<ElasticRecord>();
        
        var period = PeriodFromFileName(fileName);
        var index = $"onec-{baseRecord.Name}.log-{period}";
        elasticService.UseDefaultIndex(index);

        var counter = 0;

        var readLogFileService = new ReadOdinAssLogFileService();

        using var fileStream = File.OpenRead(fileName);
        using var textReader = new StreamReader(fileStream);
        readLogFileService.Open(textReader);

        while (!readLogFileService.EOF())
        {
            LogRecord? record = readLogFileService.ReadNextRecord();
            
            if (record is not null)
            {
                record.Infobase = baseRecord.Name;
                ElasticRecord? elkRecord = converter.LogRecordToElasticRecord(record, dictionary);
                if (elkRecord is not null)
                {
                    messages.Add(elkRecord);
                }
            }

            if (messages.Count == config.Elastic.BulkSize)
            {
                counter += messages.Count;
                await elasticService.SendBulkToStorageAsync(messages);
                messages.Clear();
            }
        }

        if (messages.Count > 0)
        {
            counter += messages.Count;
            await elasticService.SendBulkToStorageAsync(messages);
            messages.Clear();
        }

        textReader.Close();
        fileStream.Close();

        logger.LogInfo($"[{baseRecord.Name}] Найдено {counter} записей");
    }

    private string PeriodFromFileName(string fileName)
    {
        var file = Path.GetFileNameWithoutExtension(fileName);
        string result = $"{file[0..4]}-{file[4..6]}-{file[6..8]}";
        return result;
    }

}
