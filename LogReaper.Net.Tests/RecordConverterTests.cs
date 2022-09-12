
using LogReaper.Net.Models;
using LogReaper.Net;
using LogReaper.Net.Service;

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

        ILocalLogger logger = new LocalLogger();

        RecordConverter recordConverter = new RecordConverter(logger);

        string result = recordConverter.ElkRecordToJsonString(record);

        result.FirstOrDefault(c => c == '\n', '-').Should().Be('-');
    }
}
