using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class TierUsage : IEntity
    {
        public int Id { get; set; }

        public int TierId { get; set; }

        public int VolumeUsed { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Price { get; set; }
    }
}
