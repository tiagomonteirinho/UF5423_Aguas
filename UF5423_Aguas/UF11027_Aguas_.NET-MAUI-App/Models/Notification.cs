namespace UF11027_Aguas_.NET_MAUI_App.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public string? Action { get; set; }

        public DateTime Date { get; set; }

        public bool Read { get; set; }
    }
}
