using CivicRegistry.API.Models;
using CivicRegistry.API.DTOs;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service xử lý yêu cầu công dân
    /// </summary>
    public class RequestService
    {
        private readonly MongoDbContext _context;

        public RequestService(MongoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy yêu cầu theo ID
        /// </summary>
        public async Task<RequestInfo?> GetRequestByIdAsync(string id)
        {
            var request = await _context.CitizenRequests
                .Find(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return null;
            }

            return new RequestInfo
            {
                Id = request.Id,
                CitizenId = request.CitizenId,
                RequestType = request.RequestType,
                Content = request.Content,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                ProcessedBy = request.ProcessedBy,
                ProcessedAt = request.ProcessedAt
            };
        }

        /// <summary>
        /// Lấy danh sách yêu cầu đang chờ xử lý
        /// </summary>
        public async Task<List<RequestInfo>> GetPendingRequestsAsync()
        {
            var requests = await _context.CitizenRequests
                .Find(r => r.Status == 0) // 0 = Pending
                .SortByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests.Select(r => new RequestInfo
            {
                Id = r.Id,
                CitizenId = r.CitizenId,
                RequestType = r.RequestType,
                Content = r.Content,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ProcessedBy = r.ProcessedBy,
                ProcessedAt = r.ProcessedAt
            }).ToList();
        }

        /// <summary>
        /// Duyệt yêu cầu
        /// </summary>
        public async Task<RequestInfo?> ApproveRequestAsync(string id, string processedBy)
        {
            var request = await _context.CitizenRequests
                .Find(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return null;
            }

            // Cập nhật trạng thái
            request.Status = 1; // 1 = Approved
            request.ProcessedBy = processedBy;
            request.ProcessedAt = DateTime.UtcNow;

            await _context.CitizenRequests.ReplaceOneAsync(r => r.Id == id, request);

            return new RequestInfo
            {
                Id = request.Id,
                CitizenId = request.CitizenId,
                RequestType = request.RequestType,
                Content = request.Content,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                ProcessedBy = request.ProcessedBy,
                ProcessedAt = request.ProcessedAt
            };
        }

        /// <summary>
        /// Từ chối yêu cầu
        /// </summary>
        public async Task<RequestInfo?> RejectRequestAsync(string id, string processedBy)
        {
            var request = await _context.CitizenRequests
                .Find(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return null;
            }

            // Cập nhật trạng thái
            request.Status = 2; // 2 = Rejected
            request.ProcessedBy = processedBy;
            request.ProcessedAt = DateTime.UtcNow;

            await _context.CitizenRequests.ReplaceOneAsync(r => r.Id == id, request);

            return new RequestInfo
            {
                Id = request.Id,
                CitizenId = request.CitizenId,
                RequestType = request.RequestType,
                Content = request.Content,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                ProcessedBy = request.ProcessedBy,
                ProcessedAt = request.ProcessedAt
            };
        }
    }
}

