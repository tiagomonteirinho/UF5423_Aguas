using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;

namespace UF5423_Aguas.Models
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Full name")]
        [MaxLength(99)]
        public string FullName { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "A role must be selected")]
        public string RoleName { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
