using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
        public class ChatMessage
        {
            public string? Text { get; set; }
            public string? Time { get; set; }
            public bool IsUser { get; set; }
        }
}
