using System;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using System.Windows.Forms;
using Client.Core;
using Client.Network;

namespace Client.Forms
{
    public partial class LoginForm : Form
    {
        public event Action<NetworkClient, string>? LoginSucceeded;
        private readonly NetworkClient _client = new NetworkClient();

        public LoginForm()
        {
            InitializeComponent();
            _client.OnPacketReceived += HandlePacket;
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                await _client.ConnectAsync("127.0.0.1", 8888);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect to server.");
                Logger.Log($"Connection error: {ex.Message}", Logger.LogLevel.Error);
            }
        }

        private void HandlePacket(NetworkPacket packet)
        {
            this.Invoke(() =>
            {
                switch (packet.Action)
                {
                    case "RegisterResult":
                        if (packet.Data == "OK")
                        {
                            MessageBox.Show("Registration successful!");
                            Logger.Log("Registration successful", Logger.LogLevel.Success);
                        }
                        else if (packet.Data == "EXISTS")
                        {
                            MessageBox.Show("User already exists.");
                        }
                        else
                        {
                            MessageBox.Show("Registration failed.");
                        }
                        break;

                    case "LoginResult":
                        if (packet.Data == "OK")
                        {
                            
                            Logger.Log("Login successful", Logger.LogLevel.Success);

                            // чтобы не было повторных вызовов/гонок
                            _client.OnPacketReceived -= HandlePacket;

                            var login = txtLogin.Text.Trim();
                            LoginSucceeded?.Invoke(_client, login);
                        }
                        else if (packet.Data == "BANNED")
                        {
                            MessageBox.Show("You are banned.");
                        }
                        else
                        {
                            MessageBox.Show("Incorrect login or password.");
                        }
                        break;
                }
            });
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter login and password.");
                Logger.Log("Empty login/password on registration", Logger.LogLevel.Warning);
                return;
            }

            var packet = new NetworkPacket
            {
                Action = "Register",
                Data = JsonSerializer.Serialize(new { Login = login, Password = password })
            };

            _ = _client.SendAsync(packet);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter login and password.");
                Logger.Log("Empty login/password on login", Logger.LogLevel.Warning);
                return;
            }

            var packet = new NetworkPacket
            {
                Action = "Login",
                Data = JsonSerializer.Serialize(new { Login = login, Password = password })
            };

            _ = _client.SendAsync(packet);
        }
    }
}