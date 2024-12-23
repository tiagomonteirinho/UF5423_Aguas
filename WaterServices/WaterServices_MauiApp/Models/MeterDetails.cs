namespace WaterServices_MauiApp.Models;

public class MeterDetails
{
    public int Id { get; set; }

    public int SerialNumber { get; set; }

    public string? Address { get; set; }

    public List<Consumption>? Consumptions { get; set; }
}
