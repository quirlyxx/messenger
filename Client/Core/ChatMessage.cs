using System;

namespace Client.Core
{
    public class ChatMessage
    {
        public string FromLogin { get; set; } = string.Empty; // технический
        public string FromName { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string MessageId { get; set; } = string.Empty;
    }
}
