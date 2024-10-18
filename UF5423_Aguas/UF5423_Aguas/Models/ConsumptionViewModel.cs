using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Models
{
    public class ConsumptionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Volume")]
        public int Volume { get; set; }

        [Display(Name = "Payment confirmed")]
        public bool PaymentConfirmed { get; set; }

        public int MeterId { get; set; }

        public Meter Meter { get; set; }
    }
}
