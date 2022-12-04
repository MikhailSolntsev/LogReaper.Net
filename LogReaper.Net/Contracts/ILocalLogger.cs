namespace LogReaper.Net.Contracts;

public interface ILocalLogger
{
    void LogError(string? message);
    void LogInfo(string? message);
    void LogDebug(string? message);
}