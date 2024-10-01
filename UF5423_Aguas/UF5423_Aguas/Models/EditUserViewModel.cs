using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(99)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}