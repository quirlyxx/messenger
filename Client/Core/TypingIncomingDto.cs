using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class TypingIncomingDto
    {
        public string FromLogin { get; set; } = "";
        public bool IsTyping { get; set; }
    }
}
