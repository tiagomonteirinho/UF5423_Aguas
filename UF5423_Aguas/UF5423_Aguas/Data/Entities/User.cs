using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.Entities
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
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://uf5423aguasapp.azurewebsites.net//images/default_profile_picture.jpg"
            : $"http://uf5423aguasstorageacc.blob.core.windows.net/users/{ImageId}";

        public ICollection<Meter> Meters { get; set; }
    }
}
