using ClockApi.Common.Model;
using ClockApi.Common.Responses;
using ClockApi.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace ClockApi.Functions.Functions
{
    public static class RecordApi
    {

        [FunctionName(nameof(CreateRecord))]
        public static async Task<IActionResult> CreateRecord(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "record")] HttpRequest req,
        [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
        ILogger log)
        {
            log.LogInformation("Recived a new Record");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Record record = JsonConvert.DeserializeObject<Record>(requestBody);


            if (record.Id == 0)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request body must have a employed ID"
                });
            }

            if (record.Type != 1 && record.Type != 0)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "the record type can only be 1 or 0"
                });
            }

            RecordEntity recordEntity = new RecordEntity
            {
                DateTimeRecord = System.DateTime.UtcNow,
                ETag = "*",
                IsConsolidated = false,
                PartitionKey = "REC",
                RowKey = System.Guid.NewGuid().ToString(),
                Id = record.Id,
                Type = record.Type


            };

            TableOperation addOperation = TableOperation.Insert(recordEntity);
            await recordTable.ExecuteAsync(addOperation);

            string message = "New Record stored in table";
            log.LogInformation(message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordEntity

            });
        }

    }
}
