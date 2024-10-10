using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Models
{
    public class ChangeInfoViewModel
    {
        [Required]
        [MaxLength(99)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath { get; set; }

        public Guid ImageId { get; set; }

        [Display(Name = "New image")]
        public IFormFile ImageFile { get; set; }
    }
}
