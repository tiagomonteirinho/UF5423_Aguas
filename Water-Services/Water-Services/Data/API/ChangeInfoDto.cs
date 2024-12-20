using System.ComponentModel.DataAnnotations;

namespace Water_Services.Data.API
{
    public class ChangeInfoDto
    {
        [Required]
        [MaxLength(99)]
        public string Name { get; set; }
    }
}
