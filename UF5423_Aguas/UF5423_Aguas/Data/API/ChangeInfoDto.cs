using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.API
{
    public class ChangeInfoDto
    {
        [Required]
        [MaxLength(99)]
        public string Name { get; set; }
    }
}
