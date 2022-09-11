
using System.Text.Json;

namespace LogReaper.Net.Service;

internal static class JsonReader<T>
{

    public static T? Deserialize(Stream stream)
    {
        JsonSerializerOptions options = SerializerOptions();

        T? result = JsonSerializer.Deserialize<T>(stream, options);
        return result;
    }

    private static JsonSerializerOptions SerializerOptions()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return options;
    }

}
