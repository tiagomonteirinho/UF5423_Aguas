using System.ComponentModel.DataAnnotations;

namespace Water_Services.Data.API
{
    public class RecoverPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
