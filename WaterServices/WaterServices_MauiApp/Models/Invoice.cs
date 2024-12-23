namespace WaterServices_MauiApp.Models;

public class Invoice
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public Consumption? Consumption { get; set; }

    public Meter? Meter { get; set; }
}
