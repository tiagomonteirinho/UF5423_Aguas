using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data.API
{
    public class MeterDto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Serial number")]
        public int SerialNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }

        [JsonIgnore]
        public ICollection<Consumption> Consumptions { get; set; }
    }
}
