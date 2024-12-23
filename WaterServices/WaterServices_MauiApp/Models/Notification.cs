namespace WaterServices_MauiApp.Models;

public class Notification
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public DateTime Date { get; set; }

    public bool Read { get; set; }
}
