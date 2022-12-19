using LogReaper.Net.Service;

namespace LogReaper.Net.Tests.Small;

public class ExtensionTests
{
    [Theory(DisplayName = "StringToPeriod должна обрезать лишнее в строке и заменять тире на точки")]
    [InlineData("2022-09-04T12:34:11", "2022.09.04")]
    [InlineData("2022", "2022")]
    [InlineData("2022:09:04", "2022:09:04")]
    public void StringToPeriodShouldReturn10charStringWithDots(string source, string expects)
    {
        source.ToPeriod().Should().Be(expects);
    }

    [Theory(DisplayName = "StringToTransactionNumber должна конвертировать в строку с датой/временем и номером транзакции")]
    [InlineData("{2443dafa807e0,3d}", "07.09.2022 17:26:54 (61)")]
    [InlineData("{2443dafa9b590,24b}", "07.09.2022 17:27:05 (587)")]
    public void TransactionDateConvertionTest(string source, string expects)
    {
        source.ToTransactionNumber().Should().Be(expects);
    }

    [Theory(DisplayName = "")]
    [InlineData(20221219160942, "2022-12-19T16:09:42")]
    [InlineData(20221120030942, "2022-11-20T03:09:42")]
    public void LongToDateTimeString_Should_Return_Correct_DateTime(long source, string expects)
    {
        // act
        var result = source.ToDateTimeString();

        // assert
        result.Should().Be(expects);
    }
}