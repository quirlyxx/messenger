using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Server.Core.Models;
using Server.Storage;

namespace Server.Core.Services
{
    public class AuthService
    {
        private readonly List<User> _users;

        public AuthService()
        {
            _users = JsonStorage.Load() ?? new List<User>();
        }

        public bool Register(string login, string password)
        {
            if (_users.Any(u => u.Login == login)) return false;
            _users.Add(new User { Login = login, HashPassword = Hash(password) });
            JsonStorage.Save(_users);
            return true;
        }

        public User? Login(string login, string password)
        {
            var hash = Hash(password);
            return _users.FirstOrDefault(u => u.Login == login && u.HashPassword == hash);
        }

        private string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool SendContactRequest(string from, string to)
        {
            var sender = _users.FirstOrDefault(u => u.Login == from);
            var target = _users.FirstOrDefault(u => u.Login == to);
            if (sender == null || target == null)
            {
                Logger.Log($"User not found: {from} or {to}", Logger.LogLevel.Error);
                return false;
            }
            if (target.Contacts.Contains(from))
            {
                return false;
            }
            if (target.IncomingRequests.Contains(from))
            {
                return false;
            }
            target.IncomingRequests.Add(from);
            JsonStorage.Save(_users);
            return true;

        }

        public bool AcceptContactRequest(string user, string from)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);  
            var sender = _users.FirstOrDefault(u => u.Login == from);  

            if (current == null || sender == null)
            {
                Logger.Log($"User not found: {user} or {from}", Logger.LogLevel.Error);
                return false;
            }

            if (!current.IncomingRequests.Contains(from))
                return false;

            current.IncomingRequests.Remove(from);

            if (!current.Contacts.Contains(from))
                current.Contacts.Add(from);

            if (!sender.Contacts.Contains(user))
                sender.Contacts.Add(user);

            JsonStorage.Save(_users);
            Logger.Log($"Contact request accepted: {user} <-> {from}", Logger.LogLevel.Success);
            return true;
        }

        public bool RemoveContact(string user, string contact)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);
            var target = _users.FirstOrDefault(u => u.Login == contact);
            if (current == null || target == null)
            {
                Logger.Log($"User not found: {user} or {contact}", Logger.LogLevel.Error);
                return false;
            }
            if (!current.Contacts.Contains(contact))
            {
                return false;
            }
            current.Contacts.Remove(contact);
            target.Contacts.Remove(user);
            JsonStorage.Save(_users);
            Logger.Log($"Contact removed: {user} and {contact}", Logger.LogLevel.Info);
            return true;
        }

        public List<string> GetContacts(string user)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);
            return current?.Contacts ?? new List<string>();
        }

            public List<string> GetIncomingRequests(string user)
            {
                var current = _users.FirstOrDefault(u => u.Login == user);
                return current?.IncomingRequests ?? new List<string>();
        }

        public bool DeclineContactRequest(string user, string from)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);

            if (current == null) return false;
            if (!current.IncomingRequests.Contains(from)) return false;

            current.IncomingRequests.Remove(from);
            JsonStorage.Save(_users);

            return true;
        }
    }
}