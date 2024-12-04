namespace UF11027_Aguas_.NET_MAUI_App.Models
{
    public class MeterDetails
    {
        public int Id { get; set; }

        public int SerialNumber { get; set; }

        public string? Address { get; set; }

        public List<Consumption>? Consumptions { get; set; }
    }
}
