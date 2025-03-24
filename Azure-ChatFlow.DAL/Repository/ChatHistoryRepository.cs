using Azure.Data.Tables;
using AzureChatFlow.DAL.Entities;

namespace AzureChatFlow.DAL.Repositories
{
    public class ChatHistoryRepository : TableStorageRepository<ChatHistoryEntity>, IChatHistoryRepository
    {
        public ChatHistoryRepository(TableClient tableClient)
            : base(tableClient)
        {
        }

        public async Task<List<ChatHistoryEntity>> GetEntitiesByPartitionAsync(string partitionKey, int? maxResults = null)
        {
            var query = _tableClient.QueryAsync<ChatHistoryEntity>(e => e.PartitionKey == partitionKey);

            List<ChatHistoryEntity> results = new List<ChatHistoryEntity>();

            await foreach (var entity in query)
            {
                results.Add(entity);
            }

            return results.OrderByDescending(e => e.RowKey)
                          .Take(maxResults ?? int.MaxValue)
                          .ToList();
        }
    }
}
