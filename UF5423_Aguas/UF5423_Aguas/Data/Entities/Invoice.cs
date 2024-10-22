namespace UF5423_Aguas.Data.Entities
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }

        public Consumption Consumption { get; set; }

        public int ConsumptionId { get; set; }

        public decimal Price { get; set; }
    }
}
