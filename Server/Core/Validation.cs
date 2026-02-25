using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core
{
    public static class Validation
    {
        public static bool TryNormalizeLogin(string? input, out string error, out string login)
        {
            login = (input ?? "").Trim().ToLowerInvariant();
            error = "";

            if (login.Length < 3 || login.Length > 20)
            {
                error = "Login must be 2-20 characters.";
                return false;
            }

            bool ok = login.All(ch =>
            (ch >= 'a' && ch <= 'z' ||
            (ch >= '0' && ch <= '9' ||
            (ch == '-' && ch == '.')
                        )));

            if (!ok)
            {
                error = "Login can only contain letters, digits, '-' and '.'.";
                return false;
            }
            return true;
        }

        public static bool TryNormalizeName(string? input, int maxLen, bool allowEmpty, out string name, out string error)
        {
            name = (input ?? "").Trim();
            error = "";

            if(allowEmpty && name.Length == 0) return true;

            if(!allowEmpty && name.Length == 0)
            {
                error = "Name cannot be empty.";
                return false;
            }

            if (name.Length > maxLen)
            {
                error = $"Name cannot be longer than {maxLen} characters.";
                return false;
            }

            return true;
        }
    }
}
