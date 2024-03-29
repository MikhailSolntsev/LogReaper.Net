﻿namespace LogReaper.Net.Service;

public static class Extensions
{
    public static string ToDateTimeString(this long source)
    {
        long tdatetime = source;
        long year = tdatetime / 10000000000;
        tdatetime %= 10000000000;
        long month = tdatetime / 100000000;
        tdatetime %= 100000000;
        long day = tdatetime / 1000000;
        tdatetime %= 1000000;
        long hour = tdatetime / 10000;
        tdatetime %= 10000;
        long min = tdatetime / 100;
        tdatetime %= 100;
        long sec = tdatetime;
        return $"{year:0000}-{month:00}-{day:00}T{hour:00}:{min:00}:{sec:00}";
    }

    public static string ToTransactionNumber(this string source)
    {
        if (source.Length < 14)
        {
            return "";
        }

        //  {243fc7779c570,335}
        //  Транзакция в формате записи из двух элементов преобразованных в шестнадцатеричное число
        //  – первый – число секунд с 01.01.0001 00:00:00 умноженное на 10000, второй – номер транзакции;
        string secondsString = source.Substring(1, 13);
        long secondsAll = Convert.ToInt64(secondsString, 16) / 10000;
        int delimeter = 86400; // 24 * 60 * 60
        int hours = (int)(secondsAll / delimeter);
        int seconds = (int)(secondsAll % delimeter);

        string numberString = source[15..^1];
        int number = Convert.ToInt32(numberString, 16);

        DateTime date = new(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        date += TimeSpan.FromDays(hours);
        date += TimeSpan.FromSeconds(seconds);

        string result = $"{date:dd.MM.yyyy HH:mm:ss} ({number})";

        return result;
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
