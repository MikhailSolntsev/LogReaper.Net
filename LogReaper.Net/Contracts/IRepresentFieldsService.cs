using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts
{
    public interface IRepresentFieldsService
    {
        void ReadRepresentationsFromDirectory(string directory);

        void UseDictionary(OdinAssLogDictionary dictionary);

        string RepresentTransaction(LogRecord logRecord);

        string RepresentUser(LogRecord logRecord);

        string RepresentComputer(LogRecord logRecord);

        string RepresentApplication(LogRecord logRecord);

        string RepresentServer(LogRecord logRecord);

        string RepresentEvent(LogRecord logRecord);

        string RepresentLevel(LogRecord logRecord);

        string RepresentMetadata(LogRecord logRecord);
    }
}