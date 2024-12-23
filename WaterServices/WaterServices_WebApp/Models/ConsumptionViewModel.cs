using System;
using System.ComponentModel.DataAnnotations;
using WaterServices_WebApp.Data.Entities;

namespace WaterServices_WebApp.Models
{
    public class ConsumptionViewModel
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Volume { get; set; }

        public string Status { get; set; }

        public int MeterId { get; set; }

        public Meter Meter { get; set; }
    }
}
