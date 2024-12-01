using System;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data.API
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Action { get; set; }

        public User Receiver { get; set; }

        public string ReceiverEmail { get; set; }

        public string ReceiverRole { get; set; }

        public DateTime Date { get; set; }

        public bool Read { get; set; }

        public string NewAccountEmail { get; set; }
    }
}
