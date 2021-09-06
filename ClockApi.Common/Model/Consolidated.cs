using System;

namespace ClockApi.Common.Model
{
    public class Consolidated
    {
        // ID Empleo(int)       
        // Fecha(date time)
        // Minutos trabajados(int)

        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }

    }
}
