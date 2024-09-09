using System;

namespace UF5423_Aguas.Data.Entities
{
    public class Consumption
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public Meter Meter { get; set; }
    }
}
