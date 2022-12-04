using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Service;

namespace LogReaper.Net
{
    public class RepresentFieldsService : IRepresentFieldsService
    {
        private readonly ILocalLogger logger;

        private OdinAssLogDictionary dictionary = null!;

        private RepresentationData representations = null!;

        public RepresentFieldsService(ILocalLogger logger)
        {
            this.logger = logger;
        }

        public void ReadRepresentationsFromDirectory(string directory)
        {
            string fullFileName = Path.Combine(directory, "representation.json");

            logger.LogInfo($"Чтение представлений из файла {fullFileName}");

            var stream = File.OpenRead(fullFileName);
            RepresentationData? representationData = ReadJsonService<RepresentationData>.Deserialize(stream);

            if (representationData is null)
            {
                throw new Exception($"Can't read represenations from file {fullFileName}");
            }

            representations = representationData;

            logger.LogInfo("Чтение представлений завершено");
        }

        public void UseDictionary(OdinAssLogDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public string RepresentMetadata(LogRecord logRecord)
        {
            string representation = "";
            if (logRecord.Metadata != 0)
            {
                representation = logRecord.Metadata.ToString();
                dictionary.Metadata.TryGetValue(logRecord.Metadata, out string? metadata);
                if (metadata is null)
                {
                    logger.LogDebug($"В словаре метадаты не найдено соответствие для {logRecord.Metadata}");
                }
                else
                {
                    representation = metadata;
                }
            }
            return representation;
        }

        public string RepresentLevel(LogRecord logRecord)
        {
            string representation = logRecord.Importance.ToString();
            representations.Levels.TryGetValue(logRecord.Importance, out string? importance);
            if (importance is null)
            {
                logger.LogDebug($"В соответствии представлений не найдено соответствие для уровня {logRecord.Importance}");
            }
            else
            {
                representation = importance;
            }
            return representation;
        }

        public string RepresentEvent(LogRecord logRecord)
        {
            string representation = logRecord.EventId.ToString();
            dictionary.Events.TryGetValue(logRecord.EventId, out string? eventId);
            if (eventId is null)
            {
                logger.LogDebug($"В словаре событий не найдено соответствие для {logRecord.EventId}");
            }
            else
            {
                representation = eventId;
                representations.Events.TryGetValue(eventId, out string? eventRepresentation);
                if (eventRepresentation is null)
                {
                    //logger.LogDebug($"В соответствии представлений не найдено соответствие для представления {eventId}");
                }
                else
                {
                    representation = eventRepresentation;
                }
            }
            return representation;
        }

        public string RepresentServer(LogRecord logRecord)
        {
            string representation = logRecord.Server.ToString();
            dictionary.Servers.TryGetValue(logRecord.Server, out string? server);
            if (server is null)
            {
                logger.LogDebug($"В словаре серверов не найдено соответствие для {logRecord.Server}");
            }
            else
            {
                representation = server;
            }
            return representation;
        }

        public string RepresentApplication(LogRecord logRecord)
        {
            string representation = logRecord.Application.ToString();
            dictionary.Applications.TryGetValue(logRecord.Application, out string? application);
            if (application is null)
            {
                logger.LogDebug($"В словаре приложений не найдено соответствие для {logRecord.Application}");
            }
            else
            {
                representation = application;
                representations.Applications.TryGetValue(application, out string? applicationRepresentation);
                if (applicationRepresentation is null)
                {
                    logger.LogDebug($"В соответствии представлений не найдено соответствие для приложения {application}");
                }
                else
                {
                    representation = applicationRepresentation;
                }
            }
            return representation;
        }

        public string RepresentComputer(LogRecord logRecord)
        {
            string representation = logRecord.Computer.ToString();
            dictionary.Computers.TryGetValue(logRecord.Computer, out string? computer);
            if (computer is null)
            {
                logger.LogDebug($"В словаре компьютеров не найдено соответствие для {logRecord.Computer}");
            }
            else
            {
                representation = computer;
            }
            return representation;
        }

        public string RepresentUser(LogRecord logRecord)
        {
            string representation = logRecord.User.ToString();

            dictionary.Users.TryGetValue(logRecord.User, out string? user);
            if (user is null)
            {
                logger.LogDebug($"В словаре пользоватлей не найдено соответствие для {logRecord.User}");
            }
            else
            {
                representation = user;
            }

            return representation;
        }

        public string RepresentTransaction(LogRecord logRecord)
        {
            string representation = logRecord.TransactionStatus;
            representations.TransactionStatuses.TryGetValue(logRecord.TransactionStatus, out string? transactionStatus);
            if (transactionStatus is null)
            {
                logger.LogDebug($"В соответствии представлений не найдено соответствие для статуса транзакций {logRecord.TransactionStatus}");
            }
            else
            {
                representation = transactionStatus;
            }

            return representation;
        }

        private class RepresentationData
        {
            public Dictionary<string, string> Events { get; set; } = null!;
            public Dictionary<string, string> Levels { get; set; } = null!;
            public Dictionary<string, string> Applications { get; set; } = null!;
            public Dictionary<string, string> TransactionStatuses { get; set; } = null!;
        }
    }
}
