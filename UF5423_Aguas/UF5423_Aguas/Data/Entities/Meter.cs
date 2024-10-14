using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.Entities
{
    public class Meter : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        [MaxLength(99)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Address")]
        [MaxLength(99)]
        public string Address { get; set; }

        public string UserEmail { get; set; }

        [Display(Name = "User")]
        public User User { get; set; }

        public ICollection<Consumption> Consumptions { get; set; }
    }
}
