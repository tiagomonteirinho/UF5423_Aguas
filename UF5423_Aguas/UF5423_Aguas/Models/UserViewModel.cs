using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Models
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Full name")]
        [MaxLength(99)]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
