
using System.Text;

namespace LogReaper.Net;

public class OdinAssFileReader
{
    private readonly TextReader reader;
    private string? currentString = null;

    public OdinAssFileReader(TextReader reader)
    {
        this.reader = reader;

        ReadNewLine();
    }

    private bool AddNextString()
    {
        var nextString = reader.ReadLine();
        if (nextString is null) return false;

        currentString += nextString;

        return true;
    }

    private void ReadNewLine()
    {
        currentString = reader.ReadLine();
    }

    private int FindFirst(char symbol)
    {
        if (currentString is null)
        {
            throw new NullReferenceException("Current string is null!!!!");
        }

        return currentString.IndexOf(symbol);
    }

    private void CutAfter(int index)
    {
        if (currentString is null)
        {
            throw new NullReferenceException("Current string is null!!!!");
        }
        currentString = currentString.Substring(startIndex: index + 1);
    }

    private string SubstringBefore(int index)
    {
        if (index < 1) return "";
        
        return currentString!.Substring(0, index);
    }

    private int FindMinimumChar(char c1, char c2)
    {
        var index1 = FindFirst(c1);
        var index2 = FindFirst(c2);
        return FindMinimumEOG(index1, index2);
    }

    private int FindMinimumEOG(int value1, int value2, int value3 = -1, int value4 = -1)
    {
        var result = Math.Max(Math.Max(value1, value2), Math.Max(value3, value4));

        if (result < 0)
        {
            return -1;
        }

        int[] range = new[] { value1, value2, value3, value4 };
        result = range.Where(v => v >= 0).Min();
        //var range = Enumerable.Range(0, result).Min;

        //if (range.Contains(value1)) result = value1;
        //if (range.Contains(value2)) result = value2;
        //if (range.Contains(value3)) result = value3;
        //if (range.Contains(value4)) result = value4;

        return result;
    }

    public bool EOF() => currentString is null;

    public bool ReadBegin()
    {
        while (!EOF())
        {
            var positionStart = FindFirst('{');
            if (positionStart >= 0)
            {
                CutAfter(positionStart);
                return true;
            }
            ReadNewLine();
        }

        return false;
    }

    public string ReadValue()
    {
        var builder = new StringBuilder();

        var minimum = FindMinimumChar(',', '}');

        while (!EOF() && minimum< 0) {
            builder.Append(currentString);
            ReadNewLine();
            minimum = FindMinimumChar(',', '}');
        }

        builder.Append(SubstringBefore(minimum));

        CutAfter(minimum);

        return builder.ToString();
    }

    public string ReadStructure()
    {
        var builder = new StringBuilder();

        var level = 0;
        var isText = false;
        var stopSearch = false;

        while (!stopSearch)
        {
            if (EOF())
            {
                break;
            }

            int bracketStart = FindFirst('{');
            int bracketEnd = FindFirst('}');
            int comma = FindFirst(',');
            int quote = FindFirst('"');

            int minimum = FindMinimumEOG(bracketStart, bracketEnd, comma, quote);

            if (minimum < 0)
            {
                if (!AddNextString())
                {
                    builder.Append(currentString);
                }
                continue;
            }

            var substring = SubstringBefore(minimum);
            var breakStuff = false;

            if (minimum == bracketStart)
            {
                builder.Append(substring).Append('{');
                if (!isText) level += 1;
            }
            else if (minimum == bracketEnd)
            {
                if (isText)
                {
                    builder.Append(substring).Append('}');
                }
                else if (level > 0)
                {
                    builder.Append(substring).Append('}');
                    level -= 1;
                }
                else
                    breakStuff = true;
            }
            else if (minimum == quote)
            {
                isText = !isText;
                builder.Append(substring).Append('"');
            }
            else if (minimum == comma)
            {
                if (isText || level > 0)
                    builder.Append(substring).Append(',');
                else
                {
                    builder.Append(substring);
                    stopSearch = true;
                }
            }

            if (breakStuff) break;
            CutAfter(minimum);
        }

        return builder.ToString();
    }

}
