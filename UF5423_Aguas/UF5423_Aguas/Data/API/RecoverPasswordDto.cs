using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.API
{
    public class RecoverPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
