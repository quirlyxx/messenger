using Server.Core;
using Server.Core.Services;
using Server.Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Network
{
    public class ServerP
    {
        private TcpListener? _listener;
        private readonly AuthService _authService = new AuthService();
        private bool _isRunning = false;

        public async Task StartAsync(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _isRunning = true;
            Logger.Log($"Server started on 0.0.0.0:{port}", Logger.LogLevel.Success);

            _ = Task.Run(() =>
            {
                var cmd = new ServerCommandHandler();
                cmd.Start();
            });

            await AcceptClientsAsync();
        }

        private async Task AcceptClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var tcpClient = await _listener!.AcceptTcpClientAsync();
                    Logger.Log("New client connected", Logger.LogLevel.Info);

                    var handler = new ClientHandler(tcpClient, _authService);
                    _ = handler.ProcessAsync(); 
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error accepting client: {ex.Message}", Logger.LogLevel.Error);
                }
            }
        }

        public void Stop()
        {
            Logger.Log("Stopping server...", Logger.LogLevel.Warning);
            _isRunning = false;

            // Отключаем всех клиентов
            foreach (var client in ClientHandler.ConnectedClients.Values)
            {
                client.Disconnect();
            }

            _listener?.Stop();
            Logger.Log("Server stopped", Logger.LogLevel.Warning);
        }
    }
}