
namespace LogReaper.Net.Tests;

public class ExtensionTests
{
    [Theory(DisplayName ="String to period should truncate string and change dash to dot")]
    [InlineData("2022-09-04T12:34:11", "2022.09.04")]
    [InlineData("2022", "2022")]
    [InlineData("2022:09:04", "2022:09:04")]
    public void StringToPeriodShouldReturn10charStringWithDots(string source, string expected)
    {
        string result = source.ToPeriod();

        result.Should().Be(expected);
    }
}