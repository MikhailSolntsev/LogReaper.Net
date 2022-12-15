
using LogReaper.Net.Exceptions;

namespace LogReaper.Net.Service;

public static class FileHelper
{
    public const string logDirectory = "1Cv8Log";

    public static List<string> GetUnlockedFiles(string subdirectry)
    {
        var fullDirectoryName = Path.Combine(subdirectry, logDirectory);
        var files = Directory.GetFiles(fullDirectoryName, "*.lgp");

        var unlockedFiles = new List<string>();

        foreach (var file in files)
        {
            if (!IsFileLocked(file)) unlockedFiles.Add(file);
        }

        return unlockedFiles;
    }

    private static bool IsFileLocked(string file)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(file);
            using (FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            return true;
        }

        return false;
    }

    
}
