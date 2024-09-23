using System.ComponentModel.DataAnnotations;

namespace UF5423_Aguas.Data.Entities
{
    public class Meter : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(99)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
