using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Models
{
    public class FileMessage
    {
        public string FromLogin { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public string Base64 { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime Time { get; set; }
    }
}
