using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using LogReaper.Net.Service;

namespace LogReaper.Net.Tests.Small;

public class RecordConverterTests
{
    [Fact]
    public void ElcRecordToStringConvertsInOneLine()
    {
        // arrange
        ElasticRecord record = new()
        {
            Application = "ThinClient",
            Comment = "Ha-ha, classic",
            Computer = "My Computer"
        };

        ILocalLogger logger = new LocalLogger();
        IRepresentFieldsService representFieldsService = new RepresentFieldsService(logger);
        IFilterRecordsService filterRecordsService = new FilterRecordsService(logger, representFieldsService);

        ConvertRecordService recordConverter = new ConvertRecordService(filterRecordsService, representFieldsService);

        // act
        string result = recordConverter.ElasticRecordToJsonString(record);

        // assert
        result.Contains('\n').Should().BeFalse();
    }
}
