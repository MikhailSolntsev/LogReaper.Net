
using LogReaper.Net.Models;
using LogReaper.Net.Exceptions;
using LogReaper.Net.Service;
using LogReaper.Net.Elastic;

namespace LogReaper.Net;

internal class LogReader
{
    private Configuration config;
    private BaseListRecord baseRecord;
    private RecordConverter converter;

    private LogDictionary dictionary = new();
    private ElkSender sender;

    private ILocalLogger logger;

    public LogReader(Configuration config, BaseListRecord baseRecord, RecordConverter converter, ILocalLogger logger)
    {
        this.config = config;
        this.baseRecord = baseRecord;
        this.converter = converter;
        this.logger = logger;
        sender = new ElkSender(config.ElkUrl, logger);
    }

    public async Task ReadAsync()
    {
        var directory = Path.Combine(config.LogDirectory, baseRecord.Uid);
        var files = FileHelper.GetUnlockedFiles(directory);

        logger.LogInfo($"Найдено ${files.Count} файлов", baseRecord.Name);

        if (files.Count == 0)
        {
            return;
        }

        logger.LogInfo("Чтение словаря", baseRecord.Name);
        dictionary.Read(directory);

        foreach (var fileName in files)
        {
            logger.LogInfo($"Чтение файла \'{fileName}\'", baseRecord.Name);
            await ProcessFileAsync(fileName);
        }

        logger.LogInfo("Завершено", baseRecord.Name);
        dictionary.Clear();
    }

    private async Task ProcessFileAsync(string fileName)
    {

        try
        {
            await ReadFileAsync(fileName);
        }
        catch (Exception e) {
            logger.LogError($"Ошибка чтения файла \'{fileName}\': {e.Message}", baseRecord.Name);
            logger.LogError(e.StackTrace);
            return;
        }

        try
        {
            ManageProcessedFile(fileName);
        }
        catch (DirectoryCreationException e)
        {
            logger.LogError($"Ошибка создания каталога: {e.Message}", baseRecord.Name);
        }
        catch (FileRenameException e)
        {
            logger.LogError($"Ошибка переименования файла: {e.Message}", baseRecord.Name);
        }
        catch (Exception e)
        {
            logger.LogError($"Ошибка перемещения файла $fileName: ${e.Message}", baseRecord.Name);
        }
    }

    private async Task ReadFileAsync(string fileName)
    {
        var messages = new List<ElkRecord>();
        var period = PeriodFromFileName(fileName);
        var index = $"{baseRecord.Name}.log-{period}";
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

            if (messages.Count == config.BulkSize)
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

        logger.LogInfo($"Найдено {counter} записей", baseRecord.Name);
    }

    private async Task SendBulkToStorageAsync(List<ElkRecord> messages, string index)
    {
        try
        {
            await sender.BulkPostAsync(converter.ElkRecordListToElkMessage(messages, index));
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

    private void ManageProcessedFile(string fileName)
    {
        logger.LogInfo("Перемещение файла \"$fileName\"", baseRecord.Name);
        var directory = Path.Combine(config.BackupDirectory, baseRecord.Name);
        FileHelper.MoveFile(fileName, directory);
    }

}
