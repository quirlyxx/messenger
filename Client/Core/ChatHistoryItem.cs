using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class ChatHistoryItem
    {
        public string WithLogin { get; set; } = "";
        public bool isOutgoing { get; set; }
        public string FromLogin { get; set; } = "";
        public string FromName { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Time { get; set; }
        public bool isFile { get; set; }
        public string? FileName { get; set; }
        public string? SavedPath { get; set; }
        public string MessageId { get; set; } = "";
        public string Status { get; set; } = "Sent";
        public long SizeBytes { get; set; }
        public bool IsRead { get; set; }
    }
}
