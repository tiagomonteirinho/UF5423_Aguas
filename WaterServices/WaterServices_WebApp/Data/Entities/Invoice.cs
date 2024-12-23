using System.Collections.Generic;

namespace WaterServices_WebApp.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public Consumption Consumption { get; set; }

        public int ConsumptionId { get; set; }

        public decimal Price { get; set; }

        public string Nonce { get; set; }

        public List<TierUsage> TierUsages { get; set; } = new List<TierUsage>();
    }
}
