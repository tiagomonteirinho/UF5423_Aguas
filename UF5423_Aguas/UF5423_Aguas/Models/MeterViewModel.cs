using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Models
{
    public class MeterViewModel
    {
        public int Id { get; set; }

        public int SerialNumber { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "A user must be selected")]
        public string UserEmail { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        [Required]
        [Display(Name = "Address")]
        [MaxLength(99)]
        public string Address { get; set; }
    }
}
