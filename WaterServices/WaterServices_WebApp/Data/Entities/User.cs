﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "Full name")]
        [MaxLength(99)]
        public string FullName { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return $"https://fcbdb5dh-44326.uks1.devtunnels.ms/images/default_profile_picture.jpg";
                    //return $"https://localhost:44326/images/default_profile_picture.jpg";
                }

                return $"https://fcbdb5dh-44326.uks1.devtunnels.ms{ImageUrl.Substring(1)}";
                //return $"https://localhost:44326{ImageUrl.Substring(1)}";
            }
        }

        public ICollection<Meter> Meters { get; set; }
    }
}
