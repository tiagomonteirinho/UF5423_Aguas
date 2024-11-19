using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.API
{
    public class ChangeInfoDto
    {
        [Required]
        [MaxLength(99)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }
    }
}
