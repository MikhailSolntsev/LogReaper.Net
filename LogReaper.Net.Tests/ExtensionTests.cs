
namespace LogReaper.Net.Tests;

public class ExtensionTests
{
    [Theory(DisplayName ="String to period should truncate string and change dash to dot")]
    [InlineData("2022-09-04T12:34:11", "2022.09.04")]
    [InlineData("2022", "2022")]
    [InlineData("2022:09:04", "2022:09:04")]
    public void StringToPeriodShouldReturn10charStringWithDots(string source, string expects)
    {
        source.ToPeriod().Should().Be(expects);
    }

    [Theory(DisplayName = "Transaction number should convert inte dane and number")]
    [InlineData("{2443dafa807e0,3d}", "07.09.2022 17:26:54 (61)")]
    [InlineData("{2443dafa9b590,24b}", "07.09.2022 17:27:05 (587)")]
    public void TransactionDateConvertionTest(string source, string expects)
    {
        source.ToTransactionNumber().Should().Be(expects);
    }
}