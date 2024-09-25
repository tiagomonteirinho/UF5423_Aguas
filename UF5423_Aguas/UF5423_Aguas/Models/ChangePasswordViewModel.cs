using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        [Display(Name = "New password")]
        public string ConfirmNewPassword { get; set; }
    }
}
