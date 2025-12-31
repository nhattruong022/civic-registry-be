using CivicRegistry.API.Models;
using CivicRegistry.API.DTOs;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service xử lý thống kê
    /// </summary>
    public class StatisticsService
    {
        private readonly MongoDbContext _context;

        public StatisticsService(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thống kê tỉnh
        /// </summary>
        /// <param name="provinceId">ID tỉnh (optional, nếu null thì lấy tất cả)</param>
        public async Task<ProvinceStatisticsResponse> GetProvinceStatisticsAsync(int? provinceId = null)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Đếm tổng số hộ khẩu
            FilterDefinition<Household> householdFilter;
            if (provinceId.HasValue)
            {
                // Lấy danh sách huyện thuộc tỉnh
                // Chuyển đổi int sang string để so sánh với ProvinceId (string)
                var districtIds = await _context.Districts
                    .Find(d => d.ProvinceId == provinceId.Value.ToString())
                    .Project(d => d.Id)
                    .ToListAsync();

                // Lấy danh sách xã thuộc các huyện
                var wardIds = await _context.Wards
                    .Find(w => districtIds.Contains(w.DistrictId))
                    .Project(w => w.Id)
                    .ToListAsync();

                // Lấy danh sách thôn thuộc các xã
                var hamletIds = await _context.Hamlets
                    .Find(h => wardIds.Contains(h.WardId))
                    .Project(h => h.Id)
                    .ToListAsync();

                // Đếm hộ khẩu thuộc các thôn
                householdFilter = Builders<Household>.Filter.In(h => h.HamletId, hamletIds);
            }
            else
            {
                householdFilter = Builders<Household>.Filter.Empty;
            }

            var totalHouseholds = await _context.Households
                .CountDocumentsAsync(householdFilter);

            // Đếm tổng số nhân khẩu
            FilterDefinition<Citizen> citizenFilter;
            if (provinceId.HasValue)
            {
                // Lấy danh sách hộ khẩu thuộc tỉnh
                // Chuyển đổi int sang string để so sánh với ProvinceId (string)
                var districtIds = await _context.Districts
                    .Find(d => d.ProvinceId == provinceId.Value.ToString())
                    .Project(d => d.Id)
                    .ToListAsync();

                var wardIds = await _context.Wards
                    .Find(w => districtIds.Contains(w.DistrictId))
                    .Project(w => w.Id)
                    .ToListAsync();

                var hamletIds = await _context.Hamlets
                    .Find(h => wardIds.Contains(h.WardId))
                    .Project(h => h.Id)
                    .ToListAsync();

                var householdIds = await _context.Households
                    .Find(householdFilter)
                    .Project(h => h.Id)
                    .ToListAsync();

                citizenFilter = Builders<Citizen>.Filter.In(c => c.HouseholdId, householdIds);
            }
            else
            {
                citizenFilter = Builders<Citizen>.Filter.Empty;
            }

            var totalCitizens = await _context.Citizens
                .CountDocumentsAsync(citizenFilter);

            // Đếm yêu cầu
            var pendingRequests = await _context.CitizenRequests
                .CountDocumentsAsync(r => r.Status == 0); // 0 = Pending

            var approvedRequests = await _context.CitizenRequests
                .CountDocumentsAsync(r => r.Status == 1); // 1 = Approved

            var rejectedRequests = await _context.CitizenRequests
                .CountDocumentsAsync(r => r.Status == 2); // 2 = Rejected

            // Đếm biến động dân cư trong tháng
            var monthlyChanges = await _context.PopulationChanges
                .CountDocumentsAsync(c => c.ChangeDate >= startOfMonth);

            return new ProvinceStatisticsResponse
            {
                TotalHouseholds = (int)totalHouseholds,
                TotalCitizens = (int)totalCitizens,
                PendingRequests = (int)pendingRequests,
                ApprovedRequests = (int)approvedRequests,
                RejectedRequests = (int)rejectedRequests,
                MonthlyPopulationChanges = (int)monthlyChanges
            };
        }
    }
}

