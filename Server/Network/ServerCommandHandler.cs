using Server.Core;
using Server.Network;
using System;
using System.Collections.Generic;

public class ServerCommandHandler
{
    private readonly HashSet<string> _bannedUsers = new();
    public void Start()
    {
        Logger.Log("Command console started. Type '/help' for commands.", Logger.LogLevel.Info);

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            var parts = input.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].ToLower();

            switch (command)
            {
                case "/stop":
                    Logger.Log("Stopping server...", Logger.LogLevel.Warning);
                    Environment.Exit(0);
                    break;

                case "/list":
                    var clients = ClientHandler.ConnectedClients;
                    if (clients.Count == 0)
                        Logger.Log("No users connected.", Logger.LogLevel.Info);
                    else
                        foreach (var user in clients.Keys)
                            Logger.Log($"Connected user: {user}", Logger.LogLevel.Info);
                    break;

                case "/ban":
                    if (parts.Length < 2)
                    {
                        Logger.Log("Usage: ban <login>", Logger.LogLevel.Warning);
                        break;
                    }
                    var loginToBan = parts[1];
                    if (ClientHandler.ConnectedClients.TryGetValue(loginToBan, out var handler))
                    {
                        handler.Disconnect();
                        Logger.Log($"User {loginToBan} has been banned and disconnected.", Logger.LogLevel.Warning);
                        _bannedUsers.Add(loginToBan);
                    }
                    else
                        Logger.Log($"User {loginToBan} not found.", Logger.LogLevel.Warning);
                    break;

                case "/msg":
                    if (parts.Length < 3)
                    {
                        Logger.Log("Usage: msg <login> <text>", Logger.LogLevel.Warning);
                        break;
                    }
                    var targetUser = parts[1];
                    var msgText = parts[2];
                    if (ClientHandler.ConnectedClients.TryGetValue(targetUser, out var targetHandler))
                    {
                        targetHandler.SendSystemMessage(msgText);
                        Logger.Log($"Sent message to {targetUser}: {msgText}", Logger.LogLevel.Message);
                    }
                    else
                        Logger.Log($"User {targetUser} not found.", Logger.LogLevel.Warning);
                    break;

                case "/broadcast":
                    if (parts.Length < 2)
                    {
                        Logger.Log("Usage: broadcast <text>", Logger.LogLevel.Warning);
                        break;
                    }
                    var broadcastText = input.Substring(command.Length + 1);
                    foreach (var client in ClientHandler.ConnectedClients.Values)
                        client.SendSystemMessage(broadcastText);
                    Logger.Log($"Broadcast message: {broadcastText}", Logger.LogLevel.Message);
                    break;

                case "/help":
                    Logger.Log("Commands:\n/stop - stop server\n/list - list users\n/ban <login> - ban user\n/msg <login> <text> - send message\n/broadcast <text> - send to all\n/clear - clear console", Logger.LogLevel.Info);
                    break;

                case "/clear":
                    Console.Clear();
                    break;

                default:
                    Logger.Log("Unknown command. Type 'help' for commands.", Logger.LogLevel.Warning);
                    break;
            }
        }
    }

    public bool IsBanned(string login) => _bannedUsers.Contains(login);
}
