
using LogReaper.Net.Models;

namespace LogReaper.Net.Service;

internal class ConfigReader
{
    public static Configuration ReadConfig(string directory)
    {
        string fileName = Path.Combine(directory, "config.json");

        var stream = File.OpenRead(fileName);

        Configuration? config = JsonReader<Configuration>.Deserialize(stream);

        if (config == null)
        {
            throw new Exception("Can't read configuration file config.json");
        }

        return config;
    }

}
