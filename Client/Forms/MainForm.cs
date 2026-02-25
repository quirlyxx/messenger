using Client.Core;
using Client.Forms.Controls;
using Client.Network;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Client.Forms
{
    public partial class MainForm : Form
    {
        private readonly NetworkClient _client;
        private readonly string _login;
        private List<ContactViewDto> _contactsCache = new();
        private string? _activeChatLogin;
        private System.Windows.Forms.Timer _presenceTimer = new();
        private Dictionary<string, bool> _presence = new();
        private System.Windows.Forms.Timer _typingStopTimer = new();
        private bool _isTypingSent;

        public MainForm(NetworkClient client, string login)
        {

            InitializeComponent();

            flpChat.AutoScroll = true;
            flpChat.WrapContents = false;
            flpChat.FlowDirection = FlowDirection.TopDown;
            flpChat.Padding = new Padding(12);
            flpChat.BackColor = Color.FromArgb(20, 24, 32);
            flpChat.HorizontalScroll.Enabled = false;
            flpChat.HorizontalScroll.Visible = false;

            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(flpChat, true, null);

            flpChat.ControlAdded += (s, e) => ScrollChatToBottom();

            // 1) базовые поля
            _client = client;
            _login = (login ?? "").Trim().ToLowerInvariant();

            // 2) базовая настройка окна (по желанию)
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;

            // 3) OWNER DRAW ListBox (важно до заполнения Items)
            lstContacts.DrawMode = DrawMode.OwnerDrawVariable;
            lstContacts.MeasureItem += LstContacts_MeasureItem;
            lstContacts.DrawItem += LstContacts_DrawItem;

            // 4) подписки на сеть (до LoadInitialData)
            _client.OnPacketReceived += OnPacketReceived;

            // 5) таймер присутствия
            _presenceTimer.Interval = 2000;
            _presenceTimer.Tick += async (s, e) => await SendPresenceAsync();
            _presenceTimer.Start();

            // 6) таймер "перестал печатать" (СНАЧАЛА Tick, ПОТОМ Start/Stop)
            _typingStopTimer.Interval = 800;
            _typingStopTimer.Tick += async (s, e) =>
            {
                _typingStopTimer.Stop();
                await SendTypingAsync(false);
                _isTypingSent = false;
            };

            // 7) TextChanged — ПОСЛЕ настройки таймера
            txtMessage.TextChanged += async (s, e) =>
            {
                if (_activeChatLogin == null) return;
                if (!_client.IsConnected) return;

                if (!_isTypingSent)
                {
                    await SendTypingAsync(true);
                    _isTypingSent = true;
                }

                _typingStopTimer.Stop();
                _typingStopTimer.Start();
            };

            // 8) первичное UI
            lblCurrentUser.Text = $"Current User: {_login}";
            lblTyping.Visible = false;
            lblTyping.Text = "";
            lblStatus.Text = "";
            lblStatus.ForeColor = Color.Gray;

            // 9) стартовые запросы к серверу
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            await _client.SendAsync(new NetworkPacket { Action = "GetContacts" });
            await _client.SendAsync(new NetworkPacket { Action = "GetRequests" });
            await _client.SendAsync(new NetworkPacket { Action = "GetProfile" });
        }


        private void AddTextBubble(string text, bool isMine, DateTime time, string name, string? status = null)
        {
            flpChat.SuspendLayout();

            var row = new Panel
            {
                Height = 1,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 6, 0, 6),
                Padding = new Padding(10, 0, 10, 0)
            };

            // строка должна быть "на всю ширину"
            row.Width = flpChat.ClientSize.Width - flpChat.Padding.Horizontal - 5;
            row.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            var bubble = new MessageBubble();
            bubble.SetTextMessage(isMine, name, text, time, status);

            // ограничение ширины пузыря
            bubble.MaximumSize = new Size((int)(flpChat.ClientSize.Width * 0.65), 0);
            bubble.AutoSize = true;

            row.Controls.Add(bubble);

            // ВАЖНО: после добавления можно посчитать PreferredSize и выставить Left
            row.Layout += (s, e) =>
            {
                // подстрахуемся на ресайз/скролл
                row.Width = flpChat.ClientSize.Width - flpChat.Padding.Horizontal - 5;

                var w = bubble.PreferredSize.Width;
                bubble.Width = w;

                bubble.Top = 0;

                if (isMine)
                    bubble.Left = row.Width - row.Padding.Right - bubble.Width; // вправо
                else
                    bubble.Left = row.Padding.Left; // влево
            };

            flpChat.Controls.Add(row);

            flpChat.ResumeLayout();
            ScrollChatToBottom();
        }

        private void AddFileBubble(string fileName, long sizeBytes, string pathToOpen,
    bool isMine, DateTime time, string name, string? status = null)
        {
            flpChat.SuspendLayout();

            var row = new Panel
            {
                Height = 1,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 6, 0, 6),
                Padding = new Padding(10, 0, 10, 0)
            };

            row.Width = flpChat.ClientSize.Width - flpChat.Padding.Horizontal - 5;
            row.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            var bubble = new MessageBubble();
            bubble.SetFileMessage(isMine, name, fileName, sizeBytes, time, pathToOpen, status);

            bubble.MaximumSize = new Size((int)(flpChat.ClientSize.Width * 0.65), 0);
            bubble.AutoSize = true;

            row.Controls.Add(bubble);

            row.Layout += (s, e) =>
            {
                row.Width = flpChat.ClientSize.Width - flpChat.Padding.Horizontal - 5;

                var w = bubble.PreferredSize.Width;
                bubble.Width = w;

                bubble.Top = 0;

                if (isMine)
                    bubble.Left = row.Width - row.Padding.Right - bubble.Width;
                else
                    bubble.Left = row.Padding.Left;
            };

            flpChat.Controls.Add(row);

            flpChat.ResumeLayout();
            ScrollChatToBottom();
        }

        private void ScrollChatToBottom()
        {
            if (flpChat.Controls.Count == 0) return;
            flpChat.ScrollControlIntoView(flpChat.Controls[flpChat.Controls.Count - 1]);
        }

        

        private async Task SendTypingAsync(bool isTyping)
        {
            if(_activeChatLogin == null) return;

            await _client.SendAsync(new NetworkPacket {
                Action = "Typing",
                Data = JsonSerializer.Serialize(new { To = _activeChatLogin, IsTyping = isTyping })
            });
        }

        private async Task SendPresenceAsync()
        {
            if (!_client.IsConnected) return;
            if(_contactsCache.Count == 0) return;

            var dto = new Client.Core.PresenceRequestDto
            {
                Logins = _contactsCache.Select(c => c.Login).ToList()
            };

            await _client.SendAsync(new NetworkPacket
            {
                Action = "GetPresence",
                Data = JsonSerializer.Serialize(dto)
            });
        }

        private void LstContacts_MeasureItem(object? sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 42;
        }

        private void LstContacts_DrawItem(object? sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0 || e.Index >= lstContacts.Items.Count) return;

            var item = (ContactViewDto)lstContacts.Items[e.Index];

            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var bg = isSelected ? SystemColors.Highlight : lstContacts.BackColor;
            var fg1 = isSelected ? SystemColors.HighlightText : lstContacts.ForeColor;
            var fg2 = isSelected ? SystemColors.HighlightText : Color.Gray;

            using var bgBrush = new SolidBrush(bg);
            e.Graphics.FillRectangle(bgBrush, e.Bounds);

            var nameFont = new Font(e.Font, FontStyle.Regular);
            var loginFont = new Font(e.Font.FontFamily, e.Font.Size - 1, FontStyle.Regular);

            var x = e.Bounds.Left + 8;
            var y = e.Bounds.Top + 4;

            e.Graphics.DrawString(item.DisplayName, nameFont, new SolidBrush(fg1), x, y);
            e.Graphics.DrawString($"@{item.Login}", loginFont, new SolidBrush(fg2), x, y + 18);

            e.DrawFocusRectangle();
        }

        private void OpenChat(string withLogin)
        {
            if (string.IsNullOrWhiteSpace(withLogin))
                return;

            _activeChatLogin = withLogin.Trim().ToLowerInvariant();

            flpChat.SuspendLayout();
            flpChat.Controls.Clear();

            var history = ChatHistoryStore.Load(_login, _activeChatLogin);

            foreach (var h in history)
            {
                var name = h.isOutgoing
                    ? "Me"
                    : ResolveSenderName(h.FromLogin, h.FromName);

                string? status = h.isOutgoing ? h.Status : null;

                if (!h.isFile)
                {
                    AddTextBubble(
                        h.Text ?? "",
                        isMine: h.isOutgoing,
                        time: h.Time,
                        name: name,
                        status: status
                    );
                }
                else
                {
                    AddFileBubble(
                        h.FileName ?? "file",
                        h.SizeBytes,
                        h.SavedPath,
                        isMine: h.isOutgoing,
                        time: h.Time,
                        name: name,
                        status: status
                    );
                }
            }

            flpChat.ResumeLayout();
            ScrollChatToBottom();

            // ✅ после открытия — помечаем входящие как прочитанные
            MarkIncomingAsReadAndNotifyServer();

            UpdatePresenceUi();
        }

        private void MarkIncomingAsReadAndNotifyServer()
        {
            if (_activeChatLogin == null) return;
            if (!_client.IsConnected) return;

            var peer = _activeChatLogin.Trim().ToLowerInvariant();
            var history = ChatHistoryStore.Load(_login, peer);

            bool changed = false;

            foreach (var h in history)
            {
                if (h.isOutgoing) continue;
                if (h.IsRead) continue;
                if (string.IsNullOrWhiteSpace(h.MessageId)) continue;

                _ = _client.SendAsync(new NetworkPacket
                {
                    Action = "MsgRead",
                    Data = JsonSerializer.Serialize(new { MessageId = h.MessageId, PeerLogin = peer })
                });

                h.IsRead = true;
                changed = true;
            }

            if (changed)
                ChatHistoryStore.SaveAll(_login, peer, history);
        }

        private static int StatusRank(string s) => (s ?? "") switch
        {
            "Sent" => 1,
            "Delivered" => 2,
            "Read" => 3,
            _ => 0
        };

        private void UpdateOutgoingStatusHistory(string peerLogin, string messageId, string newStatus)
        {
            var peer = (peerLogin ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(peer)) return;
            if (string.IsNullOrWhiteSpace(messageId)) return;

            var list = ChatHistoryStore.Load(_login, peer);

            var item = list.LastOrDefault(x => x.isOutgoing && x.MessageId == messageId);
            if (item == null) return;

            if (StatusRank(newStatus) > StatusRank(item.Status))
            {
                item.Status = newStatus;
                ChatHistoryStore.SaveAll(_login, peer, list);
            }
        }

        private void OnPacketReceived(NetworkPacket packet)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnPacketReceived(packet)));
                return;
            }

            switch (packet.Action)
            {

                case "ReceiveMessage":
                    {
                        var msg = JsonSerializer.Deserialize<ChatMessage>(packet.Data);
                        if (msg == null) break;

                        var chatLogin = (msg.FromLogin ?? "").Trim().ToLowerInvariant();

                        if (!string.IsNullOrWhiteSpace(msg.MessageId))
                        {
                            _ = _client.SendAsync(new NetworkPacket
                            {
                                Action = "MsgDelivered",
                                Data = JsonSerializer.Serialize(new { MessageId = msg.MessageId, PeerLogin = msg.FromLogin })
                            });
                        }

                        ChatHistoryStore.Append(_login, chatLogin, new ChatHistoryItem
                        {
                            MessageId = msg.MessageId,
                            WithLogin = chatLogin,
                            isOutgoing = false,
                            FromLogin = msg.FromLogin,
                            FromName = msg.FromName,
                            Text = msg.Text,
                            Time = msg.Time,
                            isFile = false,
                            IsRead = false
                        });

                        var display = ResolveSenderName(msg.FromLogin, msg.FromName);

                        // Если открыт именно этот чат — показываем в UI
                        if (_activeChatLogin == chatLogin)
                        {
                            var name = ResolveSenderName(msg.FromLogin, msg.FromName);

                            AddTextBubble(msg.Text, isMine: false, time: msg.Time, name: name);
                            MarkIncomingAsReadAndNotifyServer();
                        }
                        
                        break;
                    }

                        
                    


                case "RequestsList":
                    var requests = JsonSerializer.Deserialize<List<string>>(packet.Data);
                    lstRequests.Items.Clear();
                    if (requests != null)
                        foreach (var r in requests)
                            lstRequests.Items.Add(r);
                    break;

                case "ContactRequestResult":
                    MessageBox.Show(packet.Data == "OK"
                        ? "Request sent"
                        : "Failed to send request");
                    break;

                case "AcceptResult":
                    MessageBox.Show(packet.Data == "OK"
                        ? "Contact accepted"
                        : "Failed to accept");
                    break;

                case "DeclineResult":
                    MessageBox.Show(packet.Data == "OK"
                        ? "Request declined"
                        : "Failed to decline");
                    break;

                case "RemoveResult":
                    MessageBox.Show(packet.Data == "OK"
                        ? "Contact removed"
                        : "Failed to remove");
                    break;
                case "ReceiveFile":
                    var fileMsg = JsonSerializer.Deserialize<FileMessage>(packet.Data);
                    if (fileMsg == null) break;

                    if (!string.IsNullOrWhiteSpace(fileMsg.MessageId))
                    {
                        _ = _client.SendAsync(new NetworkPacket
                        {
                            Action = "MsgDelivered",
                            Data = JsonSerializer.Serialize(new Client.Core.MsgStatusDto
                            {
                                MessageId = fileMsg.MessageId,
                                PeerLogin = fileMsg.FromLogin // отправитель файла
                            })
                        });
                    }

                    var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "MessengerFiles");
                    Directory.CreateDirectory(directory);

                    var safeName = $"{fileMsg.Time:yyyyMMdd_HHmmss}_{fileMsg.FromLogin}_{fileMsg.FileName}";
                    foreach (var c in Path.GetInvalidFileNameChars())
                    {
                        safeName = safeName.Replace(c, '_');
                    }

                    var savePath = Path.Combine(directory, safeName);

                    var bytes = Convert.FromBase64String(fileMsg.Base64);
                    File.WriteAllBytes(savePath, bytes);

                    var withLogin = fileMsg.FromLogin.Trim().ToLowerInvariant();

                    ChatHistoryStore.Append(_login, withLogin, new ChatHistoryItem
                    {
                        MessageId = fileMsg.MessageId,
                        WithLogin = withLogin,
                        isOutgoing = false,
                        FromLogin = fileMsg.FromLogin,
                        FromName = fileMsg.FromName,
                        Time = fileMsg.Time,
                        isFile = true,
                        FileName = fileMsg.FileName,
                        SavedPath = savePath,
                        SizeBytes = fileMsg.SizeBytes,
                        IsRead = false
                    });

                    if (_activeChatLogin == withLogin)
                    {
                        var name = ResolveSenderName(fileMsg.FromLogin, fileMsg.FromName);

                        AddFileBubble(fileMsg.FileName, fileMsg.SizeBytes, savePath, isMine: false, time: fileMsg.Time, name: name);

                    }

                    Logger.Log($"Received file from {fileMsg.FromName}: {fileMsg.FileName} saved to {savePath}", Logger.LogLevel.Success);

                    var result = MessageBox.Show($"Received file '{fileMsg.FileName}' from {fileMsg.FromName}. Do you want to open it?", "File Received", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try { Process.Start(new ProcessStartInfo(savePath) { UseShellExecute = true }); }

                        catch { }
                    }
                    break;
                case "SendFileResult":
                    if (packet.Data == "OK") MessageBox.Show("File sent!");
                    else if (packet.Data == "USER_OFFLINE") MessageBox.Show("User is offline.");
                    else if (packet.Data == "TOO_LARGE") MessageBox.Show("File too large.");
                    else MessageBox.Show("File send failed.");
                    break;
                case "SendMessageResult":
                    if (packet.Data == "USER_OFFLINE")
                        MessageBox.Show("User is offline.");
                    break;
                case "Profile":
                    var profile = JsonSerializer.Deserialize<ProfileDto>(packet.Data);
                    if (profile != null)
                    {
                        lblCurrentUser.Text = $"Current User: {profile.UserName} (@{profile.Login})";
                        txtMyNick.Text = profile.UserName;
                    }
                    break;
                case "ContactsList":
                    var contacts = JsonSerializer.Deserialize<List<ContactViewDto>>(packet.Data);
                    _contactsCache = contacts ?? new List<ContactViewDto>();

                    lstContacts.Items.Clear();
                    foreach (var c in _contactsCache)
                        lstContacts.Items.Add(c);

                    break;
                case "UpdateAliasResult":
                    if (packet.Data != "OK") MessageBox.Show("Rename Failed.");
                    Logger.Log($"Alias update result: {packet.Data}", Logger.LogLevel.Info);
                    break;
                case "UpdateProfileResult":
                    if (packet.Data == "OK") MessageBox.Show("Nickname updated!");
                    else MessageBox.Show("Nickname update failed.");
                    break;
                case "PresenceData":
                    var map = JsonSerializer.Deserialize<Dictionary<string, bool>>(packet.Data);
                    if (map != null)
                    {
                        _presence = map;
                        
                    }
                    UpdatePresenceUi();
                    lstContacts.Invalidate();
                    break;
                case "Typing":
                    var dto = JsonSerializer.Deserialize<TypingIncomingDto>(packet.Data);
                    if (dto == null) break;

                    var from = dto.FromLogin.Trim().ToLowerInvariant();

                    if (_activeChatLogin == from)
                    {
                        lblTyping.Text = dto.IsTyping ? "typing..." : "";
                        lblTyping.Visible = dto.IsTyping;
                        lblTyping.ForeColor = Color.Gray;
                    }
                    break;
                case "MessageStatus":
                    {
                        var st = JsonSerializer.Deserialize<MessageStatusIncoming>(packet.Data);
                        if (st == null) break;

                        var peer = (st.PeerLogin ?? "").Trim().ToLowerInvariant();
                        var status = (st.Status ?? "").Trim();

                        if (string.IsNullOrWhiteSpace(peer) || string.IsNullOrWhiteSpace(st.MessageId))
                            break;

                        UpdateOutgoingStatusHistory(peer, st.MessageId, status);

                        if (_activeChatLogin != null && _activeChatLogin.Trim().ToLowerInvariant() == peer)
                            OpenChat(peer);

                        break;
                    }
            }
        }

        private void UpdatePresenceUi()
        {
            if(_activeChatLogin == null) return;

            var key = _activeChatLogin.Trim().ToLowerInvariant();
            var online = _presence.TryGetValue(key, out var isOn) && isOn;

            lblStatus.Text = online ? "● Online" : "● Offline";
            lblStatus.ForeColor = online ? Color.LimeGreen : Color.Gray;
        }

        private string ResolveSenderName(string fromLogin, string fallbackName)
        {
            var key = (fromLogin ?? "").Trim().ToLowerInvariant();

            var c = _contactsCache.FirstOrDefault(x =>
       (x.Login ?? "").Trim().ToLowerInvariant() == key);

            // Если ты переименовал контакт (alias) — показываем alias
            if (c != null && !string.IsNullOrWhiteSpace(c.Alias))
                return c.Alias;

            // иначе показываем его ник (FromName), который прислал сервер
            return string.IsNullOrWhiteSpace(fallbackName) ? fromLogin : fallbackName;
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("No server connection");
                return;
            }

            var to = txtTo.Text.Trim();
            if (string.IsNullOrWhiteSpace(to) && lstContacts.SelectedItem is ContactViewDto c)
                to = c.Login;

            var text = txtMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(to))
            {
                MessageBox.Show("Select a contact or type username in 'Send to'.");
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Message is empty.");
                return;
            }

            var mid = Guid.NewGuid().ToString();
            var message = new ChatMessage
            {
                MessageId = mid,
                FromLogin = _login,
                To = to,
                Text = text,
                Time = DateTime.Now
            };

            await _client.SendAsync(new NetworkPacket
            {
                Action = "SendMessage",
                Data = JsonSerializer.Serialize(message)
            });

            //lstMessage.Items.Add($"[{message.Time:HH:mm}] Me: {text}");
            txtMessage.Clear();

            Logger.Log($"Sent message to {to}: {text}", Logger.LogLevel.Message);

            var withLogin = to.Trim().ToLowerInvariant();
            ChatHistoryStore.Append(_login, withLogin, new ChatHistoryItem
            {
                MessageId=mid,
                WithLogin = withLogin,
                isOutgoing = true,
                FromLogin = _login,
                FromName = "Me",
                Text = text,
                Time = message.Time,
                isFile = false,
                Status = "Sent"
            });

            if(_activeChatLogin == withLogin)
            {
                AddTextBubble(text, isMine: true, time: message.Time, name: "Me");

            }

            _typingStopTimer.Stop();
            _isTypingSent = false;
            await SendTypingAsync(false);
        }

        private async void btnSendRequest_Click(object sender, EventArgs e)
        {
            string targetUser = txtAddContact.Text.Trim();
            if (string.IsNullOrEmpty(targetUser))
            {
                MessageBox.Show("Enter a username to send request.");
                return;
            }

            var packet = new NetworkPacket
            {
                Action = "SendContactRequest",
                Data = targetUser
            };

            await _client.SendAsync(packet);
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedItem == null) return;

            var packet = new NetworkPacket
            {
                Action = "AcceptContact",
                Data = lstRequests.SelectedItem.ToString()
            };

            await _client.SendAsync(packet);

            await _client.SendAsync(new NetworkPacket { Action = "GetContacts" });
            await _client.SendAsync(new NetworkPacket { Action = "GetRequests" });
        }

        private async void btnDecline_Click(object sender, EventArgs e)
        {
            if (lstRequests.SelectedItem == null) return;

            var packet = new NetworkPacket
            {
                Action = "DeclineContact",
                Data = lstRequests.SelectedItem.ToString()
            };

            await _client.SendAsync(packet);

            await _client.SendAsync(new NetworkPacket { Action = "GetRequests" });
        }

        private async void btnRemoveContact_Click(object sender, EventArgs e)
        {
            if (lstContacts.SelectedItem is not ContactViewDto c) return;

            await _client.SendAsync(new NetworkPacket
            {
                Action = "RemoveContact",
                Data = c.Login
            });

            await _client.SendAsync(new NetworkPacket { Action = "GetContacts" });
        }

        private void lstContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstContacts.SelectedItem is ContactViewDto c)
            {
                txtTo.Text = c.Login;
                txtAlias.Text = c.Alias ?? "";

                OpenChat(c.Login);
            }

            
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Log($"User {_login} clicked Logout", Logger.LogLevel.Warning);
                if (_client.IsConnected)
                {
                    await _client.SendAsync(new NetworkPacket { Action = "Logout" });
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Logout send error: {ex.Message}", Logger.LogLevel.Error);
            }
            finally
            {
                _client.Disconnect();
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Logger.Log($"MainForm closing for {_login}", Logger.LogLevel.Warning);

                if (_client.IsConnected)
                    _ = _client.SendAsync(new NetworkPacket { Action = "Logout" });
            }
            catch { }

            _client.Disconnect();
            base.OnFormClosing(e);
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("No server connection");
                return;
            }

            var to = txtTo.Text.Trim();
            if (string.IsNullOrWhiteSpace(to) && lstContacts.SelectedItem is ContactViewDto c)
                to = c.Login;

            to = (to ?? "").Trim();
            if (string.IsNullOrWhiteSpace(to))
            {
                MessageBox.Show("Select a contact or type username in 'Send to'.");
                return;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "Select a file to send",
                Filter = "All files (*.*)|*.*"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            var path = ofd.FileName;

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch
            {
                MessageBox.Show("Cannot read file.");
                return;
            }

            const int maxBytes = 10 * 1024 * 1024;
            if (bytes.Length > maxBytes)
            {
                MessageBox.Show("File is too large (max 10 MB).");
                return;
            }

            var fileMsg = new FileMessage
            {
                // важно: по твоей серверной логике он сам проставит FromLogin/FromName
                FromName = _login, // можно оставить, сервер все равно перезапишет/нормализует
                To = to,
                FileName = Path.GetFileName(path),
                ContentType = "application/octet-stream",
                Base64 = Convert.ToBase64String(bytes),
                SizeBytes = bytes.Length,
                Time = DateTime.Now
            };

            await _client.SendAsync(new NetworkPacket
            {
                Action = "SendFile",
                Data = JsonSerializer.Serialize(fileMsg)
            });

            // ---- UI + история (как будто "отправлено")
            var withLogin = to.Trim().ToLowerInvariant();

            ChatHistoryStore.Append(_login, withLogin, new ChatHistoryItem
            {
                WithLogin = withLogin,
                isOutgoing = true,
                FromLogin = _login,
                FromName = "Me",
                Time = fileMsg.Time,
                isFile = true,
                FileName = fileMsg.FileName,
                SavedPath = path // у отправителя файл лежит тут; bubble сможет "открыть"
                                 // если у ChatHistoryItem есть SizeBytes - добавь тоже
            });

            if (_activeChatLogin == withLogin)
            {
                AddFileBubble(fileMsg.FileName, fileMsg.SizeBytes, path, isMine: true, time: fileMsg.Time, name: "Me");

            }

            Logger.Log($"Sent file to {to}: {fileMsg.FileName} ({fileMsg.SizeBytes} bytes)", Logger.LogLevel.Message);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private async void btnRenameContact_Click(object sender, EventArgs e)
        {
            if (lstContacts.SelectedItem is not Client.Core.ContactViewDto c) return;

            var alias = txtAlias.Text.Trim();

            var packet = new NetworkPacket
            {
                Action = "UpdateContactAlias",
                Data = JsonSerializer.Serialize(new { ContactLogin = c.Login, Alias = alias })
            };

            await _client.SendAsync(packet);
        }

        private async void btnSaveMyNick_Click(object sender, EventArgs e)
        {
            var nick = txtMyNick.Text.Trim();
            if (string.IsNullOrWhiteSpace(nick))
            {
                MessageBox.Show("Nickname cannot be empty.");
                return;
            }

            await _client.SendAsync(new NetworkPacket {
                Action = "UpdateProfileName",
                Data = JsonSerializer.Serialize(new { UserName = nick })
            });
        }
    }
}