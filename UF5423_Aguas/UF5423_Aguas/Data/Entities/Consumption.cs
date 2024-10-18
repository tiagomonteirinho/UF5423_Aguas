using System;
using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.Entities
{
    public class Consumption : IEntity
    {
        public Consumption()
        {
            Date = DateTime.Now;
        }

        public int Id { get; set; }

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
