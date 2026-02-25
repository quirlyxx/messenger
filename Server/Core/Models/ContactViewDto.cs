using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core.Models
{
    public class ContactViewDto
    {
        public string Login { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string DisplayName {  get; set; } = string.Empty;
    }
}
