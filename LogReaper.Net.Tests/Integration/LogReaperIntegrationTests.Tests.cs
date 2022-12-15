using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogReaper.Net.Tests.Integration
{
    public partial class LogReaperIntegrationTests
    {
        [Fact(DisplayName ="Не ёбнет? Да не, не должно")]
        async Task ReadDictionaryShouldReadDictionary()
        {
            // arrange

            // act
            await logProcessor.ProcessLogsAsync();

            // assert

        }
    }
}
