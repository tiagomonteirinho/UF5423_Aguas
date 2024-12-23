namespace WaterServices_MauiApp.Models;

public class Consumption
{
    public int Id { get; set; }

    public string? Date { get; set; }

    public int Volume { get; set; }

    public string? Status { get; set; }

    public int MeterId { get; set; }
}
