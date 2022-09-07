using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogReaper.Net;

public class LogConfig
{
    public IList<string> Bases { get; set; }
    public string LogDirectory { get; set; }
    public string BackupDirectory { get; set; }
    public string ElkUrl { get; set; }
    public int BulkSize { get; set; }

    public static LogConfig ReadConfig(string directory)
    {
        string fileName = Path.Combine(directory, "config.json");
        
        var stream = File.OpenRead(fileName);

        JsonSerializerOptions options = SerializerOptions();
        LogConfig config = JsonSerializer.Deserialize<LogConfig>(stream, options);

        return config;
    }

    private static JsonSerializerOptions SerializerOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return options;
    }
}
