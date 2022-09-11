
namespace LogReaper.Net.Service;

public interface ILocalLogger
{
    void LogError(string? message);
    void LogError(string message, string database);
    void LogInfo(string? message);
    void LogInfo(string message, string database);
    void LogDebug(string? message);
    void LogDebug(string message, string database);
}