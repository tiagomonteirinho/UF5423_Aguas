using System.ComponentModel.DataAnnotations;

namespace Water_Services.Data.API
{
    public class RequestWaterMeterDto
    {
        [Required]
        [MaxLength(99)]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }

        [Required]
        [MaxLength(9)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        public int SerialNumber { get; set; }
    }
}
