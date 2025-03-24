using AzureChatFlow.DAL.Entities;

namespace AzureChatFlow.DAL.Repositories
{
    public interface IChatHistoryRepository : ITableStorageRepository<ChatHistoryEntity>
    {
        Task<List<ChatHistoryEntity>> GetEntitiesByPartitionAsync(string partitionKey, int? maxResults = null);
    }
}