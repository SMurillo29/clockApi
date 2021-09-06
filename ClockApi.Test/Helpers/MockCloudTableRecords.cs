using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace ClockApi.Test.Helpers
{
    public class MockCloudTableRecords : CloudTable
    {
        public MockCloudTableRecords(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableRecords(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableRecords(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {

        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetRecordEntity()
            });
        }
    }
}
