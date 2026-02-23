using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Core
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public enum LogLevel
        {
            Info,
            Success,
            Warning,
            Error,
            Message
        }

        public static void Log(string text, LogLevel level = LogLevel.Info)
        {
            lock (_lock)
            {
                ConsoleColor prev = Console.ForegroundColor;

                Console.ForegroundColor = level switch
                {
                    LogLevel.Info => ConsoleColor.Gray,
                    LogLevel.Success => ConsoleColor.Green,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Message => ConsoleColor.Cyan,
                    _ => ConsoleColor.White
                };

                string prefix = level switch
                {
                    LogLevel.Info => "[INFO]",
                    LogLevel.Success => "[SUCCESS]",
                    LogLevel.Warning => "[WARNING]",
                    LogLevel.Error => "[ERROR]",
                    LogLevel.Message => "[MESSAGE]",
                    _ => "[LOG]"
                };

                Console.WriteLine($"{DateTime.Now:HH:mm:ss} {prefix} {text}");
                Console.ForegroundColor = prev;
            }
        }
    }
}
