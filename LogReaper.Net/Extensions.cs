using LogReaper.Net.Models;

namespace LogReaper.Net;

public static class Extensions
{
    public static ElkRecord ToElkRecord(this LogRecord record, LogDictionary dictionary) =>
        new()
        {
            Datetime = record.Datetime.ToDateTimeString(),
            User = dictionary.Users[record.User] ?? "\"\"",
            Computer = dictionary.Computers[record.Computer] ?? "\"\"",
            Server = dictionary.Servers[record.Server] ?? "\"\"",
            Application = dictionary.Applications[record.Application] ?? "\"\"",
            Event = dictionary.Events[record.EventId] ?? "\"\"",
            Importance = record.Importance,
            Comment = record.Comment,
            Metadata = dictionary.Metadata[record.Metadata] ?? "\"\"",
            Representation = record.Representation,
            Connection = record.Connection
        };

    public static string ToDateTimeString(this long source)
    {
        long tdatetime = source;
        long year = tdatetime / 10000000000;
        tdatetime %= 10000000000;
        long month = tdatetime / 100000000;
        tdatetime %= 100000000;
        long day = tdatetime / 1000000;
        tdatetime %= 1000000;
        long hour = tdatetime / 1000000;
        tdatetime %= 10000;
        long min = tdatetime / 1000000;
        tdatetime %= 100;
        long sec = tdatetime;
        return $"{year:0000}-{month:00}-{day:00}T{hour:00}:{min:00}:{sec:00}";
    }

    public static string ToPeriod(this string source) =>
        source[..Math.Min(10, source.Length)].Replace('-', '.');

    public static string CutQuotes(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return source;
        }

        var result = source;

        if (result.StartsWith('"'))
        {
            result = source[1..];
        }

        if (result.EndsWith('"'))
        {
            result = result[..^1];
        }

        return result;
    }
}
