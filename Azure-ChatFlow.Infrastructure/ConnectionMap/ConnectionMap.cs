namespace AzureChatFlow.Infrastructure.ConnectionMap
{
    public class ConnectionMap
    {
        private readonly Dictionary<string, string> _map = new();

        public void Set(string userId, string connectionId) => _map[userId] = connectionId;
        public string? Get(string userId) => _map.TryGetValue(userId, out var id) ? id : null;
        public void Delete(string userId) => _map.Remove(userId);
        public int Count => _map.Count;
    }
}
