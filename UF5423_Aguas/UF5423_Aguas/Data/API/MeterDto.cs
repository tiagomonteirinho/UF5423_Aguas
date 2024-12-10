using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.API
{
    public class MeterDto
    {
        public int Id { get; set; }

        [Required]
        public int SerialNumber { get; set; }

        [Required]
        [MaxLength(99)]
        public string Address { get; set; }
    }
}
