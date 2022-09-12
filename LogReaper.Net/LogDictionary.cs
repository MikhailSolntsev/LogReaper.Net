
using LogReaper.Net.Models;

namespace LogReaper.Net;

public class LogDictionary
{
    private const string fileName = "1Cv8Log\\1Cv8.lgf";
    private readonly uint[] dictionaryType = {1, 5, 9, 10, 11, 12};
    public Dictionary<uint, string> Users { get; set; } = new();
    public Dictionary<uint, string> Computers { get; set; } = new();
    public Dictionary<uint, string> Applications { get; set; } = new();
    public Dictionary<uint, string> Events { get; set; } = new();
    public Dictionary<uint, string> Metadata { get; set; } = new();
    public Dictionary<uint, string> Servers { get; set; } = new();

    public void Clear()
    {
        Users.Clear();
        Computers.Clear();
        Applications.Clear();
        Events.Clear();
        Metadata.Clear();
        Servers.Clear();
    }

    private void AddRecord(DictionaryRecord record)
    {
        switch (record.Dictionary)
        {
            case 1: { Users[record.Id] = record.Representation; break; };
            case 2: { Computers[record.Id] = record.Representation; break; };
            case 3: { Applications[record.Id] = record.Representation.CutQuotes(); break; }
            case 4: { Events[record.Id] = record.Representation.CutQuotes(); break; }
            case 5: { Metadata[record.Id] = record.Representation; break; }
            case 6: { Servers[record.Id] = record.Representation; break; }
        }
    }

    public void Read(string directory)
    {
        var dictionaryFileName = directory + fileName;

        var fileStream = File.OpenRead(dictionaryFileName);
        var fileReader = new StreamReader(fileStream);

        var readMachine = new OdinAssFileReader(fileReader);

        while (!readMachine.EOF())
        {
            var record = ReadDictionaryRecord(readMachine);
            if (record is not null)
            {
                AddRecord(record);
            }
        }
        fileReader.Close();
        fileStream.Close();
    }

    private DictionaryRecord? ReadDictionaryRecord(OdinAssFileReader readMachine)
    {
        if (!readMachine.ReadBegin()) return null;

        var record = new DictionaryRecord
        {
            Dictionary = uint.Parse(readMachine.ReadValue())
        };

        if (dictionaryType.Contains(record.Dictionary))
        {
            record.Uid = readMachine.ReadStructure();
        }
        record.Representation = readMachine.ReadStructure();
        record.Id = uint.Parse(readMachine.ReadValue());

        return record;
    }

}
