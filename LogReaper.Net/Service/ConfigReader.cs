
using LogReaper.Net.Models;

namespace LogReaper.Net.Service;

internal class ConfigReader
{
    public static Configuration ReadConfig(string directory, ILocalLogger logger)
    {
        string fileName = Path.Combine(directory, "config.json");

        logger.LogInfo($"Чтение конфигурации из файла {fileName}");

        var stream = File.OpenRead(fileName);

        Configuration? config = JsonReader<Configuration>.Deserialize(stream);

        if (config == null)
        {
            throw new Exception("Can't read configuration file config.json");
        }

        logger.LogInfo("Чтение конфигурации завершено");

        return config;
    }

}
