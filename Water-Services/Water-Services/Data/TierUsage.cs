using Water_Services.Data.Entities;

namespace Water_Services.Data
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
