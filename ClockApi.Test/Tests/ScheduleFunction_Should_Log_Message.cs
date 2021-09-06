using ClockApi.Functions.Functions;
using ClockApi.Test.Helpers;
using System;
using Xunit;

namespace ClockApi.Test.Tests
{
    public class ScheduleFunction_Should_Log_Message
    {
        [Fact]
        public void ScheduleFunctionTest()
        {
            //Arrang 
            MockCloudTableRecords mockTable = new MockCloudTableRecords(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockTable2 = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act
            ConsolidatedApi.Run(null, mockTable, mockTable2, logger);
            string message = logger.Logs[0];

            //Assert

            Assert.Contains("Consolidated completed function", message);
        }
    }
}
