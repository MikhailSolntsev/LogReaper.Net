using LogReaper.Net.Service;

namespace LogReaper.Net.Tests
{
    public partial class FileHelperTests
	{
		[Fact(DisplayName = "If one file is open even for read, it counts as locked")]
		public void GetCorrectUnlockedFiles()
		{
			using var lockedFile = File.CreateText(fileName2);

			var files = FileHelper.GetUnlockedFiles(subdirectory);

			files.Should().HaveCount(2);
			files[0].Should().BeEquivalentTo(fileName1);
			files[1].Should().BeEquivalentTo(fileName3);
		}
	}
}
