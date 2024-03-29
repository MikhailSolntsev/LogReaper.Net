﻿
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Service;

namespace LogReaper.Net;

internal class GetBaseListService : IGetBaseListService
{
    private readonly ILocalLogger logger;

    public GetBaseListService(ILocalLogger logger)
    {
        this.logger = logger;
    }

    public IReadOnlyCollection<BaseListRecord> Read(string directory)
    {
        var fileName = Path.Combine(directory, "1CV8Clst.lst");

        logger.LogInfo($"Чтение списка баз из файла {fileName}");

        var basesStringStructure = ReadBasesStringStructure(fileName);
        var basesRawList = GetBasesStringList(basesStringStructure);

        var bases = new List<BaseListRecord>();

        foreach (string record in basesRawList)
        {
            bases.Add(ProcessRawBase(record));
        }

        logger.LogInfo($"Чтение списка баз завершено. Прочитано {bases.Count} элементов");

        return bases;
    }

    private string ReadBasesStringStructure(string fileName)
    {
        var fileStream = File.OpenRead(fileName);
        var fileReader = new StreamReader(fileStream);
        var readMachine = new ReadAnyOdinAssFileService(fileReader);
        var basesRaw = string.Empty;

        if (!readMachine.EOF()) {
            if (readMachine.ReadBegin()) {
                readMachine.ReadValue(); //  0 in first position
                readMachine.ReadStructure(); //  server information
                basesRaw = readMachine.ReadStructure(); //  baselist structure
}
        }

        fileReader.Close();
        fileStream.Close();

        return basesRaw;
    }

    private IReadOnlyCollection<string> GetBasesStringList(string basesRaw)
    {
        var reader = new StringReader(basesRaw);
        var readMachine = new ReadAnyOdinAssFileService(reader);
        var rawList = new List<string>();

        while (!readMachine.EOF())
        {
            if (readMachine.ReadBegin())
            {
                readMachine.ReadValue(); //  number of bases
                var record = readMachine.ReadStructure().Trim();
                while (!string.IsNullOrEmpty(record))
                {
                    rawList.Add(record);
                    record = readMachine.ReadStructure().Trim();
                }
            }
        }

        reader.Close();

        return rawList;
    }

    private BaseListRecord ProcessRawBase(string rawBase)
    {
        var reader = new StringReader(rawBase);
        var readMachine = new ReadAnyOdinAssFileService(reader);
        var record = new BaseListRecord();

        if (!readMachine.EOF())
        {
            if (readMachine.ReadBegin())
            {
                record.Uid = readMachine.ReadStructure();
                record.Name = readMachine.ReadStructure().ToLower().CutQuotes();
                readMachine.ReadStructure(); //  SQL Server type
                record.SqlServer = readMachine.ReadStructure().CutQuotes();
                record.SqlBase = readMachine.ReadStructure().CutQuotes();
            }
        }

        return record;
    }

}