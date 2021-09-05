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

        [FunctionName(nameof(UpdateRecord))]
        public static async Task<IActionResult> UpdateRecord(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "record/{id}")] HttpRequest req,
        [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"Update for Record: {id}, recived.");

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

            //validate record id.
            TableOperation findOperation = TableOperation.Retrieve<RecordEntity>("REC", id);
            TableResult findResult = await recordTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Record not found."
                });
            }
            /// update record
            RecordEntity recordEntity = (RecordEntity)findResult.Result;
            recordEntity.Type = record.Type;
            if (record.Id != 0)
            {
                recordEntity.Id = record.Id;
            }
            TableOperation updateOperation = TableOperation.Replace(recordEntity);
            await recordTable.ExecuteAsync(updateOperation);

            string message = $"Record: {id}, updated in table.";
            log.LogInformation(message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordEntity

            });
        }

        [FunctionName(nameof(GetAllRecords))]
        public static async Task<IActionResult> GetAllRecords(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "record")] HttpRequest req,
        [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
        ILogger log)
        {
            log.LogInformation("Get all Records Recived.");

            TableQuery<RecordEntity> query = new TableQuery<RecordEntity>();
            TableQuerySegment<RecordEntity> records = await recordTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all Records";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = records

            });
        }

        [FunctionName(nameof(GetRecordById))]
        public static IActionResult GetRecordById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "record/{id}")] HttpRequest req,
        [Table("record", "REC", "{id}", Connection = "AzureWebJobsStorage")] RecordEntity recordEntity,
        string id,
        ILogger log)
        {
            log.LogInformation($"Get record by id: {id} Recived.");


            if (recordEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Record not found."
                });
            }

            string message = $"Record {id} retived";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordEntity

            });
        }

        [FunctionName(nameof(DeleteRecord))]
        public static async Task<IActionResult> DeleteRecord(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "record/{id}")] HttpRequest req,
        [Table("record", "REC", "{id}", Connection = "AzureWebJobsStorage")] RecordEntity recordEntity,
        [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"Delete Record: {id} Recived.");


            if (recordEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Record not found."
                });
            }

            await recordTable.ExecuteAsync(TableOperation.Delete(recordEntity));
            string message = $"Record {id} deleted";
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
