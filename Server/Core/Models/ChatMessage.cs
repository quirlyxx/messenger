using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Models
{
    public class ChatMessage
    {
        public string FromLogin { get; set; } = string.Empty; // технический
        public string FromName { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; }
    }
}
