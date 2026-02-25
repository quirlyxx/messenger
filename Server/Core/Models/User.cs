using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Models
{
    public class User
    {
        public string Login { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<Contact> Contacts { get; set; } = new();
        public List<string> IncomingRequests { get; set; } = new();
        public List<PendingItem> Pending { get; set; } = new();
    }

    public class PendingItem
    {
        public string Kind { get; set; } = "";
        public string Json { get; set; } = "";

    }
    public class Contact
    {
        public string Login {  set; get; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
    }
}
