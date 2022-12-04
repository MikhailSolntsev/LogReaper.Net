﻿using LogReaper.Net.Exceptions;
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
    private OdinAssLogDictionary dictionary = null!;

    private BaseListRecord baseRecord;

    public ProcessBaseDirectoryService(
        LogReaperConfig config,
        ConvertRecordService converter,
        ILocalLogger logger,
        ISendElasticMessageService elasticService,
        IRepresentFieldsService representFieldsService)
    {
        this.config = config;
        this.converter = converter;
        this.logger = logger;
        this.elasticService = elasticService;
        this.representFieldsService = representFieldsService;
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

        try
        {
            ManageProcessedFile(fileName);
        }
        catch (DirectoryCreationException e)
        {
            logger.LogError($"[{baseRecord.Name}] Ошибка создания каталога: {e.Message}");
        }
        catch (FileRenameException e)
        {
            logger.LogError($"[{baseRecord.Name}] Ошибка переименования файла: {e.Message}");
        }
        catch (Exception e)
        {
            logger.LogError($"[{baseRecord.Name}] Ошибка перемещения файла $fileName: ${e.Message}");
        }
    }

    private async Task ReadFileAsync(string fileName)
    {
        var messages = new List<ElasticRecord>();
        var period = PeriodFromFileName(fileName);
        var index = $"{baseRecord.Name}.log-{period}";
        var counter = 0;

        var journal = new ReadOdinAssLogFileService();
        journal.Open(fileName);

        while (!journal.EOF())
        {
            LogRecord? record = journal.ReadNext();
            
            if (record is not null)
            {
                ElasticRecord? elkRecord = converter.LogRecordToElasticRecord(record, dictionary);
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

        logger.LogInfo($"[{baseRecord.Name}] Найдено {counter} записей");
    }

    private async Task SendBulkToStorageAsync(List<ElasticRecord> messages, string index)
    {
        try
        {
            await elasticService.BulkPostAsync(converter.ElasticRecordsToElasticMessage(messages, index));
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
        logger.LogInfo($"[{baseRecord.Name}] Перемещение файла \"{fileName}\"");
        var directory = Path.Combine(config.Files.BackupDirectory, baseRecord.Name);
        FileHelper.MoveFile(fileName, directory);
    }

}