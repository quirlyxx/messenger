using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
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
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };

                while (true)
                {
                    var json = await _reader.ReadLineAsync();
                    if (json == null) break;
                    var packet = JsonSerializer.Deserialize<NetworkPacket>(json);
                    Logger.Log($"SERVER: Received Action={packet?.Action}", Logger.LogLevel.Info);
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

            }
        }

        private async Task HandleSendFileAsync(string? data)
        {
            if (data == null || _currentUser == null) return;

            var fileMsg = JsonSerializer.Deserialize<FileMessage>(data);
            if (fileMsg == null) return;

            fileMsg.From = _currentUser.Trim();
            fileMsg.To = fileMsg.To.Trim();
            fileMsg.Time = DateTime.Now;

            Logger.Log($"SERVER: SendFile from {fileMsg.From} to '{fileMsg.To}' size={fileMsg.SizeBytes}",
                Logger.LogLevel.Info);

            if (fileMsg.SizeBytes > 10 * 1024 * 1024)
            {
                await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "TOO_LARGE" });
                Logger.Log($"SERVER: File too large from {fileMsg.From} to {fileMsg.To}", Logger.LogLevel.Warning);
                return;
            }

            ClientHandler? target;
            lock (_lock) { _connectedClients.TryGetValue(fileMsg.To, out target); }

            if (target == null)
            {
                await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "USER_OFFLINE" });
                Logger.Log($"SERVER: File target offline: {fileMsg.From} -> {fileMsg.To}", Logger.LogLevel.Warning);
                return;
            }

            await target.SendAsync(new NetworkPacket
            {
                Action = "ReceiveFile",
                Data = JsonSerializer.Serialize(fileMsg)
            });

            await SendAsync(new NetworkPacket { Action = "SendFileResult", Data = "OK" });

            Logger.Log($"SERVER: File delivered {fileMsg.FileName} {fileMsg.From} -> {fileMsg.To}",
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

            var login = data;
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
                    Data = JsonSerializer.Serialize(_authService.GetContacts(_currentUser))
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
                        Data = JsonSerializer.Serialize(_authService.GetContacts(data))
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

            var contacts = _authService.GetContacts(_currentUser);

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
            msg.From = _currentUser;
            msg.Time = DateTime.Now;

            ClientHandler? target;
            lock (_lock) { _connectedClients.TryGetValue(msg.To, out target); }
            if (target != null)
            {
                await target.SendAsync(new NetworkPacket { Action = "ReceiveMessage", Data = JsonSerializer.Serialize(msg) });
                Logger.Log($"Message from {msg.From} to {msg.To}: {msg.Text}", Logger.LogLevel.Message);
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
                    From = "SYSTEM",
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
}