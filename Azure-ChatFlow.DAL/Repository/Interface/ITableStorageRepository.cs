using Azure.Data.Tables;

namespace AzureChatFlow.DAL.Repositories
{
    public interface ITableStorageRepository<T> where T : class, ITableEntity, new()
    {
        Task AddEntityAsync(T entity);
    }
}