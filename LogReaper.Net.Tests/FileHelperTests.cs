
using LogReaper.Net.Service;

namespace LogReaper.Net.Tests;

public class FileHelperTests : IDisposable
{
    private readonly string fileName1;
    private readonly string fileName2;
    private readonly string fileName3;
    private readonly string subdirectory;

    public FileHelperTests()
    {
        subdirectory = CreateSubdirectory();

        fileName1 = Path.Combine(subdirectory, FileHelper.logDirectory, "file1.lgp");
        fileName2 = Path.Combine(subdirectory, FileHelper.logDirectory, "file2.lgp");
        fileName3 = Path.Combine(subdirectory, FileHelper.logDirectory, "file3.lgp");

        CreateFiles();
    }

    public void Dispose()
    {
        Directory.Delete(subdirectory, true);
    }

    [Fact(DisplayName = "If one file is open even for read, it counts as locked")]
    public void GetCorrectUnlockedFiles()
    {
        using var lockedFile = File.CreateText(fileName2);

        var files = FileHelper.GetUnlockedFiles(subdirectory);

        files.Should().HaveCount(2);
        files[0].Should().BeEquivalentTo(fileName1);
        files[1].Should().BeEquivalentTo(fileName3);
    }

    private string CreateSubdirectory()
    {
        string name = Path.GetRandomFileName();
        Directory.CreateDirectory(name);
        string subdirectory = Path.Combine(name, FileHelper.logDirectory);
        Directory.CreateDirectory(subdirectory);

        return name;
    }

    private void CreateFiles()
    {
        var file = File.CreateText(fileName1);
        file.Write("text 1");
        file.Close();

        file = File.CreateText(fileName2);
        file.Write("text 2");
        file.Close();

        file = File.CreateText(fileName3);
        file.Write("text 3");
        file.Close();
    }

}
