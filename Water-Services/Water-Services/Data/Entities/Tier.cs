﻿using System.ComponentModel.DataAnnotations;

namespace Water_Services.Data.Entities
{
    public class Tier : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Unit price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Display(Name = "Volume limit")]
        public int VolumeLimit { get; set; }
    }
}
