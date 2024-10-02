using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "Full name")]
        [MaxLength(99)]
        public string FullName { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return $"https://localhost:44377/images/default_profile_picture.jpg";
                }

                return $"https://localhost:44377{ImageUrl.Substring(1)}";
            }
        }
    }
}
