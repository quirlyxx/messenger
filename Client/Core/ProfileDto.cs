using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Core
{
    public class ProfileDto
    {
        public string Login { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    public class ContactViewDto
    {
        public string Login { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;

    }
}
