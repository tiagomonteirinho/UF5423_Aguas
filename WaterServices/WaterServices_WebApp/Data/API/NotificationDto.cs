using System;

namespace WaterServices_WebApp.Data.API
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }

        public bool Read { get; set; }
    }
}
