using System.ComponentModel.DataAnnotations;

namespace Water_Services.Models
{
    public class SetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordToken { get; set; }

        public string ConfirmationToken { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
