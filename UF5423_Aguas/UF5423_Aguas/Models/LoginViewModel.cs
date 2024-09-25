using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
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
