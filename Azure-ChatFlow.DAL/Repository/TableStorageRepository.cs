using Azure.Data.Tables;

namespace AzureChatFlow.DAL.Repositories
{
    public class TableStorageRepository<T> : ITableStorageRepository<T> where T : class, ITableEntity, new()
    {
        protected readonly TableClient _tableClient;

        public TableStorageRepository(TableClient tableClient)
        {
            _tableClient = tableClient;
            _tableClient.CreateIfNotExists();
        }

        public async Task AddEntityAsync(T entity)
        {
            await _tableClient.AddEntityAsync(entity);
        }

    }
}
