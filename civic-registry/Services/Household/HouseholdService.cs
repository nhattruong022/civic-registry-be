using CivicRegistry.API.Models;
using CivicRegistry.API.DTOs;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service xử lý quản lý hộ khẩu
    /// </summary>
    public class HouseholdService
    {
        private readonly MongoDbContext _context;

        public HouseholdService(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách hộ khẩu (có thể lọc theo hamletId)
        /// </summary>
        public async Task<List<HouseholdInfo>> GetAllHouseholdsAsync(string? hamletId = null)
        {
            FilterDefinition<Household> filter;
            
            if (!string.IsNullOrEmpty(hamletId))
            {
                filter = Builders<Household>.Filter.Eq(h => h.HamletId, hamletId);
            }
            else
            {
                filter = Builders<Household>.Filter.Empty;
            }

            var households = await _context.Households
                .Find(filter)
                .SortByDescending(h => h.CreatedDate)
                .ToListAsync();

            return households.Select(h => new HouseholdInfo
            {
                Id = h.Id,
                HouseholdCode = h.HouseholdCode,
                Address = h.Address,
                HamletId = h.HamletId,
                HeadCitizenId = h.HeadCitizenId ?? string.Empty,
                CreatedDate = h.CreatedDate,
                Status = h.Status
            }).ToList();
        }

        /// <summary>
        /// Lấy danh sách hộ khẩu có phân trang (có thể lọc theo hamletId)
        /// </summary>
        public async Task<PaginatedResponse<HouseholdInfo>> GetHouseholdsPaginatedAsync(int page = 1, int pageSize = 20, string? hamletId = null)
        {
            FilterDefinition<Household> filter;
            
            if (!string.IsNullOrEmpty(hamletId))
            {
                filter = Builders<Household>.Filter.Eq(h => h.HamletId, hamletId);
            }
            else
            {
                filter = Builders<Household>.Filter.Empty;
            }

            // Đảm bảo page và pageSize hợp lệ
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa 100 items mỗi trang

            // Đếm tổng số items
            var totalItems = await _context.Households.CountDocumentsAsync(filter);

            // Tính toán phân trang
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var skip = (page - 1) * pageSize;

            // Lấy dữ liệu với phân trang
            var households = await _context.Households
                .Find(filter)
                .SortByDescending(h => h.CreatedDate)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            var items = households.Select(h => new HouseholdInfo
            {
                Id = h.Id,
                HouseholdCode = h.HouseholdCode,
                Address = h.Address,
                HamletId = h.HamletId,
                HeadCitizenId = h.HeadCitizenId,
                CreatedDate = h.CreatedDate,
                Status = h.Status
            }).ToList();

            return new PaginatedResponse<HouseholdInfo>
            {
                Items = items,
                Pagination = new PaginationInfo
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPrevPage = page > 1
                }
            };
        }

        /// <summary>
        /// Lấy hộ khẩu theo ID
        /// </summary>
        public async Task<HouseholdInfo?> GetHouseholdByIdAsync(string id)
        {
            var household = await _context.Households
                .Find(h => h.Id == id)
                .FirstOrDefaultAsync();

            if (household == null)
            {
                return null;
            }

            return new HouseholdInfo
            {
                Id = household.Id,
                HouseholdCode = household.HouseholdCode,
                Address = household.Address,
                HamletId = household.HamletId,
                HeadCitizenId = household.HeadCitizenId ?? string.Empty,
                CreatedDate = household.CreatedDate,
                Status = household.Status
            };
        }

        /// <summary>
        /// Tạo hộ khẩu mới
        /// </summary>
        public async Task<HouseholdInfo?> CreateHouseholdAsync(CreateHouseholdRequest request)
        {
            // Kiểm tra mã hộ khẩu đã tồn tại chưa
            var existingHousehold = await _context.Households
                .Find(h => h.HouseholdCode == request.HouseholdCode)
                .FirstOrDefaultAsync();

            if (existingHousehold != null)
            {
                return null; // Mã hộ khẩu đã tồn tại
            }

            // Kiểm tra thôn có tồn tại không
            if (!string.IsNullOrEmpty(request.HamletId))
            {
                var hamlet = await _context.Hamlets
                    .Find(h => h.Id == request.HamletId)
                    .FirstOrDefaultAsync();

                if (hamlet == null)
                {
                    return null; // Thôn không tồn tại
                }
            }

            // Kiểm tra chủ hộ có tồn tại không (nếu có)
            if (!string.IsNullOrEmpty(request.HeadCitizenId))
            {
                var citizen = await _context.Citizens
                    .Find(c => c.Id == request.HeadCitizenId)
                    .FirstOrDefaultAsync();

                if (citizen == null)
                {
                    return null; // Chủ hộ không tồn tại
                }
            }

            // Tạo hộ khẩu mới
            var newHousehold = new Household
            {
                HouseholdCode = request.HouseholdCode,
                Address = request.Address,
                HamletId = request.HamletId,
                HeadCitizenId = request.HeadCitizenId ?? string.Empty,
                Status = request.Status,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Households.InsertOneAsync(newHousehold);

            return new HouseholdInfo
            {
                Id = newHousehold.Id,
                HouseholdCode = newHousehold.HouseholdCode,
                Address = newHousehold.Address,
                HamletId = newHousehold.HamletId,
                HeadCitizenId = newHousehold.HeadCitizenId,
                CreatedDate = newHousehold.CreatedDate,
                Status = newHousehold.Status
            };
        }

        /// <summary>
        /// Cập nhật hộ khẩu
        /// </summary>
        public async Task<HouseholdInfo?> UpdateHouseholdAsync(string id, UpdateHouseholdRequest request)
        {
            // Tìm hộ khẩu
            var household = await _context.Households
                .Find(h => h.Id == id)
                .FirstOrDefaultAsync();

            if (household == null)
            {
                return null; // Hộ khẩu không tồn tại
            }

            // Kiểm tra mã hộ khẩu mới có trùng không (nếu có thay đổi)
            if (!string.IsNullOrEmpty(request.HouseholdCode) && request.HouseholdCode != household.HouseholdCode)
            {
                var existingHousehold = await _context.Households
                    .Find(h => h.HouseholdCode == request.HouseholdCode)
                    .FirstOrDefaultAsync();

                if (existingHousehold != null)
                {
                    return null; // Mã hộ khẩu đã tồn tại
                }
            }

            // Kiểm tra thôn có tồn tại không (nếu có thay đổi)
            if (!string.IsNullOrEmpty(request.HamletId) && request.HamletId != household.HamletId)
            {
                var hamlet = await _context.Hamlets
                    .Find(h => h.Id == request.HamletId)
                    .FirstOrDefaultAsync();

                if (hamlet == null)
                {
                    return null; // Thôn không tồn tại
                }
            }

            // Kiểm tra chủ hộ có tồn tại không (nếu có thay đổi)
            if (request.HeadCitizenId != null && request.HeadCitizenId != household.HeadCitizenId)
            {
                if (!string.IsNullOrEmpty(request.HeadCitizenId))
                {
                    var citizen = await _context.Citizens
                        .Find(c => c.Id == request.HeadCitizenId)
                        .FirstOrDefaultAsync();

                    if (citizen == null)
                    {
                        return null; // Chủ hộ không tồn tại
                    }
                }
            }

            // Cập nhật thông tin
            if (!string.IsNullOrEmpty(request.HouseholdCode))
            {
                household.HouseholdCode = request.HouseholdCode;
            }
            if (!string.IsNullOrEmpty(request.Address))
            {
                household.Address = request.Address;
            }
            if (!string.IsNullOrEmpty(request.HamletId))
            {
                household.HamletId = request.HamletId;
            }
            if (request.HeadCitizenId != null)
            {
                household.HeadCitizenId = request.HeadCitizenId;
            }
            if (request.Status.HasValue)
            {
                household.Status = request.Status.Value;
            }

            await _context.Households.ReplaceOneAsync(h => h.Id == id, household);

            return new HouseholdInfo
            {
                Id = household.Id,
                HouseholdCode = household.HouseholdCode,
                Address = household.Address,
                HamletId = household.HamletId,
                HeadCitizenId = household.HeadCitizenId ?? string.Empty,
                CreatedDate = household.CreatedDate,
                Status = household.Status
            };
        }

        /// <summary>
        /// Kiểm tra mã hộ khẩu đã tồn tại chưa
        /// </summary>
        public async Task<bool> CheckHouseholdCodeExistsAsync(string householdCode)
        {
            var household = await _context.Households
                .Find(h => h.HouseholdCode == householdCode)
                .FirstOrDefaultAsync();
            return household != null;
        }
    }
}

