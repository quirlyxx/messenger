using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;

namespace Client.Core
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Success = 2,
            Message = 3,
            Warning = 4,
            Error = 5,
        }

        private class LogSettings
        {
            public bool Enabled { get; set; } = true;
            public string MinLeveled { get; set; } = "Info";
        }

        private static bool _enabled = true;
        private static LogLevel _minLevel = LogLevel.Info;
        private static bool _loaded;

        private static void EnsureLoaded()
        {
            if(_loaded) return;
            _loaded = true;

            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logsettings.json");
                if(!File.Exists(path)) return;

                var json = File.ReadAllText(path);
                var cfg = JsonSerializer.Deserialize<LogSettings>(json);

                if(cfg != null)
                {
                    _enabled = cfg.Enabled;
                    if(Enum.TryParse<LogLevel>(cfg.MinLeveled, true, out var lvl))
                    {
                        _minLevel = lvl ;
                    }
                }
            }
            catch { }
        }

        public static void Log(string text, LogLevel level = LogLevel.Info)
        {
            EnsureLoaded();
            if(!_enabled || level < _minLevel) return;
            lock (_lock)
            {
                var prev = Console.ForegroundColor;

                Console.ForegroundColor = level switch
                {
                    LogLevel.Debug => ConsoleColor.DarkGray,
                    LogLevel.Info => ConsoleColor.Gray,
                    LogLevel.Success => ConsoleColor.Green,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Message => ConsoleColor.Cyan,
                    _ => ConsoleColor.White
                };

                string prefix = level switch
                {
                    LogLevel.Debug => "[DEBUG]",
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
