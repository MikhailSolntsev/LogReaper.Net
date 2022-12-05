
using LogReaper.Net.Contracts;
using LogReaper.Net.Dto;
using Moq;
using System.Collections.Generic;

namespace LogReaper.Net.Tests.Mock
{
    public static class MockExtensions
    {
        public static MockDataAccessor<ElasticRecord> SetupForElastic(this Mock<ISendElasticMessageService> mock)
        {
            var accessor = new MockDataAccessor<ElasticRecord>();

            mock.Reset();
            mock.Setup(s => s.SendBulkToStorageAsync(It.IsAny<List<ElasticRecord>>()))
                .Callback<List<ElasticRecord>>(messages =>
                {
                    foreach (var message in messages)
                    {
                        accessor.Enqueue(message);
                    }
                });

            return accessor;
        }
    }
}
