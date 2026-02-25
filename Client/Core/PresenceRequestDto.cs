using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class PresenceRequestDto
    {
        public List<string> Logins { get; set; } = new();
    }
}
