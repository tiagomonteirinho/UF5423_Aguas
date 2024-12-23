using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.API
{
    public class RecoverPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
