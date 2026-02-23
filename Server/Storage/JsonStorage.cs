using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Server.Core.Models;

namespace Server.Storage
{
    public static class JsonStorage
    {
        private const string FileName = "users.json";

        public static List<User> Load()
        {
            if (!File.Exists(FileName)) return new List<User>();
            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void Save(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}