using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

namespace IOU.Common.Azure
{
    public class TableStorageRepository : BaseStorageRepository
    {
        #region Fields

        private readonly CloudTableClient _tableClient;

        #endregion

        #region Constructors

        public TableStorageRepository(string connectionString) : base(connectionString)
        {
            _tableClient = _storageAccount.CreateCloudTableClient();
        }

        #endregion

        public async Task InsertEntityAsync<T>(T entity) where T : ITableEntity
        {
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task UpdateEntityAsync<T>(T entity) where T : ITableEntity
        {
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();

            var updateOperation = TableOperation.Replace(entity);

            await table.ExecuteAsync(updateOperation);
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>() where T : ITableEntity, new()
        {
            TableQuerySegment<T> querySegment = null;
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();
            var entities = new List<T>();
            var query = new TableQuery<T>();

            do
            {
                querySegment = await table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                entities.AddRange(querySegment.Results);
            } while (querySegment.ContinuationToken != null);

            return entities;
        }

        public async Task<T> GetEntityByPartitionKeyRowKeyAsync<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            return (T)(await table.ExecuteAsync(operation)).Result;
        }

        public async Task<IEnumerable<T>> GetEntitiesByPartitionKeyAsync<T>(string partitionKey) where T : ITableEntity, new()
        {
            TableQuerySegment<T> querySegment = null;
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();
            var entities = new List<T>();
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.Equal, partitionKey));

            do
            {
                querySegment = await table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken);
                entities.AddRange(querySegment.Results);
            } while (querySegment.ContinuationToken != null);

            return entities;
        }

        public async Task DeleteEntityAsync<T>(T entity) where T : ITableEntity
        {
            CloudTable table = _tableClient.GetTableReference(typeof(T).Name.ToLower());
            await table.CreateIfNotExistsAsync();

            var deleteOperation = TableOperation.Delete(entity);

            await table.ExecuteAsync(deleteOperation);
        }
    }
}