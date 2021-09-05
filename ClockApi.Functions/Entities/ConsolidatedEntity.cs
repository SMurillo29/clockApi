using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ClockApi.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }

    }
}
