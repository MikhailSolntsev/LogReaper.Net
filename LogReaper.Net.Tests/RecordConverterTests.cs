
using LogReaper.Net.Models;
using LogReaper.Net;

namespace LogReaper.Net.Tests;

public class RecordConverterTests
{
    [Fact]
    public void ElcRecordToStringConvertsInOneLine()
    {
        ElkRecord record = new()
        {
            Application = "ThinClient",
            Comment = "Ha-ha, classic",
            Computer = "My Computer"
        };

        RecordConverter recordConverter = new RecordConverter();
        string result = recordConverter.ElkRecordToJsonString(record);

        result.FirstOrDefault(c => c == '\n', '-').Should().Be('-');
    }
}
