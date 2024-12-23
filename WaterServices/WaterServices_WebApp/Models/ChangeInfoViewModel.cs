using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Models
{
    public class ChangeInfoViewModel
    {
        [Required]
        [MaxLength(99)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "New image")]
        public IFormFile ImageFile { get; set; }
    }
}
