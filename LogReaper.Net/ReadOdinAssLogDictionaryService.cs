using LogReaper.Net.Dto;
using LogReaper.Net.Enums;
using LogReaper.Net.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net
{
    public class ReadOdinAssLogDictionaryService
    {
        private const string fileName = "1Cv8Log\\1Cv8.lgf";

        public OdinAssLogDictionary ReadOdinAssDictionaryInDirectory(string directory)
        {
            var dictionaryFileName = Path.Combine(directory, fileName);

            var fileStream = File.OpenRead(dictionaryFileName);
            var fileReader = new StreamReader(fileStream);

            var readMachine = new ReadAnyOdinAssFileService(fileReader);

            OdinAssLogDictionary dictionary = new();

            while (!readMachine.EOF())
            {
                var record = ReadDictionaryRecord(readMachine);
                if (record is not null)
                {
                    AddRecord(dictionary, record);
                }
            }
            fileReader.Close();
            fileStream.Close();

            return dictionary;
        }

        private DictionaryRecord? ReadDictionaryRecord(ReadAnyOdinAssFileService readMachine)
        {
            if (!readMachine.ReadBegin()) return null;

            var record = new DictionaryRecord
            {
                Dictionary = (OdinAssLogDictionaryType)int.Parse(readMachine.ReadValue())
            };

            OdinAssLogDictionaryType dictionaryType = (OdinAssLogDictionaryType)record.Dictionary;
            //bool storeValue = Enum.TryParse(typeof(OdinAssLogDictionaryType), record.Dictionary, out OdinAssLogDictionaryType dictionary);

            if (dictionaryType != default)
            {
                record.Uid = readMachine.ReadStructure();
            }
            record.Representation = readMachine.ReadStructure();
            record.Id = uint.Parse(readMachine.ReadValue());

            return record;
        }

        private void AddRecord(OdinAssLogDictionary dictionary, DictionaryRecord record)
        {
            switch (record.Dictionary)
            {
                case OdinAssLogDictionaryType.Users: { dictionary.Users[record.Id] = record.Representation; break; };
                case OdinAssLogDictionaryType.Computers: { dictionary.Computers[record.Id] = record.Representation; break; };
                case OdinAssLogDictionaryType.Applications: { dictionary.Applications[record.Id] = record.Representation.CutQuotes(); break; }
                case OdinAssLogDictionaryType.Events: { dictionary.Events[record.Id] = record.Representation.CutQuotes(); break; }
                case OdinAssLogDictionaryType.Metadata: { dictionary.Metadata[record.Id] = record.Representation; break; }
                case OdinAssLogDictionaryType.Servers: { dictionary.Servers[record.Id] = record.Representation; break; }
            }
        }

    }
}
