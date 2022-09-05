
using System.IO;
using System;
using static System.Net.WebRequestMethods;
using System.Net.Sockets;

namespace LogReaper.Net;

public static class FileHelper
{
    public const string logDirectory = "1Cv8Log\\";

    public static List<string> GetUnlockedFiles(string subdirectry)
    {
        var fullDirectoryName = Path.Combine(subdirectry, logDirectory);
        var files = Directory.GetFiles(fullDirectoryName, "*.lgp");

        var unlockedFiles = new List<string>();

        foreach (var file in files)
        {
            if (!IsFileLocked(file)) unlockedFiles.Add(file)
                    ;
        }
        //files.Select(file => { ; return file; });

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

    public static void MoveFile(string fileName, string directory)
    {
        var file = new FileInfo(fileName);
        var dir = new DirectoryInfo(directory);

        if (!dir.Exists)
        {
            dir.Create();
            if (!dir.Exists)
            {
                throw new FileNotFoundException($"Не удалось создать каталог \"{directory}\"");
            }
        }

        string newFileName = $"{directory}{file.Name}";
        file.MoveTo(newFileName);

        var dest = new FileInfo(newFileName);
        if (!dest.Exists)
        {
            throw new FileNotFoundException($"Ошибка перемещения файла в \"{newFileName}\"");
        }
    }
}
