using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
