
using LogReaper.Net.Service;
namespace LogReaper.Net.Tests;

public partial class FileHelperTests : IAsyncLifetime
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
    }

    public Task InitializeAsync()
    {
        CreateFiles();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Directory.Delete(subdirectory, true);
        return Task.CompletedTask;
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
