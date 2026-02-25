using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.Core;
using Server.Core.Models;
using Server.Core.Services;

namespace Server.Network
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly AuthService _authService;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private static readonly Dictionary<string, ClientHandler> _connectedClients = new();
        private static readonly object _lock = new();
        private string? _currentUser;
        private static string Norm(string s) => (s ?? "").Trim().ToLowerInvariant();

        public ClientHandler(TcpClient client, AuthService authService)
        {
            _client = client;
            _authService = authService;
        }

        public static Dictionary<string, ClientHandler> ConnectedClients
        {
            get
            {
                lock (_lock)
                {
                    return new Dictionary<string, ClientHandler>(_connectedClients);
                }
            }
        }

        public async Task ProcessAsync()
        {
            try
            {
                var stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                while (true)
                {
                    var json = await _reader.ReadLineAsync();
                    if (json == null) break;
                    var packet = JsonSerializer.Deserialize<NetworkPacket>(json);
                    Logger.Log($"SERVER: Received Action={packet?.Action}", Logger.LogLevel.Debug);
                    if (packet != null) await HandlePacketAsync(packet);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error with client {_currentUser ?? "unknown"}: {e.Message}", Logger.LogLevel.Error);
            }
            finally
            {
                if (_currentUser != null)
                {
                    lock (_lock)
                    {
                        _connectedClients.Remove(_currentUser);
                        Logger.Log($"User {_currentUser} disconnected", Logger.LogLevel.Info);
                    }
                }
                _client.Close();
            }
        }

        private async Task HandlePacketAsync(NetworkPacket packet)
        {
            switch (packet.Action)
            {
                case "Register": await HandleRegisterAsync(packet.Data); break;
                case "Login": await HandleLoginAsync(packet.Data); break;
                case "SendMessage": await HandleSendMessageAsync(packet.Data); break;
                case "SendContactRequest": await HandleSendRequest(packet.Data); break;
                case "AcceptContact": await HandleAcceptRequest(packet.Data); break;
                case "RemoveContact": await HandleRemoveContact(packet.Data); break;
                case "GetContacts": await HandleGetContacts(); break;
                case "GetRequests": await HandleGetRequests(); break;
                case "DeclineContact": await HandleDeclineRequest(packet.Data); break;
                case "Logout": await HandleLogoutAsync(); break;
                case "SendFile": await HandleSendFileAsync(packet.Data); break;
                case "UpdateProfileName": await HandleUpdateProfileNameAsync(packet.Data); break;
                case "UpdateContactAlias": await HandleUpdateContactAliasAsync(packet.Data); break;
                case "GetProfile": await HandleGetProfileAsync(); break;
                case "GetPresence": await HandleGetPresenceAsync(packet.Data); break;
                case "Typing": await HandleTypingAsync(packet.Data); break;
            }
        }

        private async Task HandleGetPresenceAsync(string? data)
        {
            if(_currentUser == null || data == null)return;

            var dto = JsonSerializer.Deserialize<PresenceRequestDto>(data);
            if (dto == null) return;

            var result = new Dictionary<string, bool>();
            var snapshot = ConnectedClients;

            foreach(var l in dto.Logins ?? new List<string>())
            {
                var key = Norm(l);
                result[key] = snapshot.ContainsKey(key);

            }

            await SendAsync(new NetworkPacket
            {
                Action = "PresenceData",
                Data = JsonSerializer.Serialize(result)
            });
        }

        private async Task HandleTypingAsync(string? data)
        {
            if (_currentUser == null || data == null) return;
            var dto = JsonSerializer.Deserialize<TypingDto>(data);
            if (dto == null) return;
            
            var toKey = Norm(dto.To);

            ClientHandler? target;
            lock (_lock) { _connectedClients.TryGetValue(toKey, out target); }
            if (target == null) return;

            await target.SendAsync(new NetworkPacket
            {
                Action = "Typing",
                Data = JsonSerializer.Serialize(new { FromLogin = _currentUser, IsTyping = dto.IsTyping })
            });
        }
        private async Task HandleGetProfileAsync()
        {
            if(_currentUser == null)  return;
            
            var user = _authService.GetUser(_currentUser);
            if(user == null) return;

            await SendAsync(new NetworkPacket
            {
                Action = "Profile",
                Data = JsonSerializer.Serialize(new { Login = user.Login, UserName = user.UserName })
            });
        }

        private async Task HandleUpdateProfileNameAsync(string? data)
        {
            if (_currentUser == null || data == null) return;

            var dto = JsonSerializer.Deserialize<UpdateProfileDto>(data);
            if (dto == null) return;

            var success = _authService.UpdateUserName(_currentUser, dto.UserName);

            await SendAsync(new NetworkPacket
            {
                Action = "UpdateProfileResult",
                Data = success ? "OK" : "FAIL"
            });

            if (!success) return;

            // Обновляем профиль себе
            await HandleGetProfileAsync();
            await HandleGetContacts();

            // 🔥 ВАЖНО: уведомляем всех онлайн-контактов
            var myContacts = _authService.GetContacts(_currentUser);

            foreach (var contact in myContacts)
            {
                ClientHandler? handler;

                lock (_lock)
                {
                    _connectedClients.TryGetValue(contact.Login, out handler);
                }

                if (handler != null)
                {
                    await handler.SendAsync(new NetworkPacket
                    {
                        Action = "ContactsList",
                        Data = JsonSerializer.Serialize(
                            _authService.GetContactView(contact.Login)
                        )
                    });
                }
            }

            Logger.Log($"{_currentUser} updated profile name to {dto.UserName}",
                Logger.LogLevel.Success);
        }

        private async Task HandleUpdateContactAliasAsync(string? data)
        {
            if (_currentUser == null || data == null) return;

            var dto = JsonSerializer.Deserialize<UpdateAliasDto>(data);
            if (dto == null) return;

            var success = _authService.UpdateContactAlias(_currentUser, dto.ContactLogin, dto.Alias);
            await SendAsync(new NetworkPacket { Action = "UpdateAliasResult", Data = success ? "OK" : "FAIL" });

            if (success)
            {
                await HandleGetContacts();
            }
        }

        private async Task HandleSendFileAsync(string? data)
        {
            if (data == null || _currentUser == null) return;

            var fileMsg = JsonSerializer.Deserialize<FileMessage>(data);
            if (fileMsg == null) return;

            // sender
            var senderLogin = Norm(_currentUser);
            var sender = _authService.GetUser(senderLogin);
            var senderName = sender?.UserName ?? senderLogin;

            // normalize receiver
            var toKey = Norm(fileMsg.To);

            fileMsg.FromLogin = senderLogin;
            fileMsg.FromName = senderName;
            fileMsg.To = toKey;
            fileMsg.Time = DateTime.Now;

            Logger.Log($"SERVER: SendFile from {fileMsg.FromLogin} to '{fileMsg.To}' size={fileMsg.SizeBytes}",
                Logger.LogLevel.Info);

            if (fileMsg.SizeBytes > 10 * 1024 * 1024)
            {
                await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "TOO_LARGE" });
                return;
            }

            ClientHandler? target;
            lock (_lock) { _connectedClients.TryGetValue(toKey, out target); }

            if (target == null)
            {
                await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "USER_OFFLINE" });
                Logger.Log($"SERVER: File target offline: {fileMsg.FromLogin} -> {fileMsg.To}", Logger.LogLevel.Warning);
                return;
            }

            await target.SendAsync(new NetworkPacket
            {
                Action = "ReceiveFile",
                Data = JsonSerializer.Serialize(fileMsg)
            });

            await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "OK" });

            Logger.Log($"SERVER: File delivered {fileMsg.FileName} {fileMsg.FromLogin} -> {fileMsg.To}",
                Logger.LogLevel.Success);
        }



        private async Task HandleLogoutAsync()
        {
            if (_currentUser == null) return;

            Logger.Log($"User {_currentUser} requested logout", Logger.LogLevel.Warning);


            await SendAsync(new NetworkPacket { Action = "LogoutResult", Data = "OK" });


            try { _client.Close(); } catch { }
        }

        private async Task HandleDeclineRequest(string? data)
        {
            if (_currentUser == null || data == null) return;

            var result = _authService.DeclineContactRequest(_currentUser, data);

            await SendAsync(new NetworkPacket
            {
                Action = "DeclineResult",
                Data = result ? "OK" : "FAIL"
            });

            Logger.Log($"{_currentUser} declined contact request from {data}",
                result ? Logger.LogLevel.Warning : Logger.LogLevel.Error);
        }

        private async Task HandleSendRequest(string? data)
        {
            if (_currentUser == null || data == null) return;

            var login = data.Trim().ToLowerInvariant();
            var result = _authService.SendContactRequest(_currentUser, login);

            await SendAsync(new NetworkPacket
            {
                Action = "ContactRequestResult",
                Data = result ? "OK" : "FAIL"
            });

            Logger.Log($"{_currentUser} sent contact request to {login}",
                result ? Logger.LogLevel.Success : Logger.LogLevel.Warning);

            if (result)
            {
                ClientHandler? targetHandler;
                lock (_lock)
                {
                    _connectedClients.TryGetValue(login, out targetHandler);
                }
                if (targetHandler != null)
                {
                    await targetHandler.SendAsync(new NetworkPacket
                    {
                        Action = "RequestsList",
                        Data = JsonSerializer.Serialize(_authService.GetIncomingRequests(login))
                    });
                    Logger.Log($"Notified {login} about new contact request from {_currentUser}", Logger.LogLevel.Info);
                }
            }
        }
        private async Task HandleAcceptRequest(string? data)
        {

            if (_currentUser == null || data == null) return;

            var result = _authService.AcceptContactRequest(_currentUser, data);
            if (result)
            {
                await SendAsync(new NetworkPacket
                {
                    Action = "ContactsList",
                    Data = JsonSerializer.Serialize(_authService.GetContactView(_currentUser))
                });

                await SendAsync(new NetworkPacket
                {
                    Action = "RequestsList",
                    Data = JsonSerializer.Serialize(_authService.GetIncomingRequests(_currentUser))
                });

                ClientHandler? senderHandler;
                lock (_lock) { _connectedClients.TryGetValue(data, out senderHandler); }

                if (senderHandler != null)
                {
                    await senderHandler.SendAsync(new NetworkPacket
                    {
                        Action = "ContactsList",
                        Data = JsonSerializer.Serialize(_authService.GetContactView(data))
                    });
                }
            }
            await SendAsync(new NetworkPacket
            {
                Action = "AcceptResult",
                Data = result ? "OK" : "FAIL"
            });

            Logger.Log($"{_currentUser} accepted request from {data}",
                result ? Logger.LogLevel.Success : Logger.LogLevel.Warning);
        }

        private async Task HandleRemoveContact(string? data)
        {
            if (_currentUser == null || data == null) return;

            var result = _authService.RemoveContact(_currentUser, data);
            if (result)
            {
                await SendAsync(new NetworkPacket
                {
                    Action = "ContactsList",
                    Data = JsonSerializer.Serialize(_authService.GetContacts(_currentUser))
                });
                ClientHandler? targetHandler;
                lock (_lock) { _connectedClients.TryGetValue(data, out targetHandler); }

                if (targetHandler != null)
                {
                    await targetHandler.SendAsync(new NetworkPacket
                    {
                        Action = "ContactsList",
                        Data = JsonSerializer.Serialize(_authService.GetContacts(data))
                    });
                }
            }
            await SendAsync(new NetworkPacket
            {
                Action = "RemoveResult",
                Data = result ? "OK" : "FAIL"
            });

            Logger.Log($"{_currentUser} removed contact {data}",
                result ? Logger.LogLevel.Warning : Logger.LogLevel.Error);
        }

        private async Task HandleGetContacts()
        {
            if (_currentUser == null) return;

            var contacts = _authService.GetContactView(_currentUser);

            await SendAsync(new NetworkPacket
            {
                Action = "ContactsList",
                Data = JsonSerializer.Serialize(contacts)
            });

            Logger.Log($"Contacts list sent to {_currentUser}", Logger.LogLevel.Info);
        }

        private async Task HandleGetRequests()
        {
            if (_currentUser == null) return;

            var requests = _authService.GetIncomingRequests(_currentUser);

            await SendAsync(new NetworkPacket
            {
                Action = "RequestsList",
                Data = JsonSerializer.Serialize(requests)
            });

            Logger.Log($"Incoming requests sent to {_currentUser}", Logger.LogLevel.Info);
        }

        private async Task HandleRegisterAsync(string? data)
        {
            if (data == null) return;
            var creds = JsonSerializer.Deserialize<LoginDto>(data);
            if (creds == null) return;
            var success = _authService.Register(creds.Login, creds.Password);
            await SendAsync(new NetworkPacket { Action = "RegisterResult", Data = success ? "OK" : "EXISTS" });
            Logger.Log(success
                ? $"User {creds.Login} registered successfully"
                : $"Registration failed: user {creds.Login} already exists",
                success ? Logger.LogLevel.Success : Logger.LogLevel.Warning);
        }

        private async Task HandleLoginAsync(string? data)
        {
            if (data == null) return;
            var creds = JsonSerializer.Deserialize<LoginDto>(data);
            if (creds == null) return;
            var loginNormalized = creds.Login.Trim().ToLowerInvariant();
            if (ServerCommandHandler.BannedUsers.Contains(loginNormalized))
            {
                await SendAsync(new NetworkPacket { Action = "LoginResult", Data = "BANNED" });
                try { _client.Close(); } catch { }
                return;
            }
            var user = _authService.Login(creds.Login, creds.Password);
            if (user != null)
            {
                _currentUser = user.Login.Trim().ToLowerInvariant();
                lock (_lock) { _connectedClients[_currentUser] = this; }
                await SendAsync(new NetworkPacket { Action = "LoginResult", Data = "OK" });
                Logger.Log($"User {_currentUser} logged in", Logger.LogLevel.Success);
            }
            else
            {
                await SendAsync(new NetworkPacket { Action = "LoginResult", Data = "FAIL" });
                Logger.Log($"Failed login attempt: {creds.Login}", Logger.LogLevel.Error);
            }
        }

        private async Task SendAsync(NetworkPacket packet)
        {
            if (_writer == null) return;

            try
            {
                var json = JsonSerializer.Serialize(packet);
                await _writer.WriteLineAsync(json);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to send message to {_currentUser ?? "unknown"}: {ex.Message}", Logger.LogLevel.Error);
            }
        }

        private async Task HandleSendMessageAsync(string? data)
        {
            if (data == null || _currentUser == null) return;

            var msg = JsonSerializer.Deserialize<ChatMessage>(data);
            if (msg == null) return;

            // кто отправитель
            var sender = _authService.GetUser(_currentUser);
            var senderName = sender?.UserName ?? _currentUser;

            msg.FromLogin = _currentUser;
            msg.FromName = senderName;
            msg.Time = DateTime.Now;

            ClientHandler? target;
            var toKey = msg.To.Trim().ToLowerInvariant();
            lock (_lock) { _connectedClients.TryGetValue(toKey, out target); }

            if (target != null)
            {
                await target.SendAsync(new NetworkPacket
                {
                    Action = "ReceiveMessage",
                    Data = JsonSerializer.Serialize(msg)
                });

                await SendAsync(new NetworkPacket { Action = "SendMessageResult", Data = "OK" });
            }
            else
            {
                await SendAsync(new NetworkPacket { Action = "SendMessageResult", Data = "USER_OFFLINE" });
            }
        }


        public void Disconnect()
        {
            Logger.Log($"Disconnecting {_currentUser ?? "unknown"}", Logger.LogLevel.Warning);
            try { _client.Close(); } catch { }
        }

        public void SendSystemMessage(string text)
        {
            if (_currentUser == null) return;
            var packet = new NetworkPacket
            {
                Action = "ReceiveMessage",
                Data = JsonSerializer.Serialize(new ChatMessage
                {
                    FromLogin = "SYSTEM",
                    To = _currentUser,
                    Text = text,
                    Time = DateTime.Now
                })
            };
            _ = SendAsync(packet);
        }
    }

    public class LoginDto
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        public string UserName { get; set; } = string.Empty;
    }

    public class UpdateAliasDto
    {
        public string ContactLogin { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
    }

    public class PresenceRequestDto
    {
        public List<string> Logins { get; set; } = new();
    }

    public class TypingDto
    {
        public string To { get; set; } = string.Empty;
        public bool IsTyping { get; set; }

    }
}