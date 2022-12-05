using LogReaper.Net.Dto;

namespace LogReaper.Net
{
    public interface IBackupProcessedFileService
    {
        void BackupProcessedFile(string fileName, BaseListRecord baseRecord);
    }
}