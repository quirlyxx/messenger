using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class MessageStatusIncoming
    {
        public string MessageId { get; set; } = "";
        public string Status { get; set; } = "";
        public string PeerLogin { get; set; } = "";
    }
}
