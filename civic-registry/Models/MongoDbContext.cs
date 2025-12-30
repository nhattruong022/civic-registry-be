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
        public IMongoCollection<KhuVuc> KhuVucs => GetCollection<KhuVuc>();
        public IMongoCollection<HoKhau> HoKhaus => GetCollection<HoKhau>();
        public IMongoCollection<NhanKhau> NhanKhaus => GetCollection<NhanKhau>();
        public IMongoCollection<TamTruTamVang> TamTruTamVangs => GetCollection<TamTruTamVang>();
        public IMongoCollection<KhaiSinh> KhaiSinhs => GetCollection<KhaiSinh>();
        public IMongoCollection<KhaiTu> KhaiTus => GetCollection<KhaiTu>();
        public IMongoCollection<LichSuThayDoi> LichSuThayDois => GetCollection<LichSuThayDoi>();
    }
}

