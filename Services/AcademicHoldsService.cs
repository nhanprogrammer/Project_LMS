using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;
using System.Collections;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAcademicHoldRepository _academicHoldRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ISemesterRepository _semesterRepository;


        public AcademicHoldsService(ApplicationDbContext context, IAcademicHoldRepository academicHoldRepository, IMapper mapper, ICloudinaryService cloudinaryService, ISemesterRepository semesterRepository)
        {
            _context = context;
            _academicHoldRepository = academicHoldRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _semesterRepository = semesterRepository;
        }

        public async Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request, int? academicYearId)
        {
            try
            {
                // Lấy thông tin niên khóa nếu academicYearId được cung cấp
                DateTime? academicYearStartDate = null;
                DateTime? academicYearEndDate = null;

                if (academicYearId.HasValue)
                {
                    var academicYear = await _context.AcademicYears
                        .Where(ay => ay.Id == academicYearId.Value && (ay.IsDelete == false || ay.IsDelete == null))
                        .Select(ay => new { ay.StartDate, ay.EndDate })
                        .FirstOrDefaultAsync();

                    if (academicYear == null)
                    {
                        throw new NotFoundException($"Không tìm thấy niên khóa với ID {academicYearId}.");
                    }

                    if (!academicYear.StartDate.HasValue || !academicYear.EndDate.HasValue)
                    {
                        throw new BadRequestException($"Niên khóa với ID {academicYearId} không có ngày bắt đầu hoặc ngày kết thúc hợp lệ.");
                    }

                    academicYearStartDate = academicYear.StartDate.Value;
                    academicYearEndDate = academicYear.EndDate.Value;
                }

                // Truy vấn dữ liệu bảo lưu
                var query = from ah in _academicHoldRepository.GetQueryable().Where(ah => !ah.IsDelete)
                            join u in _context.Users on ah.UserId equals u.Id into userGroup
                            from userInfo in userGroup.DefaultIfEmpty()
                            select new
                            {
                                ah,
                                UserInfo = userInfo
                            };

                // Lọc theo niên khóa nếu academicYearId được cung cấp
                if (academicYearId.HasValue && academicYearStartDate.HasValue && academicYearEndDate.HasValue)
                {
                    query = query.Where(data => data.ah.HoldDate >= academicYearStartDate.Value && data.ah.HoldDate <= academicYearEndDate.Value);
                }

                // Lấy danh sách dữ liệu từ database
                var rawData = await query.AsNoTracking().ToListAsync();

                // Xử lý trên bộ nhớ
                var resultList = rawData
                    .Select(data => new AcademicHoldResponse
                    {
                        Id = data.ah.Id,
                        UserId = data.ah.UserId,
                        UserCode = data.UserInfo?.UserCode,
                        FullName = data.UserInfo?.FullName,
                        BirthDate = data.UserInfo?.BirthDate,
                        Gender = data.UserInfo?.Gender is BitArray bitArray && bitArray.Count > 0 ? (bitArray[0] ? "True" : "False") : null,
                        ClassName = _context.Classes
                                           .Where(c => c.UserId == data.ah.UserId)
                                           .Select(c => c.Name)
                                           .FirstOrDefault(),
                        HoldDate = data.ah.HoldDate,
                        HoldDuration = data.ah.HoldDuration,
                        Reason = data.ah.Reason
                    })
                    .OrderByDescending(x => x.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                int totalItems = rawData.Count();

                return new PaginatedResponse<AcademicHoldResponse>
                {
                    Items = resultList,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber * request.PageSize < totalItems
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Lỗi khi lấy danh sách bảo lưu: {ex.Message}");
            }
        }
        public async Task<User_AcademicHoldResponse> GetById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.StudentStatus)
                .Include(u => u.Departments)
                .Include(u => u.Classes)
                .FirstOrDefaultAsync(u => u.Id == id && !(u.IsDelete ?? false));

            if (user == null) return null;

            var academicYear = await _context.AcademicYears
                .OrderByDescending(a => a.StartDate)
                .FirstOrDefaultAsync();

            return new User_AcademicHoldResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                PlaceOfBirth = user.PlaceOfBirth,
                Ethnicity = user.Ethnicity,
                Religion = user.Religion,
                AcademicStartDate = academicYear?.StartDate?.Year.ToString(),
                AcademicEndDate = academicYear?.EndDate?.Year.ToString(),
                //DepartmentName = string.Join(", ", user.Departments.Select(d => d.Name)),
                DepartmentName = user.Departments?.FirstOrDefault()?.Name,
                //ClassName = string.Join(", ", user.Classes.Select(d => d.Name)),
                ClassName = user.Classes?.FirstOrDefault()?.Name,
                UserCode = user.UserCode,
                StartDate = user.StartDate,
                AdmissionType = user.AdmissionType,
                StatusName_Student = user.StudentStatus?.StatusName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.Phone,
                FullnameFather = user.FullnameFather,
                FullnameMother = user.FullnameMother,
                FullnameGuardianship = user.FullnameGuardianship,
                BirthFather = user.BirthFather,
                BirthMother = user.BirthMother,
                BirthGuardianship = user.BirthGuardianship,
                WorkFather = user.WorkFather,
                WorkMother = user.WorkMother,
                WorkGuardianship = user.WorkGuardianship,
                PhoneFather = user.PhoneFather,
                PhoneMother = user.PhoneMother,
                PhoneGuardianship = user.PhoneGuardianship,
            };
        }


        public async Task<AcademicHoldResponse> AddAcademicHold(CreateAcademicHoldRequest request, int userId)
        {
            var errors = new List<ValidationError>();
            string? fileName = null;

            // Kiểm tra dữ liệu đầu vào
            if (request.UserId <= 0)
                errors.Add(new ValidationError { Field = "User", Error = "Mã người dùng không hợp lệ. Vui lòng cung cấp một mã người dùng lớn hơn 0." });

            if (request.ClassId <= 0)
                errors.Add(new ValidationError { Field = "Class", Error = "Mã lớp học không hợp lệ. Vui lòng cung cấp một mã lớp học lớn hơn 0." });

            if (string.IsNullOrWhiteSpace(request.Reason))
                errors.Add(new ValidationError { Field = "Reason", Error = "Lý do bảo lưu không được để trống. Vui lòng nhập lý do." });

            // Kiểm tra UserId và ClassId tồn tại
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && !(u.IsDelete ?? false));

            if (user == null)
            {
                errors.Add(new ValidationError { Field = "User", Error = "Người dùng không tồn tại trong hệ thống. Vui lòng kiểm tra lại mã người dùng." });
            }
            else
            {
                // Kiểm tra trạng thái của học viên
                if (user.StudentStatusId != 1)
                {
                    errors.Add(new ValidationError
                    {
                        Field = "User",
                        Error = "Học viên không ở trạng thái active. Không thể tạo bảo lưu cho học viên này."
                    });
                }
            }

            var classInfo = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == request.ClassId && !(c.IsDelete ?? false));
            if (classInfo == null)
                errors.Add(new ValidationError { Field = "Class", Error = "Lớp học không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại mã lớp học." });

            SemesterResponse? semester = null;
            try
            {
                semester = await GetSemesterByDateAsync(request.HoldDate.ToString("yyyy-MM-dd"));
            }
            catch (BadRequestException ex)
            {
                errors.Add(new ValidationError { Field = "HoldDate", Error = ex.Message });
            }

            if (semester != null && semester.Id > 0)
            {
                // Lấy StartDate và EndDate từ bảng Semesters
                var semesterDetails = await _semesterRepository.GetByIdAsync(semester.Id);

                if (semesterDetails != null && semesterDetails.StartDate.HasValue && semesterDetails.EndDate.HasValue)
                {
                    var existingHoldInSemester = await _context.AcademicHolds
                        .Where(ah => ah.UserId == request.UserId
                                  && ah.HoldDate >= semesterDetails.StartDate.Value
                                  && ah.HoldDate <= semesterDetails.EndDate.Value
                                  && !ah.IsDelete)
                        .AnyAsync();

                    if (existingHoldInSemester)
                    {
                        errors.Add(new ValidationError
                        {
                            Field = "User",
                            Error = "Học sinh này đã có một bản ghi bảo lưu trong học kỳ hiện tại. Không thể bảo lưu thêm lần nữa trong cùng học kỳ."
                        });
                    }
                }
                else
                {
                    errors.Add(new ValidationError
                    {
                        Field = "HoldDate",
                        Error = "Học kỳ không có ngày bắt đầu hoặc ngày kết thúc hợp lệ. Vui lòng liên hệ quản trị viên để kiểm tra."
                    });
                }
            }

            if (errors.Any())
            {
                throw new BadRequestException("Thêm bảo lưu thất bại do dữ liệu không hợp lệ.", errors);
            }

            if (!string.IsNullOrEmpty(request.FileName))
            {
                fileName = await _cloudinaryService.UploadDocxAsync(request.FileName);
            }

            // Tạo mới AcademicHold
            var newHold = new AcademicHold
            {
                UserId = request.UserId,
                HoldDate = request.HoldDate.DateTime,
                HoldDuration = semester?.Name ?? string.Empty,
                FileName = fileName,
                Reason = request.Reason,
                UserCreate = userId,
            };

            _context.AcademicHolds.Add(newHold);
            if (user != null)
            {
                user.StudentStatusId = 2; // Cập nhật trạng thái thành bảo lưu
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();

            return new AcademicHoldResponse
            {
                Id = newHold.Id,
                UserId = newHold.UserId,
                HoldDate = newHold.HoldDate,
                HoldDuration = newHold.HoldDuration,
                Reason = newHold.Reason,
                FileName = newHold.FileName
            };
        }

        public async Task<AcademicHoldResponse> UpdateAcademicHold(UpdateAcademicHoldRequest academicHold, int userId)
        {
            var errors = new List<ValidationError>();

            // Kiểm tra dữ liệu đầu vào
            if (academicHold.UserId <= 0)
                errors.Add(new ValidationError { Field = "User", Error = "Mã người dùng không hợp lệ. Vui lòng cung cấp một mã người dùng lớn hơn 0." });

            if (academicHold.ClassId <= 0)
                errors.Add(new ValidationError { Field = "Class", Error = "Mã lớp học không hợp lệ. Vui lòng cung cấp một mã lớp học lớn hơn 0." });

            if (string.IsNullOrWhiteSpace(academicHold.Reason))
                errors.Add(new ValidationError { Field = "Reason", Error = "Lý do bảo lưu không được để trống. Vui lòng nhập lý do." });

            // Kiểm tra UserId và ClassId tồn tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == academicHold.UserId && !(u.IsDelete ?? false));
            if (user == null)
                errors.Add(new ValidationError { Field = "User", Error = "Người dùng không tồn tại trong hệ thống. Vui lòng kiểm tra lại mã người dùng." });

            var classInfo = await _context.Classes.FirstOrDefaultAsync(c => c.Id == academicHold.ClassId && !(c.IsDelete ?? false));
            if (classInfo == null)
                errors.Add(new ValidationError { Field = "Class", Error = "Lớp học không tồn tại hoặc đã bị xóa. Vui lòng kiểm tra lại mã lớp học." });

            SemesterResponse? semester = null;
            try
            {
                semester = await GetSemesterByDateAsync(academicHold.HoldDate.ToString("yyyy-MM-dd"));
            }
            catch (BadRequestException ex)
            {
                errors.Add(new ValidationError { Field = "HoldDate", Error = ex.Message });
            }

            if (errors.Any())
            {
                throw new BadRequestException("Cập nhật bảo lưu thất bại do dữ liệu không hợp lệ.", errors);
            }

            // Kiểm tra AcademicHold tồn tại trước
            var existingHold = await _context.AcademicHolds
                .Where(ah => ah.Id == academicHold.Id && !ah.IsDelete)
                .FirstOrDefaultAsync();

            if (existingHold == null)
            {
                errors.Add(new ValidationError { Field = "Id", Error = "Bản ghi bảo lưu không tồn tại hoặc đã bị xóa." });
                throw new BadRequestException("Cập nhật bảo lưu thất bại.", errors);
            }

            // Upload file nếu có
            if (!string.IsNullOrEmpty(academicHold.FileName))
            {
                if (!string.IsNullOrEmpty(existingHold.FileName))
                {
                    try
                    {
                        await _cloudinaryService.DeleteFileByUrlAsync(existingHold.FileName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi khi xóa file cũ: {ex.Message}");
                    }
                }

                existingHold.FileName = await _cloudinaryService.UploadDocxAsync(academicHold.FileName);
            }

            // Cập nhật AcademicHold
            existingHold.UserId = academicHold.UserId;
            existingHold.HoldDate = academicHold.HoldDate;
            existingHold.HoldDuration = academicHold.HoldDuration;
            existingHold.Reason = academicHold.Reason;
            existingHold.UserUpdate = userId;

            _context.AcademicHolds.Update(existingHold);
            if (user != null)
            {
                user.StudentStatusId = 2;
                _context.Users.Update(user);
            }
            await _context.SaveChangesAsync();

            return new AcademicHoldResponse
            {
                Id = existingHold.Id,
                UserId = existingHold.UserId,
                HoldDate = existingHold.HoldDate,
                HoldDuration = existingHold.HoldDuration,
                Reason = existingHold.Reason,
                FileName = existingHold.FileName
            };
        }
        public async Task<bool> DeleteAcademicHold(int id)
        {
            await _academicHoldRepository.DeleteAsync(id);
            return true;
        }

        public async Task<List<User_AcademicHoldsResponse>> GetAllUserName()
        {
            return await _context.Users
                .Where(u => !(u.IsDelete ?? false))
                .Select(u => new User_AcademicHoldsResponse
                {
                    Id = u.Id,
                    FullName = u.FullName ?? string.Empty,
                })
                .ToListAsync();
        }
        public async Task<List<User_AcademicHoldsResponse>> SearchUsersByCriteriaAsync(int classId)
        {
            var query = _context.ClassStudents
                .Include(cs => cs.User)
                .Include(cs => cs.Class)
                .ThenInclude(c => c != null ? c.AcademicYear : null)
                .Where(cs => cs.Class != null && cs.ClassId == classId && cs.IsDelete == false);

            var users = await query
                .Select(cs => new User_AcademicHoldsResponse
                {
                    Id = cs.UserId ?? 0,
                    FullName = cs.User != null && cs.User.FullName != null ? cs.User.FullName : "Unknown"
                })
                .ToListAsync();

            return users;
        }

        public async Task<SemesterResponse?> GetSemesterByDateAsync(string dateString)
        {
            if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                throw new BadRequestException("Ngày bạn nhập không đúng định dạng. Vui lòng nhập theo định dạng 'năm-tháng-ngày' (ví dụ: 2025-06-05).");
            }

            DateTime normalizedDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 0, 0, 0);

            var now = DateTime.Now;
            var currentAcademicYear = await _context.AcademicYears
                .Where(ay => ay.StartDate <= now && ay.EndDate >= now && ay.IsDelete == false)
                .FirstOrDefaultAsync();

            // Nếu không có niên khóa hiện tại (đang trong kỳ nghỉ hè), tìm niên khóa gần nhất và niên khóa tiếp theo
            AcademicYear? nearestAcademicYear = null;
            AcademicYear? nextAcademicYear = null;

            if (currentAcademicYear == null)
            {
                // Tìm niên khóa gần nhất trước đó (dựa trên EndDate gần nhất trước now)
                nearestAcademicYear = await _context.AcademicYears
                    .Where(ay => ay.EndDate < now && ay.IsDelete == false)
                    .OrderByDescending(ay => ay.EndDate)
                    .FirstOrDefaultAsync();

                // Tìm niên khóa tiếp theo (dựa trên StartDate gần nhất sau now)
                nextAcademicYear = await _context.AcademicYears
                    .Where(ay => ay.StartDate > now && ay.IsDelete == false)
                    .OrderBy(ay => ay.StartDate)
                    .FirstOrDefaultAsync();

                if (nearestAcademicYear == null && nextAcademicYear == null)
                {
                    throw new BadRequestException("Hiện tại không có năm học nào trong hệ thống. Vui lòng liên hệ quản trị viên để kiểm tra.");
                }
            }
            else
            {
                // Nếu có niên khóa hiện tại, gán nó làm niên khóa gần nhất
                nearestAcademicYear = currentAcademicYear;
            }

            // Kiểm tra ngày bảo lưu không được nằm trong niên khóa trước niên khóa gần nhất
            if (nearestAcademicYear != null && normalizedDate < nearestAcademicYear.StartDate)
            {
                throw new BadRequestException("Ngày bạn chọn nằm trong quá khứ (trước năm học gần nhất). Vui lòng chọn một ngày từ năm học hiện tại hoặc tương lai.");
            }

            // Tìm niên khóa mà date thuộc về (có thể là niên khóa hiện tại, gần nhất, hoặc tiếp theo)
            var targetAcademicYear = await _context.AcademicYears
                .Where(ay => ay.StartDate <= normalizedDate && ay.EndDate >= normalizedDate && ay.IsDelete == false)
                .FirstOrDefaultAsync();

            if (targetAcademicYear == null)
            {
                // Nếu date không thuộc niên khóa nào (có thể là trong kỳ nghỉ hè hoặc tương lai),
                // tìm niên khóa tiếp theo gần nhất sau date
                targetAcademicYear = await _context.AcademicYears
                    .Where(ay => ay.StartDate > normalizedDate && ay.IsDelete == false)
                    .OrderBy(ay => ay.StartDate)
                    .FirstOrDefaultAsync();

                if (targetAcademicYear == null)
                {
                    throw new BadRequestException("Ngày bạn chọn không nằm trong bất kỳ năm học nào, và cũng không có năm học nào trong tương lai. Vui lòng liên hệ quản trị viên để kiểm tra.");
                }
            }

            // Chuẩn hóa StartDate và EndDate trong truy vấn để bỏ múi giờ
            var semester = await _context.Semesters
                .Where(s => s.AcademicYearId == targetAcademicYear.Id
                         && s.StartDate.HasValue && s.StartDate.Value.Date <= normalizedDate
                         && s.EndDate.HasValue && s.EndDate.Value.Date >= normalizedDate
                         && s.IsDelete == false)
                .FirstOrDefaultAsync();

            if (semester == null)
            {
                throw new BadRequestException("Ngày bạn chọn không nằm trong bất kỳ khoảng thời gian học nào của năm học. Vui lòng chọn một ngày khác hoặc liên hệ quản trị viên để kiểm tra.");
            }

            return semester == null && targetAcademicYear == null ? null : new SemesterResponse
            {
                Id = semester?.Id ?? 0,
                Name = semester?.Name ?? string.Empty
            };
        }
    }
}
