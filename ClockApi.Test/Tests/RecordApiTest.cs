using ClockApi.Functions.Functions;
using ClockApi.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace ClockApi.Test.Tests
{
    public class RecordApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();
        [Fact]
        public async void CreateRecord_Should_Return_200()
        {
            //Arrenge
            MockCloudTableRecords mockTodos = new MockCloudTableRecords(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Model.Record recordRequest = TestFactory.GetRecordRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordRequest);

            //Act
            IActionResult response = await RecordApi.CreateRecord(request, mockTodos, logger);
            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void UpdateRecord_Should_Return_200()
        {
            //Arrenge
            MockCloudTableRecords mockRecords = new MockCloudTableRecords(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Model.Record record = TestFactory.GetRecordRequest();
            Guid recordId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordId, record);

            //Act
            IActionResult response = await RecordApi.UpdateRecord(request, mockRecords, recordId.ToString(), logger);
            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }





    }
}