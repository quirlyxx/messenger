using System;
using System.Windows.Forms;
using Client.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Network;
using System.IO;
using System.Diagnostics;


namespace Client.Forms
{
    public partial class MainForm : Form
    {
        private readonly NetworkClient _client;
        private readonly string _login;

        public MainForm(NetworkClient client, string login)
        {
            InitializeComponent();
            _client = client;
            _login = login;
            lblCurrentUser.Text = $"Current User: {login}";
            _client.OnPacketReceived += OnPacketReceived;
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            await _client.SendAsync(new NetworkPacket { Action = "GetContacts" });
            await _client.SendAsync(new NetworkPacket { Action = "GetRequests" });
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
                    var msg = JsonSerializer.Deserialize<ChatMessage>(packet.Data);
                    if (msg != null)
                        lstMessage.Items.Add($"{msg.Time:HH:mm} {msg.From}: {msg.Text}");
                    break;

                case "ContactsList":
                    var contacts = JsonSerializer.Deserialize<List<string>>(packet.Data);
                    lstContacts.Items.Clear();
                    if (contacts != null)
                        foreach (var c in contacts)
                            lstContacts.Items.Add(c);
                    break;

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
                    if(fileMsg == null) break;
                    var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "MessengerFiles");
                    Directory.CreateDirectory(directory);

                    var safeName = $"{fileMsg.Time:yyyyMMdd_HHmmss}_{fileMsg.From}_{fileMsg.FileName}";
                    foreach(var c in Path.GetInvalidFileNameChars())
                    {
                        safeName = safeName.Replace(c, '_');
                    }

                    var savePath = Path.Combine(directory, safeName);

                    var bytes = Convert.FromBase64String(fileMsg.Base64);
                    File.WriteAllBytes(savePath, bytes);

                    lstMessage.Items.Add($"[{fileMsg.Time:HH:mm}] {fileMsg.From}: [File: {fileMsg.FileName}] saved → {savePath}");

                    Logger.Log($"Received file from {fileMsg.From}: {fileMsg.FileName} saved to {savePath}", Logger.LogLevel.Success);

                    var result = MessageBox.Show($"Received file '{fileMsg.FileName}' from {fileMsg.From}. Do you want to open it?", "File Received", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(result == DialogResult.Yes)
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
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("No server connection");
                return;
            }

            var to = txtTo.Text.Trim();
            if (string.IsNullOrWhiteSpace(to) && lstContacts.SelectedItem != null)
                to = lstContacts.SelectedItem.ToString()!.Trim();

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

            var message = new ChatMessage
            {
                From = _login,
                To = to,
                Text = text,
                Time = DateTime.Now
            };

            await _client.SendAsync(new NetworkPacket
            {
                Action = "SendMessage",
                Data = JsonSerializer.Serialize(message)
            });

            lstMessage.Items.Add($"[{message.Time:HH:mm}] {to}: {text}");
            txtMessage.Clear();

            Logger.Log($"Sent message to {to}: {text}", Logger.LogLevel.Message);
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
            if (lstContacts.SelectedItem == null) return;

            await _client.SendAsync(new NetworkPacket
            {
                Action = "RemoveContact",
                Data = lstContacts.SelectedItem.ToString()
            });

            await _client.SendAsync(new NetworkPacket { Action = "GetContacts" });
        }

        private void lstContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstContacts.SelectedItem != null)
                txtTo.Text = lstContacts.SelectedItem.ToString();
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

                var loginForm = new LoginForm();
                loginForm.Show();

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
            if (string.IsNullOrWhiteSpace(to) && lstContacts.SelectedItem != null)
                to = lstContacts.SelectedItem.ToString()!.Trim();

            if (string.IsNullOrWhiteSpace(to))
            {
                MessageBox.Show("Select a contact or type username in 'Send to'.");
                return;
            }

            using var ofd = new OpenFileDialog();
            ofd.Title = "Select a file to send";
            ofd.Filter = "All files (*.*)|*.*";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            var path = ofd.FileName;
            var bytes = File.ReadAllBytes(path);

            if(bytes.Length > 10 * 1024 * 1024)
            {
                MessageBox.Show("File is too large (max 10 MB).");
                return;
            }

            var fileMsg = new FileMessage
            {
                From = _login,
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

            lstMessage.Items.Add($"[{fileMsg.Time:HH:mm}] {to}: [File: {fileMsg.FileName} ({fileMsg.SizeBytes / 1024} KB)]");
            Logger.Log($"Sent file to {to}: {fileMsg.FileName} ({fileMsg.SizeBytes} bytes)", Logger.LogLevel.Message);
        }
    }
}