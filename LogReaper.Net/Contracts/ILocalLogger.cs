using LogReaper.Net.Dto;

namespace LogReaper.Net.Contracts;

public interface ILocalLogger
{
    void UseCurrentBase(BaseListRecord baseRecord);
    void LogError(string? message);
    void LogInfo(string? message);
    void LogDebug(string? message);
    void LogBaseError(string? message);
    void LogBaseInfo(string? message);
    void LogBaseDebug(string? message);
}