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

            // Tạo index cho ProvinceId trong Districts
            var districtsCollection = _context.Districts;
            var provinceIdIndex = Builders<District>.IndexKeys.Ascending(d => d.ProvinceId);
            try
            {
                await districtsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<District>(provinceIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho DistrictId trong Wards
            var wardsCollection = _context.Wards;
            var districtIdIndex = Builders<Ward>.IndexKeys.Ascending(w => w.DistrictId);
            try
            {
                await wardsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<Ward>(districtIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho WardId trong Hamlets
            var hamletsCollection = _context.Hamlets;
            var wardIdIndex = Builders<Hamlet>.IndexKeys.Ascending(h => h.WardId);
            try
            {
                await hamletsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<Hamlet>(wardIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho HouseholdCode trong Households (unique)
            var householdsCollection = _context.Households;
            var householdCodeIndex = Builders<Household>.IndexKeys.Ascending(h => h.HouseholdCode);
            var householdCodeIndexOptions = new CreateIndexOptions { Unique = true };
            try
            {
                await householdsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<Household>(householdCodeIndex, householdCodeIndexOptions));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho HouseholdId trong Citizens
            var citizensCollection = _context.Citizens;
            var householdIdIndex = Builders<Citizen>.IndexKeys.Ascending(c => c.HouseholdId);
            try
            {
                await citizensCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<Citizen>(householdIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho CitizenId trong PopulationChanges
            var populationChangesCollection = _context.PopulationChanges;
            var citizenIdIndex = Builders<PopulationChange>.IndexKeys.Ascending(p => p.CitizenId);
            try
            {
                await populationChangesCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<PopulationChange>(citizenIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho CitizenId trong CitizenRequests
            var citizenRequestsCollection = _context.CitizenRequests;
            var citizenRequestCitizenIdIndex = Builders<CitizenRequest>.IndexKeys.Ascending(c => c.CitizenId);
            try
            {
                await citizenRequestsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<CitizenRequest>(citizenRequestCitizenIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }

            // Tạo index cho UserId trong AuditLogs
            var auditLogsCollection = _context.AuditLogs;
            var auditLogUserIdIndex = Builders<AuditLog>.IndexKeys.Ascending(a => a.UserId);
            try
            {
                await auditLogsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<AuditLog>(auditLogUserIdIndex));
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.Code == 85)
            {
                // Index đã tồn tại, bỏ qua
            }
        }
    }
}

