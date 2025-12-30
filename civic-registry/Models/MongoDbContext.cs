using MongoDB.Driver;
using System.Reflection;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// MongoDB Context để quản lý kết nối và collections
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Lấy collection theo tên
        /// </summary>
        public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = GetCollectionName<T>();
            }
            return _database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Lấy tên collection từ attribute hoặc tên class
        /// </summary>
        private string GetCollectionName<T>()
        {
            var attribute = typeof(T).GetCustomAttribute<BsonCollectionAttribute>();
            return attribute?.CollectionName ?? typeof(T).Name;
        }

        // Collections
        public IMongoCollection<User> Users => GetCollection<User>();
        public IMongoCollection<Province> Provinces => GetCollection<Province>();
        public IMongoCollection<District> Districts => GetCollection<District>();
        public IMongoCollection<Ward> Wards => GetCollection<Ward>();
        public IMongoCollection<Hamlet> Hamlets => GetCollection<Hamlet>();
        public IMongoCollection<Household> Households => GetCollection<Household>();
        public IMongoCollection<Citizen> Citizens => GetCollection<Citizen>();
        public IMongoCollection<PopulationChange> PopulationChanges => GetCollection<PopulationChange>();
        public IMongoCollection<CitizenRequest> CitizenRequests => GetCollection<CitizenRequest>();
        public IMongoCollection<AuditLog> AuditLogs => GetCollection<AuditLog>();
    }
}

