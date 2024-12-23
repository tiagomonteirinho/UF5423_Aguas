using System;
using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.API
{
    public class ConsumptionDto
    {
        public int Id { get; set; }

        public string Date { get; set; }

        [Required]
        public int Volume { get; set; }

        public string Status { get; set; }

        public int MeterId { get; set; }
    }
}
