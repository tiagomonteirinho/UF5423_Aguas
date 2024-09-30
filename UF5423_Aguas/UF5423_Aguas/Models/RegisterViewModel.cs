using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Models
{
    public class RegisterViewModel : User
    {
        [Required]
        [MinLength(6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
