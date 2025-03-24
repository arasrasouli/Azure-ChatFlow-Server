namespace AzureChatFlow.Common.Helper
{
    public static class StringHelper
    {
        public static string CreatePartitionKey(string user1, string user2)
        {
            if (string.IsNullOrEmpty(user1) || string.IsNullOrEmpty(user2))
            {
                throw new ArgumentException("User IDs cannot be null or empty.");
            }

            var users = new[] { user1, user2 };
            Array.Sort(users);
            return $"{users[0]}:{users[1]}";
        }

        public static (string user1, string user2) GetIdsFromPartitionKey(string partitionKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || !partitionKey.Contains("_"))
            {
                throw new ArgumentException("Invalid PartitionKey format. Expected 'user1_user2'.");
            }

            var parts = partitionKey.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("PartitionKey must contain exactly two user IDs separated by ':'.");
            }

            return (parts[0], parts[1]);
        }
    }
}