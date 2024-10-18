using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace UF5423_Aguas.Data.Entities
{
    public class Meter : IEntity
    {
        public int Id { get; set; }

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
