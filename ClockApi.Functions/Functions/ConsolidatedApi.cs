using ClockApi.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ClockApi.Functions.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName("ConsolidatedApi")]
        public static async Task Run(
            [TimerTrigger("* * * * *")] TimerInfo myTimer,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation($"Consolidated completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal, false);
            TableQuery<RecordEntity> query = new TableQuery<RecordEntity>().Where(filter);
            TableQuerySegment<RecordEntity> records = await recordTable.ExecuteQuerySegmentedAsync(query, null);
            List<RecordEntity> orderListRecords = records.Results.OrderBy(x => x.Id).ThenBy(x => x.DateTimeRecord).ToList();

            List<ConsolidatedEntity> consolidate = null;

            RecordEntity lastRecord = null;
            foreach (RecordEntity record in orderListRecords)
            {
                if (lastRecord == null)
                {
                    lastRecord = record;
                }
                else if ((lastRecord.Id == record.Id) && (lastRecord.Type != record.Type))
                {
                    int hoursWorked = TimeBetweenDates(lastRecord.DateTimeRecord, record.DateTimeRecord);

                    string filter2 = TableQuery.GenerateFilterConditionForInt("ID", QueryComparisons.Equal, record.Id);
                    TableQuery<ConsolidatedEntity> queryConsilidates = new TableQuery<ConsolidatedEntity>().Where(filter2);
                    TableQuerySegment<ConsolidatedEntity> querys = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsilidates, null);
                    consolidate = querys.Results.Where(x => x.Date.Date == DateTime.UtcNow.Date).ToList();

                    if (consolidate.Count() == 0)
                    {
                        ConsolidatedEntity consolidateEntity = new ConsolidatedEntity
                        {
                            Date = System.DateTime.UtcNow,
                            ID = record.Id,
                            Minutes = hoursWorked,
                            ETag = "*",
                            PartitionKey = "CON",
                            RowKey = System.Guid.NewGuid().ToString(),

                        };
                        TableOperation addOperation = TableOperation.Insert(consolidateEntity);
                        await consolidatedTable.ExecuteAsync(addOperation);

                        //actualizar registro de salida
                        record.IsConsolidated = true;
                        TableOperation updateOperation = TableOperation.Replace(record);
                        await recordTable.ExecuteAsync(updateOperation);

                        //actualizar registro de entrada
                        lastRecord.IsConsolidated = true;
                        TableOperation updateOperation2 = TableOperation.Replace(lastRecord);
                        await recordTable.ExecuteAsync(updateOperation2);

                    }
                    else
                    {
                        ConsolidatedEntity con = consolidate[0];
                        con.Minutes += hoursWorked;
                        TableOperation updateOperation = TableOperation.Replace(con);
                        await consolidatedTable.ExecuteAsync(updateOperation);

                        //actualizar registro de salida
                        record.IsConsolidated = true;
                        TableOperation updateOperation2 = TableOperation.Replace(record);
                        await recordTable.ExecuteAsync(updateOperation2);

                        //actualizar registro de entrada
                        lastRecord.IsConsolidated = true;
                        TableOperation updateOperation3 = TableOperation.Replace(lastRecord);
                        await recordTable.ExecuteAsync(updateOperation3);

                    }


                    lastRecord = null;
                }
                else
                {
                    lastRecord = null;
                }

            }

            log.LogInformation("Consolidated process finish");
        }

        public static int TimeBetweenDates(DateTime initialDate, DateTime finaldate)
        {
            TimeSpan diference = finaldate - initialDate;
            return (int)diference.TotalMinutes;
        }
    }
}
