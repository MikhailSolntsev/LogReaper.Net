using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Exceptions;

namespace LogReaper.Net
{
    public class BackupProcessedFileService
    {
        private readonly ILocalLogger logger;
        private readonly LogReaperConfig config;

        public BackupProcessedFileService(ILocalLogger logger, LogReaperConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        public void BackupProcessedFile(string fileName, BaseListRecord baseRecord)
        {
            try
            {
                logger.LogInfo($"[{baseRecord.Name}] Перемещение файла \"{fileName}\"");
                var directory = Path.Combine(config.Files.BackupDirectory, baseRecord.Name);
                MoveFile(fileName, directory);
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

        private void MoveFile(string fileName, string directory)
        {
            var file = new FileInfo(fileName);
            var dir = new DirectoryInfo(directory);

            if (!dir.Exists)
            {
                dir.Create();
                if (!dir.Exists)
                {
                    throw new DirectoryCreationException($"Не удалось создать каталог \"{directory}\"");
                }
            }

            string newFileName = Path.Combine(directory, file.Name);
            file.MoveTo(newFileName);

            var dest = new FileInfo(newFileName);
            if (!dest.Exists)
            {
                throw new FileRenameException($"Ошибка перемещения файла в \"{newFileName}\"");
            }
        }

    }
}
