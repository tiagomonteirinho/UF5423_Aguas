using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
