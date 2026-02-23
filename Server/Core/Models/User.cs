using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Models
{
    public class User
    {
        public string Login { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public List<string> Contacts { get; set; } = new();
        public List<string> IncomingRequests { get; set; } = new();
    }
}
