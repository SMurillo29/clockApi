using System;

namespace ClockApi.Common.Model
{
    internal class Record
    {
        public int Id { get; set; }

        public DateTime DateTimeRecord { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
