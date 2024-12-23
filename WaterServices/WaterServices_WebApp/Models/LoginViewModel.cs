using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool StaySignedIn { get; set; }
    }
}
