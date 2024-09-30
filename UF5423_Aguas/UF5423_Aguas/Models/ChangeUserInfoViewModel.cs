using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class ChangeUserInfoViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Current Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "New Image")]
        public IFormFile ImageFile { get; set; }
    }
}
