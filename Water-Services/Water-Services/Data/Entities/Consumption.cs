using System;
using System.ComponentModel.DataAnnotations;

namespace Water_Services.Data.Entities
{
    public class Consumption : IEntity
    {
        public Consumption()
        {
            Date = DateTime.Now;
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public int Volume { get; set; }

        public string Status { get; set; }

        public int MeterId { get; set; }

        public Meter Meter { get; set; }
    }
}
