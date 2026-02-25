using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Client.Core;

namespace Client.Network
{
    public class NetworkClient
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private CancellationTokenSource? _cts;

        public event Action<NetworkPacket>? OnPacketReceived;

        public bool IsConnected => _client?.Connected ?? false;

        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            var stream = _client.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            _cts = new CancellationTokenSource();

            _ = ListenAsync(_cts.Token);

            Logger.Log($"Connected to server at {ip}:{port}", Logger.LogLevel.Success);
        }

        public async Task SendAsync(NetworkPacket packet)
        {
            if (_writer == null) return;

            string json = JsonSerializer.Serialize(packet);
            try
            {
                await _writer.WriteLineAsync(json);
                Logger.Log($"Sent: {packet.Action} -> {packet.Data}", Logger.LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"SendAsync error: {ex.Message}", Logger.LogLevel.Error);
            }
        }

        private async Task ListenAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var line = await _reader!.ReadLineAsync();
                    if (line == null) break;

                    var packet = JsonSerializer.Deserialize<NetworkPacket>(line);
                    if (packet != null)
                    {
                        OnPacketReceived?.Invoke(packet);
                        Logger.Log($"Received: {packet.Action} -> {packet.Data}", Logger.LogLevel.Info);
                    }
                    if (line == null)
                    {
                        Logger.Log("Server closed connection", Logger.LogLevel.Warning);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ListenAsync error: {ex.Message}", Logger.LogLevel.Error);
            }
        }

        public void Disconnect()
        {
            try
            {
                _cts?.Cancel();
                _reader?.Dispose();
                _writer?.Dispose();
                _client?.Close();
                Logger.Log("Disconnected from server", Logger.LogLevel.Warning);
            }
            catch { }
        }
    }
}