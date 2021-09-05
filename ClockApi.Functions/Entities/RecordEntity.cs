using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ClockApi.Functions.Entities
{
    public class RecordEntity : TableEntity
    {
        public int Id { get; set; }

        public DateTime DateTimeRecord { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
