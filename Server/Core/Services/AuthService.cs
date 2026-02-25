using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Server.Core.Models;
using Server.Storage;

namespace Server.Core.Services
{
    public class AuthService
    {
        private readonly List<User> _users;
        private static string Norm(string s) => (s ?? "").Trim().ToLowerInvariant();

        public AuthService()
        {
            _users = JsonStorage.Load() ?? new List<User>();
        }

        public bool Register(string login, string password)
        {
            if (_users.Any(u => Norm(u.Login) == login)) return false;

            _users.Add(new User
            {
                Login = login,
                HashPassword = Hash(password),
                UserName = login
            });
            JsonStorage.Save(_users);
            return true;
        }

        public User? Login(string login, string password)
        {
            login = Norm(login);
            var hash = Hash(password);
            return _users.FirstOrDefault(u => u.Login == login && u.HashPassword == hash);
        }

        private string Hash(string password)
        {
            
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public bool UpdateUserName(string login, string newUserName)
        {
            login = Norm(login);
            newUserName = (newUserName ?? "").Trim();

            if(string.IsNullOrWhiteSpace(newUserName)) return false;
            if(newUserName.Length > 32) return false;

            var user = _users.FirstOrDefault(u => Norm(u.Login) == login);
            if(user == null) return false;

            user.UserName = newUserName;
            JsonStorage.Save(_users);
            Logger.Log($"User name updated: {login} -> {newUserName}", Logger.LogLevel.Info);
            return true;
            
        }

        public bool UpdateContactAlias(string ownerLogin, string contactLogin,string alias)
        {
            ownerLogin = Norm(ownerLogin);
            contactLogin = Norm(contactLogin);
            alias = (alias ?? "").Trim();

            var owner = _users.FirstOrDefault(u => Norm(u.Login) == ownerLogin);
            if (owner == null) return false;

            var contact = owner.Contacts
                .FirstOrDefault(c => Norm(c.Login) == contactLogin);

            if (contact == null) return false;

            contact.Alias = alias;

            JsonStorage.Save(_users);
            return true;
        }

        public List<ContactViewDto> GetContactView(string ownerLogin)
        {
            ownerLogin = Norm(ownerLogin);
            var owner = _users.FirstOrDefault(u => Norm(u.Login) == ownerLogin);
            if (owner == null) return new List<ContactViewDto>();

            var result = new List<ContactViewDto>();

            foreach(var c in owner.Contacts)
            {
                var login = Norm(c.Login);
                var remote = _users.FirstOrDefault(u => Norm(u.Login) == login);

                var display = !string.IsNullOrWhiteSpace(c.Alias)
                ? c.Alias
                : (remote?.UserName ?? login);

                result.Add(new ContactViewDto
                {
                    Login = login,
                    Alias = c.Alias ?? "",
                    DisplayName = display
                });
            }
            return result;
        }

        public User? GetUser(string login)
        {
            login = Norm(login);
            return _users.FirstOrDefault(u => Norm(u.Login) == login);
        }

        public bool SendContactRequest(string from, string to)
        {
            from = Norm(from);
            to = Norm(to);
            var sender = _users.FirstOrDefault(u => u.Login == from);
            var target = _users.FirstOrDefault(u => u.Login == to);
            if (sender == null || target == null)
            {
                Logger.Log($"User not found: {from} or {to}", Logger.LogLevel.Error);
                return false;
            }
            if (target.Contacts.Any(c => c.Login == from))
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
            user = Norm(user);
            from = Norm(from);
            
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

            // ВАЖНО: добавляем, если ЕЩЁ НЕТ
            if (!current.Contacts.Any(c => c.Login == from))
                current.Contacts.Add(new Contact { Login = from, Alias = "" });

            if (!sender.Contacts.Any(c => c.Login == user))
                sender.Contacts.Add(new Contact { Login = user, Alias = "" });

            JsonStorage.Save(_users);
            Logger.Log($"Contact request accepted: {user} <-> {from}", Logger.LogLevel.Success);
            return true;
        }


        public bool RemoveContact(string user, string contactLogin)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);
            var target = _users.FirstOrDefault(u => u.Login == contactLogin);

            if (current == null || target == null)
            {
                Logger.Log($"User not found: {user} or {contactLogin}", Logger.LogLevel.Error);
                return false;
            }

            if (!current.Contacts.Any(c => c.Login == contactLogin))
                return false;

            current.Contacts.RemoveAll(c => c.Login == contactLogin);
            target.Contacts.RemoveAll(c => c.Login == user);

            JsonStorage.Save(_users);
            Logger.Log($"Contact removed: {user} and {contactLogin}", Logger.LogLevel.Info);
            return true;
        }

        public List<Contact> GetContacts(string user)
        {
            var current = _users.FirstOrDefault(u => u.Login == user);
            return current?.Contacts ?? new List<Contact>();
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