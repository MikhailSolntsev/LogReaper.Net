
using LogReaper.Net.Models;
using System.Collections.Generic;
using System.Reflection;
using System;
using Microsoft.Extensions.Logging;
using LogReaper.Net.Exceptions;

namespace LogReaper.Net;

internal class LogReader
{
    private LogConfig config;
    private BaseListRecord baseRecord;
    private RecordConverter converter;

    private LogDictionary dictionary = new();
    private ElkSender sender;

    private ILogger logger;

    public LogReader(LogConfig config, BaseListRecord baseRecord, RecordConverter converter)
    {
        this.config = config;
        this.baseRecord = baseRecord;
        this.converter = converter;
        sender = new ElkSender(config.ElkUrl, logger);
    }

    public LogReader(LogConfig config, BaseListRecord baseRecord, RecordConverter converter, ElkSender sender)
    {
        this.config = config;
        this.baseRecord = baseRecord;
        this.converter = converter;
        this.sender = sender;
    }

    public async Task ReadAsync()
    {
        var directory = Path.Combine(config.LogDirectory, baseRecord.Uid);
        var files = FileHelper.GetUnlockedFiles(directory);

        logger.Log(LogLevel.Information, $"Найдено ${files.Count} файлов", baseRecord.Name);

        if (files.Count == 0)
        {
            return;
        }

        logger.Log(LogLevel.Information, "Чтение словаря", baseRecord.Name);
        dictionary.Read(directory);

        foreach (var fileName in files)
        {
            logger.Log(LogLevel.Information, $"Чтение файла \'{fileName}\'", baseRecord.Name);
            await ProcessFileAsync(fileName);
        }

        logger.Log(LogLevel.Information, "Завершено", baseRecord.Name);
        dictionary.Clear();
    }

    private async Task ProcessFileAsync(string fileName)
    {

        try
        {
            await ReadFileAsync(fileName);
        }
        catch (Exception e) {
            logger.Log(LogLevel.Information, $"Ошибка чтения файла \'{fileName}\': {e.Message}", baseRecord.Name);
            logger.Log(LogLevel.Error, message: e.StackTrace);
            return;
        }

        try
        {
            ManageProcessedFile(fileName);
        }
        catch (DirectoryCreationException e)
        {
            logger.Log(LogLevel.Information, $"Ошибка создания каталога: {e.Message}", baseRecord.Name);
        }
        catch (FileRenameException e)
        {
            logger.Log(LogLevel.Information, $"Ошибка переименования файла: {e.Message}", baseRecord.Name);
        }
        catch (Exception e)
        {
            logger.Log(LogLevel.Information, $"Ошибка перемещения файла $fileName: ${e.Message}", baseRecord.Name);
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
                ElkRecord elkRecord = converter.Convert(record, dictionary);
                if (elkRecord is not null)
                {
                    messages.Add(elkRecord);
                }
            }

            if (messages.Count == config.BulkSize)
            {
                try
                {
                    await sender.BulkPostAsync(converter.ConvertListToMessage(messages, index));
                }
                catch (Exception e)
                {
                    message("Ошибка записи пачки в Elastic: ${e.message}");
                    journal.Close();
                    throw;
                }
                counter += messages.Count;
                messages.Clear();
            }
        }
        journal.Close();

        if (messages.Count > 0)
        {
                try
                {
                await sender.BulkPostAsync(converter.ConvertListToMessage(messages, index));
                }
                catch (Exception e)
                {
                    message("Ошибка записи пачки в Elastic: ${e.message}");
                    journal.Close();
                    throw;
                }
            counter += messages.Count;
        }

        message("Найдено $counter записей", baseRecord.Name);
    }

    private string PeriodFromFileName(string fileName)
    {
        var file = Path.GetFileNameWithoutExtension(fileName);
        string result = $"{file[0..4]}-{file[4..6]}-{file[6..8]}";
        return result;
    }

    private void ManageProcessedFile(string fileName)
    {
        message("Перемещение файла \"$fileName\"", baseRecord.Name);
        var directory = Path.Combine(config.BackupDirectory, baseRecord.Name);
        FileHelper.MoveFile(fileName, directory);
    }

    private void message(string v, string name = "")
    {
        throw new NotImplementedException();
    }
}
