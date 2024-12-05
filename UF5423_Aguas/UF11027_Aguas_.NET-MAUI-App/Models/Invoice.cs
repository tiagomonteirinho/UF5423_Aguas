namespace UF11027_Aguas_.NET_MAUI_App.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public Consumption? Consumption { get; set; }

        public Meter? Meter { get; set; }
    }
}
