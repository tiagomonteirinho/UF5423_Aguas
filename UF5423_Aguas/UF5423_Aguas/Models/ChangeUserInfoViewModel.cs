using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class ChangeUserInfoViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }
}
