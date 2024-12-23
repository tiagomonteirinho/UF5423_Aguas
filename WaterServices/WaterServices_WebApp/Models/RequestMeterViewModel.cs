using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Models
{
    public class RequestMeterViewModel
    {
        [Required]
        [MaxLength(99)]
        [Display(Name = "Contract holder (full name)")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone contact")]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(99)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [MaxLength(9)]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postal code (example: 0000-000)")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Serial number")]
        public int SerialNumber { get; set; }
    }
}
