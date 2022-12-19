using LogReaper.Net.Configuration;
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Exceptions;

namespace LogReaper.Net
{
    public class BackupProcessedFileService : IBackupProcessedFileService
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
            string directory = string.Empty;
            try
            {
                logger.LogInfo($"[{baseRecord.Name}] Перемещение файла \"{fileName}\"");
                directory = Path.Combine(config.Files.BackupDirectory, baseRecord.Name);
                MoveFile(fileName, directory, baseRecord);
            }
            catch (DirectoryCreationException e)
            {
                logger.LogError($"[{baseRecord.Name}] Ошибка создания каталога \"{directory}\": {e.Message}");
            }
            catch (FileRenameException e)
            {
                logger.LogError($"[{baseRecord.Name}] Ошибка переименования файла \"{fileName}\" в каталог \"{directory}\": {e.Message}");
            }
            catch (Exception e)
            {
                logger.LogError($"[{baseRecord.Name}] Ошибка перемещения файла \"{fileName}\" в каталог \"{directory}\": {e.Message}");
            }
        }

        private void MoveFile(string fileName, string directory, BaseListRecord baseRecord)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            { 
                return;
            }

            var directoryInfo = new DirectoryInfo(directory);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                if (!directoryInfo.Exists)
                {
                    throw new DirectoryCreationException($"Не удалось создать каталог \"{directory}\"");
                }
            }

            string newFileName = Path.Combine(directory, fileInfo.Name);
            try
            {
                fileInfo.MoveTo(newFileName);
            }
            catch(Exception e)
            {
                logger.LogError($"[{baseRecord.Name}] Ошибка переименования файла \"{fileName}\" в каталог \"{newFileName}\": {e.Message}");
                throw;
            }

            var destinationFileInfo = new FileInfo(newFileName);
            if (!destinationFileInfo.Exists)
            {
                throw new FileRenameException($"Ошибка перемещения файла \"{fileName}\" в \"{newFileName}\"");
            }
        }

    }
}
