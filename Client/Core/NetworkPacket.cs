using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class NetworkPacket
    {
        public string Action { get; set; } = string.Empty;
        public string? Data { get; set; }
    }
}
