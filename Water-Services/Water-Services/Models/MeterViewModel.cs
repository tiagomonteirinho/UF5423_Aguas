﻿using System.ComponentModel.DataAnnotations;

namespace Water_Services.Models
{
    public class MeterViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Serial number")]
        public int SerialNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }

        public string UserEmail { get; set; }

        public string UserId { get; set; }
    }
}
