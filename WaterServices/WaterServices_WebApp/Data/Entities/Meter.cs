using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.Entities
{
    public class Meter : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Serial number")]
        public int SerialNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }

        public string UserEmail { get; set; }

        public User User { get; set; }

        public ICollection<Consumption> Consumptions { get; set; }
    }
}
