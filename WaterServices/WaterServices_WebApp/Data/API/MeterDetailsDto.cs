using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.API
{
    public class MeterDetailsDto
    {
        public int Id { get; set; }

        [Required]
        public int SerialNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }

        public ICollection<ConsumptionDto> Consumptions { get; set; } = new List<ConsumptionDto>();
    }
}
