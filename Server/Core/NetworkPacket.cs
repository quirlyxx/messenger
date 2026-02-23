using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core
{
    public class NetworkPacket
    {
        public string Action { get; set; } = string.Empty;
        public string? Data { get; set; }
    }
}
