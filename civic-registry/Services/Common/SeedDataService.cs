using CivicRegistry.API.Models;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service để seed dữ liệu mẫu vào database
    /// </summary>
    public class SeedDataService
    {
        private readonly MongoDbContext _context;
        private readonly AuthService _authService;

        public SeedDataService(MongoDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Seed tất cả dữ liệu mẫu
        /// </summary>
        public async Task SeedAllAsync()
        {
            await SeedProvincesAsync();
            await SeedDistrictsAsync();
            await SeedWardsAsync();
            await SeedHamletsAsync();
            await SeedUsersAsync();
            await SeedHouseholdsAsync();
            await SeedCitizensAsync();
        }

        /// <summary>
        /// Seed dữ liệu tỉnh
        /// </summary>
        private async Task SeedProvincesAsync()
        {
            var provinces = new List<(string Code, string Name)>
            {
                ("HN", "Hà Nội"),
                ("HCM", "Hồ Chí Minh"),
                ("DN", "Đà Nẵng"),
                ("HP", "Hải Phòng"),
                ("AG", "An Giang")
            };

            foreach (var (code, name) in provinces)
            {
                var existing = await _context.Provinces
                    .Find(p => p.Code == code)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    var province = new Province
                    {
                        Name = name,
                        Code = code
                    };
                    await _context.Provinces.InsertOneAsync(province);
                }
                else
                {
                    // Cập nhật nếu đã tồn tại
                    existing.Name = name;
                    existing.Code = code;
                    await _context.Provinces.ReplaceOneAsync(p => p.Id == existing.Id, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu huyện
        /// </summary>
        private async Task SeedDistrictsAsync()
        {
            // Lấy ID tỉnh từ database
            var haNoi = await _context.Provinces.Find(p => p.Code == "HN").FirstOrDefaultAsync();
            var hcm = await _context.Provinces.Find(p => p.Code == "HCM").FirstOrDefaultAsync();
            var daNang = await _context.Provinces.Find(p => p.Code == "DN").FirstOrDefaultAsync();

            if (haNoi == null || hcm == null || daNang == null)
            {
                return; // Chưa có dữ liệu tỉnh
            }

            var districts = new List<(string Name, string ProvinceId)>
            {
                // Hà Nội
                ("Quận Ba Đình", haNoi.Id),
                ("Quận Hoàn Kiếm", haNoi.Id),
                ("Quận Đống Đa", haNoi.Id),
                ("Huyện Ba Vì", haNoi.Id),
                
                // Hồ Chí Minh
                ("Quận 1", hcm.Id),
                ("Quận 2", hcm.Id),
                ("Quận 3", hcm.Id),
                
                // Đà Nẵng
                ("Quận Hải Châu", daNang.Id),
                ("Quận Thanh Khê", daNang.Id)
            };

            foreach (var (name, provinceId) in districts)
            {
                var existing = await _context.Districts
                    .Find(d => d.Name == name && d.ProvinceId == provinceId)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    var district = new District
                    {
                        Name = name,
                        ProvinceId = provinceId
                    };
                    await _context.Districts.InsertOneAsync(district);
                }
                else
                {
                    existing.Name = name;
                    existing.ProvinceId = provinceId;
                    await _context.Districts.ReplaceOneAsync(d => d.Id == existing.Id, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu xã
        /// </summary>
        private async Task SeedWardsAsync()
        {
            // Lấy ID huyện từ database
            var baDinh = await _context.Districts.Find(d => d.Name == "Quận Ba Đình").FirstOrDefaultAsync();
            var hoanKiem = await _context.Districts.Find(d => d.Name == "Quận Hoàn Kiếm").FirstOrDefaultAsync();
            var quan1 = await _context.Districts.Find(d => d.Name == "Quận 1").FirstOrDefaultAsync();

            if (baDinh == null || hoanKiem == null || quan1 == null)
            {
                return; // Chưa có dữ liệu huyện
            }

            var wards = new List<(string Name, string DistrictId)>
            {
                // Quận Ba Đình - Hà Nội
                ("Phường Cống Vị", baDinh.Id),
                ("Phường Điện Biên", baDinh.Id),
                ("Phường Đội Cấn", baDinh.Id),
                
                // Quận Hoàn Kiếm - Hà Nội
                ("Phường Chương Dương", hoanKiem.Id),
                ("Phường Cửa Đông", hoanKiem.Id),
                
                // Quận 1 - Hồ Chí Minh
                ("Phường Bến Nghé", quan1.Id),
                ("Phường Bến Thành", quan1.Id),
                ("Phường Cô Giang", quan1.Id)
            };

            foreach (var (name, districtId) in wards)
            {
                var existing = await _context.Wards
                    .Find(w => w.Name == name && w.DistrictId == districtId)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    var ward = new Ward
                    {
                        Name = name,
                        DistrictId = districtId
                    };
                    await _context.Wards.InsertOneAsync(ward);
                }
                else
                {
                    existing.Name = name;
                    existing.DistrictId = districtId;
                    await _context.Wards.ReplaceOneAsync(w => w.Id == existing.Id, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu thôn
        /// </summary>
        private async Task SeedHamletsAsync()
        {
            // Lấy ID xã từ database
            var congVi = await _context.Wards.Find(w => w.Name == "Phường Cống Vị").FirstOrDefaultAsync();
            var dienBien = await _context.Wards.Find(w => w.Name == "Phường Điện Biên").FirstOrDefaultAsync();
            var benNghe = await _context.Wards.Find(w => w.Name == "Phường Bến Nghé").FirstOrDefaultAsync();

            if (congVi == null || dienBien == null || benNghe == null)
            {
                return; // Chưa có dữ liệu xã
            }

            var hamlets = new List<(string Name, string WardId)>
            {
                // Phường Cống Vị - Quận Ba Đình
                ("Tổ dân phố 1", congVi.Id),
                ("Tổ dân phố 2", congVi.Id),
                ("Tổ dân phố 3", congVi.Id),
                
                // Phường Điện Biên - Quận Ba Đình
                ("Tổ dân phố 1", dienBien.Id),
                ("Tổ dân phố 2", dienBien.Id),
                
                // Phường Bến Nghé - Quận 1 - HCM
                ("Tổ dân phố 1", benNghe.Id),
                ("Tổ dân phố 2", benNghe.Id)
            };

            foreach (var (name, wardId) in hamlets)
            {
                var existing = await _context.Hamlets
                    .Find(h => h.Name == name && h.WardId == wardId)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    var hamlet = new Hamlet
                    {
                        Name = name,
                        WardId = wardId
                    };
                    await _context.Hamlets.InsertOneAsync(hamlet);
                }
                else
                {
                    existing.Name = name;
                    existing.WardId = wardId;
                    await _context.Hamlets.ReplaceOneAsync(h => h.Id == existing.Id, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu users mẫu
        /// </summary>
        private async Task SeedUsersAsync()
        {
            // Lấy ID địa giới hành chính từ database
            var haNoi = await _context.Provinces.Find(p => p.Code == "HN").FirstOrDefaultAsync();
            var baDinh = await _context.Districts.Find(d => d.Name == "Quận Ba Đình").FirstOrDefaultAsync();
            var congVi = await _context.Wards.Find(w => w.Name == "Phường Cống Vị").FirstOrDefaultAsync();

            if (haNoi == null)
            {
                return; // Chưa có dữ liệu tỉnh
            }

            // Parse ProvinceId, DistrictId, WardId từ string sang int (nếu cần)
            // Nhưng vì User model dùng int? cho ProvinceId, DistrictId, WardId
            // nên ta cần map từ string ID sang int code hoặc lưu trực tiếp
            // Tạm thời dùng code số cho ProvinceId, DistrictId, WardId
            var users = new List<User>
            {
                new User
                {
                    Username = "superadmin",
                    PasswordHash = _authService.HashPassword("Admin@123"),
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "provinceadmin1",
                    PasswordHash = _authService.HashPassword("Admin@123"),
                    Role = "ProvinceAdmin",
                    ProvinceId = 1, // Hà Nội (code số)
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "districtadmin1",
                    PasswordHash = _authService.HashPassword("Admin@123"),
                    Role = "DistrictAdmin",
                    ProvinceId = 1,
                    DistrictId = 1, // Quận Ba Đình (code số)
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "wardadmin1",
                    PasswordHash = _authService.HashPassword("Admin@123"),
                    Role = "WardAdmin",
                    ProvinceId = 1,
                    DistrictId = 1,
                    WardId = 1, // Phường Cống Vị (code số)
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "citizen1",
                    PasswordHash = _authService.HashPassword("Citizen@123"),
                    Role = "Citizen",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var user in users)
            {
                var existing = await _context.Users
                    .Find(u => u.Username == user.Username)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    await _context.Users.InsertOneAsync(user);
                }
                else
                {
                    // Cập nhật thông tin user (trừ password nếu không thay đổi)
                    existing.Role = user.Role;
                    existing.ProvinceId = user.ProvinceId;
                    existing.DistrictId = user.DistrictId;
                    existing.WardId = user.WardId;
                    existing.IsActive = user.IsActive;
                    await _context.Users.ReplaceOneAsync(u => u.Username == user.Username, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu hộ khẩu mẫu
        /// </summary>
        private async Task SeedHouseholdsAsync()
        {
            // Lấy ID thôn từ database
            var congVi = await _context.Wards.Find(w => w.Name == "Phường Cống Vị").FirstOrDefaultAsync();
            if (congVi == null) return;

            var hamlets = await _context.Hamlets
                .Find(h => h.WardId == congVi.Id)
                .ToListAsync();

            if (hamlets == null || hamlets.Count == 0)
            {
                return; // Chưa có dữ liệu thôn
            }

            var firstHamlet = hamlets.FirstOrDefault();
            if (firstHamlet == null) return;

            var households = new List<(string HouseholdCode, string Address, string HamletId)>
            {
                ("HK001", "Số 123, Đường Cống Vị, Phường Cống Vị, Quận Ba Đình", firstHamlet.Id),
                ("HK002", "Số 456, Đường Cống Vị, Phường Cống Vị, Quận Ba Đình", firstHamlet.Id),
                ("HK003", "Số 789, Đường Cống Vị, Phường Cống Vị, Quận Ba Đình", firstHamlet.Id),
                ("HK004", "Số 321, Đường Cống Vị, Phường Cống Vị, Quận Ba Đình", firstHamlet.Id),
                ("HK005", "Số 654, Đường Cống Vị, Phường Cống Vị, Quận Ba Đình", firstHamlet.Id)
            };

            foreach (var (householdCode, address, hamletId) in households)
            {
                var existing = await _context.Households
                    .Find(h => h.HouseholdCode == householdCode)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    // HeadCitizenId để trống vì chưa có Citizen
                    var household = new Household
                    {
                        HouseholdCode = householdCode,
                        Address = address,
                        HamletId = hamletId,
                        HeadCitizenId = string.Empty, // Sẽ cập nhật sau khi có Citizen
                        Status = 0, // Active
                        CreatedDate = DateTime.UtcNow
                    };
                    await _context.Households.InsertOneAsync(household);
                }
                else
                {
                    // Cập nhật nếu đã tồn tại
                    existing.HouseholdCode = householdCode;
                    existing.Address = address;
                    existing.HamletId = hamletId;
                    existing.Status = 0;
                    await _context.Households.ReplaceOneAsync(h => h.Id == existing.Id, existing);
                }
            }
        }

        /// <summary>
        /// Seed dữ liệu nhân khẩu (Citizens) mẫu
        /// </summary>
        private async Task SeedCitizensAsync()
        {
            // Lấy danh sách hộ khẩu từ database
            var households = await _context.Households
                .Find(h => h.HouseholdCode.StartsWith("HK"))
                .Limit(5)
                .ToListAsync();

            if (households == null || households.Count == 0)
            {
                return; // Chưa có dữ liệu hộ khẩu
            }

            var citizens = new List<(string CitizenCode, string FullName, DateTime DateOfBirth, int Gender, string CCCD, string Ethnic, string Religion, string Job, string Education, int MaritalStatus, string HouseholdId, string Status)>
            {
                // Hộ khẩu 1 - Chủ hộ
                ("CT001", "Nguyễn Văn An", new DateTime(1980, 5, 15), 0, "001234567890", "Kinh", "Không", "Kỹ sư", "Đại học", 1, households[0].Id, "Living"),
                // Hộ khẩu 1 - Vợ
                ("CT002", "Trần Thị Bình", new DateTime(1982, 8, 20), 1, "001234567891", "Kinh", "Không", "Giáo viên", "Đại học", 1, households[0].Id, "Living"),
                // Hộ khẩu 1 - Con trai
                ("CT003", "Nguyễn Văn Cường", new DateTime(2010, 3, 10), 0, null, "Kinh", "Không", "Học sinh", "Trung học", 0, households[0].Id, "Living"),
                
                // Hộ khẩu 2 - Chủ hộ
                ("CT004", "Lê Văn Đức", new DateTime(1975, 12, 5), 0, "001234567892", "Kinh", "Phật giáo", "Bác sĩ", "Đại học", 1, households[1].Id, "Living"),
                // Hộ khẩu 2 - Vợ
                ("CT005", "Phạm Thị Em", new DateTime(1978, 6, 18), 1, "001234567893", "Kinh", "Phật giáo", "Y tá", "Cao đẳng", 1, households[1].Id, "Living"),
                
                // Hộ khẩu 3 - Chủ hộ
                ("CT006", "Hoàng Văn Phúc", new DateTime(1990, 2, 25), 0, "001234567894", "Kinh", "Không", "Công nhân", "Trung học", 0, households[2].Id, "Living"),
                
                // Hộ khẩu 4 - Chủ hộ
                ("CT007", "Vũ Thị Giang", new DateTime(1985, 9, 30), 1, "001234567895", "Kinh", "Công giáo", "Kinh doanh", "Trung học", 2, households[3].Id, "Living"),
                // Hộ khẩu 4 - Con gái
                ("CT008", "Vũ Thị Hoa", new DateTime(2015, 7, 12), 1, null, "Kinh", "Công giáo", "Học sinh", "Tiểu học", 0, households[3].Id, "Living"),
                
                // Hộ khẩu 5 - Chủ hộ
                ("CT009", "Đặng Văn Khoa", new DateTime(1970, 11, 8), 0, "001234567896", "Kinh", "Không", "Nông dân", "Tiểu học", 1, households[4].Id, "Living"),
                // Hộ khẩu 5 - Vợ
                ("CT010", "Bùi Thị Lan", new DateTime(1972, 4, 22), 1, "001234567897", "Kinh", "Không", "Nông dân", "Tiểu học", 1, households[4].Id, "Living")
            };

            foreach (var (citizenCode, fullName, dateOfBirth, gender, cccd, ethnic, religion, job, education, maritalStatus, householdId, status) in citizens)
            {
                var existing = await _context.Citizens
                    .Find(c => c.CitizenCode == citizenCode)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // Tạo mới, để MongoDB tự tạo Id
                    var citizen = new Citizen
                    {
                        CitizenCode = citizenCode,
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                        Gender = gender,
                        CCCD = cccd,
                        Ethnic = ethnic,
                        Religion = religion,
                        Job = job,
                        Education = education,
                        MaritalStatus = maritalStatus,
                        HouseholdId = householdId,
                        Status = status
                    };
                    await _context.Citizens.InsertOneAsync(citizen);
                }
                else
                {
                    // Cập nhật nếu đã tồn tại
                    existing.FullName = fullName;
                    existing.DateOfBirth = dateOfBirth;
                    existing.Gender = gender;
                    existing.CCCD = cccd;
                    existing.Ethnic = ethnic;
                    existing.Religion = religion;
                    existing.Job = job;
                    existing.Education = education;
                    existing.MaritalStatus = maritalStatus;
                    existing.HouseholdId = householdId;
                    existing.Status = status;
                    await _context.Citizens.ReplaceOneAsync(c => c.Id == existing.Id, existing);
                }
            }

            // Cập nhật HeadCitizenId cho các hộ khẩu (lấy chủ hộ đầu tiên của mỗi hộ)
            foreach (var household in households)
            {
                var headCitizen = await _context.Citizens
                    .Find(c => c.HouseholdId == household.Id)
                    .SortBy(c => c.CitizenCode)
                    .FirstOrDefaultAsync();

                if (headCitizen != null && string.IsNullOrEmpty(household.HeadCitizenId))
                {
                    household.HeadCitizenId = headCitizen.Id;
                    await _context.Households.ReplaceOneAsync(h => h.Id == household.Id, household);
                }
            }
        }
    }
}

