
namespace LogReaper.Net.Tests;

public class OdinAssFileReaderTests
{
    [Theory(DisplayName ="EOF should be true with empty string and false if not")]
    [InlineData("", true)]
    [InlineData("true", false)]
    public void ReaderEofTrueOnEmptyString(string input, bool expected)
    {
        // arrange
        TextReader textReader = new StringReader(input);
        ReadAnyOdinAssFileService reader = new(textReader);

        // act
        var result = reader.EOF();

        // assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Readed value is one before comma or }")]
    [InlineData("{0}", "0")]
    [InlineData("{1, 2}", "1")]
    public void ReadingValueTest(string input, string expected)
    {
        // arrange
        TextReader textReader = new StringReader(input);
        ReadAnyOdinAssFileService reader = new(textReader);
        reader.ReadBegin();

        // act
        var result = reader.ReadValue();
        
        // assert
        result.Should().Be(expected);
    }

    [Theory(DisplayName = "Readed structure is one before comma or } in single line")]
    [InlineData("{0,{1}}", "{1}")]
    [InlineData("{2,{3,4}}", "{3,4}")]
    [InlineData("{2,{3,4},12}","{3,4}")]
    [InlineData("{0,{1,2,\"1541\",{1,{\"DEV\",1541}},0},{269}}", "{1,2,\"1541\",{1,{\"DEV\",1541}},0}")]
    public void ReadingStructureWitnOneLineTest(string input, string expected)
    {
        // arrrange
        TextReader textReader = new StringReader(input);
        ReadAnyOdinAssFileService reader = new(textReader);
        reader.ReadBegin();
        reader.ReadValue();

        // act
        var result = reader.ReadStructure();
            
        // assert
        result.Should().Be(expected);
    }
    
    [Fact(DisplayName = "Reading structure with many lines")]
    public void ReadingStructureWithMultipleLinesTest()
    {
        // arrange
        string input = "{0,\n\r{e097995f-0229-4d25-81c0-59b23e421d3f,\"1541\",1541,\"DEV-1CSRV-01\",0,0,86400,60,0,0,0,\n\r" +
            "{1,\n\r{\"DEV-1CSRV-01\",1541}\n\r},0,0,1,0},\n\r{269}}\n\r";
        string expected = "{e097995f-0229-4d25-81c0-59b23e421d3f,\"1541\",1541,\"DEV-1CSRV-01\",0,0,86400,60,0,0,0," +
            "{1,{\"DEV-1CSRV-01\",1541}},0,0,1,0}";
        TextReader textReader = new StringReader(input);
        ReadAnyOdinAssFileService reader = new(textReader);
        reader.ReadBegin();
        reader.ReadValue();

        // act
        var result = reader.ReadStructure();

        // assert
        result.Should().Be(expected);
    }
}
