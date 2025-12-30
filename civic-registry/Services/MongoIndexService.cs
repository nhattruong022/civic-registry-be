using CivicRegistry.API.Models;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service để tạo indexes cho MongoDB collections
    /// </summary>
    public class MongoIndexService
    {
        private readonly MongoDbContext _context;

        public MongoIndexService(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo tất cả collections và indexes cần thiết
        /// </summary>
        public async Task CreateIndexesAsync()
        {
            // Collections sẽ được tạo tự động khi tạo indexes
            // Tạo unique index cho Username trong Users collection
            var usersCollection = _context.Users;
            var usernameIndex = Builders<User>.IndexKeys.Ascending(u => u.Username);
            var usernameIndexOptions = new CreateIndexOptions { Unique = true };
            try
            {
                await usersCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<User>(usernameIndex, usernameIndexOptions));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict")
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho HoKhauId trong NhanKhau collection
            var nhanKhauCollection = _context.NhanKhaus;
            var hoKhauIdIndex = Builders<NhanKhau>.IndexKeys.Ascending(n => n.HoKhauId);
            try
            {
                await nhanKhauCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<NhanKhau>(hoKhauIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho NhanKhauId trong các collection liên quan
            var tamTruTamVangCollection = _context.TamTruTamVangs;
            var tamTruTamVangIndex = Builders<TamTruTamVang>.IndexKeys.Ascending(t => t.NhanKhauId);
            try
            {
                await tamTruTamVangCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<TamTruTamVang>(tamTruTamVangIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            var khaiSinhCollection = _context.KhaiSinhs;
            var khaiSinhIndex = Builders<KhaiSinh>.IndexKeys.Ascending(k => k.NhanKhauId);
            try
            {
                await khaiSinhCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<KhaiSinh>(khaiSinhIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            var khaiTuCollection = _context.KhaiTus;
            var khaiTuIndex = Builders<KhaiTu>.IndexKeys.Ascending(k => k.NhanKhauId);
            try
            {
                await khaiTuCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<KhaiTu>(khaiTuIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho UserId trong LichSuThayDoi
            var lichSuThayDoiCollection = _context.LichSuThayDois;
            var userIdIndex = Builders<LichSuThayDoi>.IndexKeys.Ascending(l => l.UserId);
            try
            {
                await lichSuThayDoiCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<LichSuThayDoi>(userIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Đảm bảo các collections khác cũng được tạo
            // Collections sẽ được tạo tự động khi có document đầu tiên hoặc khi tạo index
            _ = _context.KhuVucs;
            _ = _context.HoKhaus;
        }
    }
}

