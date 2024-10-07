using System;

namespace UF5423_Aguas.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
