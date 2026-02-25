using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

namespace Client.Core
{
    public static class ChatHistoryStore
    {
        private static string BaseDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MessengerClient", "History");
        private static string Safe(string login)
        {
            foreach(var c in Path.GetInvalidFileNameChars())
            {
                login = login.Replace(c, '_');
            }
            return login.Trim().ToLowerInvariant();
        }

        private static string FilePath(string myLogin, string withLogin)
        {
            Directory.CreateDirectory(BaseDir);
            return Path.Combine(BaseDir, $"{Safe(myLogin)}__{Safe(withLogin)}.json");
        }

        public static List<ChatHistoryItem> Load(string myLogin, string withLogin)
        {
            var path = FilePath(myLogin, withLogin);
            if(!File.Exists(path)) return new List<ChatHistoryItem>();

            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<ChatHistoryItem>>(json) ?? new List<ChatHistoryItem>();
            }
            catch { 
                return new List<ChatHistoryItem>();
            }
        }

        public static void Append(string myLogin, string withLogin, ChatHistoryItem item)
        {
            var list = Load(myLogin, withLogin);
            list.Add(item);

            var path = FilePath(myLogin, withLogin);
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(path, json);
        }
    }
}
