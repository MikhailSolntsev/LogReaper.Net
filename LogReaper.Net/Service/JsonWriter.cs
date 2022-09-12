
using System.Text.Json;

namespace LogReaper.Net.Service;

internal static class JsonWriter<T>
{

    public static string Serialize(T source)
    {
        JsonSerializerOptions options = SerializerOptions();

        string result = JsonSerializer.Serialize(source, options);

        return result;
    }

    private static JsonSerializerOptions SerializerOptions()
    {

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return options;
    }

}
