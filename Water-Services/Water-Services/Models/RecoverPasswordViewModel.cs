using System.ComponentModel.DataAnnotations;

namespace Water_Services.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
