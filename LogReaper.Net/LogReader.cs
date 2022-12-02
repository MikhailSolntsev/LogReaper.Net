using LogReaper.Net.Exceptions;
using LogReaper.Net.Service;
using LogReaper.Net.Dto;
using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;

namespace LogReaper.Net;

internal class LogReader
{
    private readonly LogReaperConfig config;
    private readonly ILocalLogger logger;
    private readonly ConvertRecordService converter;
    private readonly ISendElasticMessageService elasticService;

    private LogDictionary dictionary = new();

    //private BaseListRecord baseRecord;

    public LogReader(
        LogReaperConfig config,
        ConvertRecordService converter,
        ILocalLogger logger,
        ISendElasticMessageService elasticService)
    {
        this.config = config;
        this.converter = converter;
        this.logger = logger;
        this.elasticService = elasticService;
    }

    public async Task ReadDirectoryAsync(BaseListRecord baseRecord)
    {
        dictionary.Clear();

        var directory = Path.Combine(config.Files.LogDirectory, baseRecord.Uid);
        var files = FileHelper.GetUnlockedFiles(directory);

        logger.LogInfo($"[{baseRecord.Name}] Найдено ${files.Count} файлов");

        if (files.Count == 0)
        {
            return;
        }

        logger.LogInfo($"[{baseRecord.Name}] Чтение словаря");

        dictionary.Read(directory);

        foreach (var fileName in files)
        {
            logger.LogInfo($"[{baseRecord.Name}] Чтение файла \'{fileName}\'");
            await ProcessFileAsync(fileName, baseRecord.Name);
        }

        logger.LogInfo($"[{baseRecord.Name}] Завершено");

        dictionary.Clear();
    }

    private async Task ProcessFileAsync(string fileName, string baseName)
    {

        try
        {
            await ReadFileAsync(fileName, baseName);
        }
        catch (Exception e) {
            logger.LogError($"[{baseName}] Ошибка чтения файла \'{fileName}\': {e.Message}");
            logger.LogError(e.StackTrace);
            return;
        }

        try
        {
            ManageProcessedFile(fileName, baseName);
        }
        catch (DirectoryCreationException e)
        {
            logger.LogError($"[{baseName}] Ошибка создания каталога: {e.Message}");
        }
        catch (FileRenameException e)
        {
            logger.LogError($"[{baseName}] Ошибка переименования файла: {e.Message}");
        }
        catch (Exception e)
        {
            logger.LogError($"[{baseName}] Ошибка перемещения файла $fileName: ${e.Message}");
        }
    }

    private async Task ReadFileAsync(string fileName, string baseName)
    {
        var messages = new List<ElkRecord>();
        var period = PeriodFromFileName(fileName);
        var index = $"{baseName}.log-{period}";
        var counter = 0;

        var journal = new LogJournal();
        journal.Open(fileName);

        while (!journal.EOF())
        {
            LogRecord? record = journal.ReadNext();
            
            if (record is not null)
            {
                ElkRecord? elkRecord = converter.LogRecordToElkRecord(record, dictionary);
                if (elkRecord is not null)
                {
                    messages.Add(elkRecord);
                }
            }

            if (messages.Count == config.Elastic.BulkSize)
            {
                counter += messages.Count;
                await SendBulkToStorageAsync(messages, index);
            }
        }

        if (messages.Count > 0)
        {
            counter += messages.Count;
            await SendBulkToStorageAsync(messages, index);
        }
        
        journal.Close();

        logger.LogInfo($"[{baseName}] Найдено {counter} записей");
    }

    private async Task SendBulkToStorageAsync(List<ElkRecord> messages, string index)
    {
        try
        {
            await elasticService.BulkPostAsync(converter.ElkRecordListToElkMessage(messages, index));
        }
        catch (Exception e)
        {
            logger.LogError($"Ошибка записи пачки в Elastic: {e.Message}");
            throw;
        }

        messages.Clear();
    }

    private string PeriodFromFileName(string fileName)
    {
        var file = Path.GetFileNameWithoutExtension(fileName);
        string result = $"{file[0..4]}-{file[4..6]}-{file[6..8]}";
        return result;
    }

    private void ManageProcessedFile(string fileName, string baseName)
    {
        logger.LogInfo($"[{baseName}] Перемещение файла \"{fileName}\"");
        var directory = Path.Combine(config.Files.BackupDirectory, baseName);
        FileHelper.MoveFile(fileName, directory);
    }

}
