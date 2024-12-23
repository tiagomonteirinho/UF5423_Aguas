using System.ComponentModel.DataAnnotations;

namespace WaterServices_WebApp.Data.API
{
    public class ChangeInfoDto
    {
        [Required]
        [MaxLength(99)]
        public string Name { get; set; }
    }
}
