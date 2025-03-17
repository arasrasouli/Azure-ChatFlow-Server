using System.Collections.Generic;

namespace AzureChatFlow.Functions
{
    internal class ConnectionMap
    {
        private static readonly Dictionary<string, string> _connections = new();

        public static void AddOrUpdate(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public static string GetConnectionId(string userId) => _connections.GetValueOrDefault(userId);

        public static void Remove(string userId) => _connections.Remove(userId);

        public static void Set(string key, string value)
        {
            _connections[key] = value;
        }

        public static string Get(string key)
        {
            return _connections.TryGetValue(key, out var value) ? value : null;
        }

        public static void Delete(string key)
        {
            _connections.Remove(key);
        }

        public static bool Exists(string key)
        {
            return _connections.ContainsKey(key);
        }

        public static void Clear()
        {
            _connections.Clear();
        }

        public static IEnumerable<string> GetAllKeys()
        {
            return _connections.Keys;
        }
    }
}