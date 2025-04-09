using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicYearsService : IAcademicYearsService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly IStudentRepository _studentRepository;

        public AcademicYearsService(IAcademicYearRepository academicYearRepository,
            ISemesterRepository semesterRepository, IUserRepository userRepository, IMapper mapper, ApplicationDbContext context, IAuthService authService, IClassStudentRepository classStudentRepository, IStudentRepository studentRepository)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
            _authService = authService;
            _classStudentRepository = classStudentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request, string? keyword)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return new PaginatedResponse<AcademicYearResponse>
                {
                    Items = new List<AcademicYearResponse>(),
                    PageNumber = 0,
                    PageSize = 0,
                    TotalItems = 0,
                    TotalPages = 0,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
            }

            var query = _academicYearRepository.GetQueryable();

            // Nếu có keyword, lọc theo year
            if (!string.IsNullOrEmpty(keyword))
            {
                if (int.TryParse(keyword, out int year))
                {
                    query = query.Where(a => (a.StartDate.HasValue && a.StartDate.Value.Year == year) ||
                                               (a.EndDate.HasValue && a.EndDate.Value.Year == year));
                }
            }

            int totalItems = await query.CountAsync();

            int pageSize = request.PageSize > 0 ? request.PageSize : 10;

            var academicYearsList = await query
                .Skip((request.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AcademicYearResponse>
            {
                Items = _mapper.Map<List<AcademicYearResponse>>(academicYearsList),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber * pageSize < totalItems
            };
        }
        public async Task<PaginatedResponse<AcademicYearResponse>> SearchAcademicYear(int year, int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _academicYearRepository.GetQueryable()
                .Where(a => a.StartDate.Value.Year == year || a.EndDate.Value.Year == year);

            int totalItems = await query.CountAsync();

            var academicYearsList = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AcademicYearResponse>
            {
                Items = _mapper.Map<List<AcademicYearResponse>>(academicYearsList),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber * pageSize < totalItems
            };
        }


        public async Task<ApiResponse<AcademicYearWithSemestersDto>> GetByIdAcademicYear(int id)
        {
            try
            {
                var academicYear = await _academicYearRepository.GetByIdAcademicYearAsync(id); // Giả sử dùng hàm này
                if (academicYear == null)
                {
                    return new ApiResponse<AcademicYearWithSemestersDto>(1, $"Không tìm thấy niên khóa với ID: {id}");
                }
                return new ApiResponse<AcademicYearWithSemestersDto>(0, "Tìm thấy niên khóa thành công!", academicYear);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                return new ApiResponse<AcademicYearWithSemestersDto>(1, $"Đã xảy ra lỗi khi lấy niên khóa với ID: {id}. Chi tiết: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AcademicYearResponse>> AddAcademicYear(CreateAcademicYearRequest request,
            int userId)
        {
            var user = await _userRepository.FindAsync(userId);

            if (user == null)
            {
                return new ApiResponse<AcademicYearResponse>(1, "User không tồn tại.");
            }

            var extistingAcademicYear = await _academicYearRepository.GetAllAsync();
            if (extistingAcademicYear.Any(x =>
                    x.StartDate.HasValue && x.StartDate.Value.Year == request.StartDate.Year &&
                    x.EndDate.HasValue && x.EndDate.Value.Year == request.EndDate.Year))
            {
                return new ApiResponse<AcademicYearResponse>(1, "Niên khóa đã tồn tại.");
            }

            if (extistingAcademicYear.Any(x =>
                    x.StartDate.HasValue && x.EndDate.HasValue &&
                    !(x.EndDate.Value < request.StartDate.ToDateTime(TimeOnly.MinValue) ||
                      x.StartDate.Value > request.EndDate.ToDateTime(TimeOnly.MinValue))))
            {
                return new ApiResponse<AcademicYearResponse>(1,
                    $"Niên khóa có năm `{request.StartDate.Year}` bị chồng lấn với Niên khóa cũ.");
            }

            var academicYear = new AcademicYear
            {
                UserCreate = userId,
                CreateAt = TimeHelper.Now
            };

            // Kế thưa niên khóa
            if (request.IsInherit.HasValue && request.IsInherit.Value && request.AcademicParent.HasValue)
            {
                bool isExist = await _academicYearRepository.IsAcademicYearExist(request.AcademicParent.Value);

                if (!isExist)
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Niên khóa kế thừa không tồn tại.");
                }

                academicYear.AcademicParent = request.AcademicParent.Value;
            }

            _mapper.Map(request, academicYear);


            if (request.StartDate > request.EndDate)
            {
                return new ApiResponse<AcademicYearResponse>(1,
                    "Ngày kết thúc của Niên Khóa không thể thấp hơn ngày bắt đầu.");
            }

            if (request.Semesters != null && request.Semesters.Any())
            {
                if (request.Semesters.Count > 2)
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Mỗi Niên Khóa chỉ được phép tối đa 2 học kỳ.");
                }

                if (request.Semesters == null || !request.Semesters.Any())
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Danh sách học kỳ không hợp lệ.");
                }

                foreach (var semesterRequest in request.Semesters)
                {
                    if (semesterRequest.DateStart < request.StartDate)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"`Ngày bắt đầu` của Niên Khóa không thể thấp hơn `Ngày bắt đầu` của {semesterRequest.Name}");
                    }

                    if (semesterRequest.DateStart > semesterRequest.DateEnd)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"`Ngày kết thúc` của {semesterRequest.Name} không thể thấp hơn `Ngày bắt đầu`.");
                    }

                    if (semesterRequest.DateEnd > request.EndDate)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"`Ngày kết thúc` của Niên Khóa không thể thấp hơn `Ngày kết thúc` của {semesterRequest.Name}");
                    }
                }

                var sortedSemesters = request.Semesters.OrderBy(s => s.DateStart).ToList();

                for (int i = 0; i < sortedSemesters.Count - 1; i++)
                {
                    var currentSemester = sortedSemesters[i];
                    var nextSemester = sortedSemesters[i + 1];

                    if (currentSemester.Name == nextSemester.Name)
                    {
                        return new ApiResponse<AcademicYearResponse>(1, "Tên học kỳ không thể trùng nhau.");
                    }

                    if (currentSemester.DateEnd > nextSemester.DateStart)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"Thời gian của {currentSemester.Name} không hợp lệ. " +
                            $"Ngày kết thúc ({currentSemester.DateEnd:yyyy-MM-dd}) " +
                            $"không thể lớn hơn Ngày bắt đầu của {nextSemester.Name} ({nextSemester.DateStart:yyyy-MM-dd}).");
                    }
                }

                academicYear.Semesters = _mapper.Map<List<Semester>>(request.Semesters);

                foreach (var semester in academicYear.Semesters)
                {
                    semester.UserCreate = userId;
                    semester.CreateAt = TimeHelper.Now;
                }

                await _semesterRepository.AddRangeAsync(academicYear.Semesters);
            }


            await _academicYearRepository.AddAsync(academicYear);
            var response = _mapper.Map<AcademicYearResponse>(academicYear);
            return new ApiResponse<AcademicYearResponse>(0, "Niên khóa đã được thêm thành công", response);
        }

        public async Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest request, int userId)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(request.Id);
            if (academicYear == null)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Niên khóa không tồn tại.");
            }

            // Kiểm tra niên khóa cha nếu có
            if (request.IsInherit.HasValue && request.IsInherit.Value && request.AcademicParent.HasValue)
            {
                if (request.AcademicParent.Value == request.Id)
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Niên khóa không thể kế thừa chính nó.");
                }

                bool isExist = await _academicYearRepository.IsAcademicYearExist(request.AcademicParent.Value);
                if (!isExist)
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Niên khóa kế thừa không tồn tại.");
                }
                academicYear.AcademicParent = request.AcademicParent.Value;
            }
            else
            {
                academicYear.AcademicParent = null;
            }

            // Cập nhật thông tin niên khóa
            academicYear.StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
            academicYear.EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue);
            academicYear.IsInherit = request.IsInherit;
            academicYear.UserUpdate = userId;
            academicYear.UpdateAt = TimeHelper.Now;
            academicYear.IsDelete = false;

            // Kiểm tra chồng lấn thời gian với các niên khóa khác
            var existingAcademicYears = await _academicYearRepository.GetAllAsync();
            var conflictingAcademicYear = existingAcademicYears.FirstOrDefault(x =>
                x.Id != request.Id &&
                x.StartDate.HasValue && x.EndDate.HasValue &&
                !(x.EndDate.Value < request.StartDate.ToDateTime(TimeOnly.MinValue) ||
                  x.StartDate.Value > request.EndDate.ToDateTime(TimeOnly.MinValue)));

            if (conflictingAcademicYear != null)
            {
                return new ApiResponse<AcademicYearResponse>(1,
                    $"Niên khóa có thời gian từ {request.StartDate:yyyy-MM-dd} đến {request.EndDate:yyyy-MM-dd} " +
                    $"bị chồng lấn với niên khóa ID {conflictingAcademicYear.Id} " +
                    $"(từ {conflictingAcademicYear.StartDate:yyyy-MM-dd} đến {conflictingAcademicYear.EndDate:yyyy-MM-dd}).");
            }

            // Kiểm tra thời gian tối thiểu của niên khóa
            if ((request.EndDate.ToDateTime(TimeOnly.MinValue) - request.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays < 270) // 9 tháng
            {
                return new ApiResponse<AcademicYearResponse>(1, "Thời gian của niên khóa phải kéo dài ít nhất 9 tháng.");
            }

            // Lấy danh sách học kỳ hiện có và ngắt theo dõi
            var existingSemesters = await _semesterRepository.GetByAcademicYearIdAsync(request.Id);
            foreach (var semester in existingSemesters)
            {
                _context.Entry(semester).State = EntityState.Detached;
            }

            // Đếm số học kỳ hiện có (IsDelete == false)
            var currentSemesterCount = existingSemesters.Count(s => !s.IsDelete.Value);

            var updatedSemesters = new List<Semester>();
            var newSemesters = new List<Semester>();

            // Xử lý các học kỳ
            foreach (var semesterRequest in request.Semesters)
            {
                if (semesterRequest.Id > 0)
                {
                    var existingSemester = existingSemesters.FirstOrDefault(s => s.Id == semesterRequest.Id);
                    if (existingSemester != null)
                    {
                        _mapper.Map(semesterRequest, existingSemester);
                        existingSemester.UserUpdate = userId;
                        existingSemester.UpdateAt = TimeHelper.Now;
                        updatedSemesters.Add(existingSemester);
                    }
                    else
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"Học kỳ với ID {semesterRequest.Id} không tồn tại.");
                    }
                }
                else if (semesterRequest.Id == 0)
                {
                    // Kiểm tra giới hạn 2 học kỳ
                    if (currentSemesterCount + newSemesters.Count >= 2)
                    {
                        return new ApiResponse<AcademicYearResponse>(1, "Niên khóa đã đạt giới hạn 2 học kỳ, không thể thêm mới.");
                    }

                    // Tạo học kỳ mới
                    var newSemester = new Semester
                    {
                        AcademicYearId = request.Id,
                        Name = semesterRequest.Name,
                        StartDate = semesterRequest.DateStart.ToDateTime(TimeOnly.MinValue),
                        EndDate = semesterRequest.DateEnd.ToDateTime(TimeOnly.MinValue),
                        CreateAt = TimeHelper.Now,
                        UserCreate = userId,
                        UpdateAt = TimeHelper.Now,
                        UserUpdate = userId,
                        IsDelete = false
                    };

                    // Kiểm tra thời gian học kỳ mới có nằm trong niên khóa không
                    if (newSemester.StartDate < academicYear.StartDate || newSemester.EndDate > academicYear.EndDate)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"Thời gian của {newSemester.Name} không hợp lệ với Niên Khóa.");
                    }

                    // Kiểm tra chồng lấn với các học kỳ hiện có
                    var overlappingSemester = existingSemesters.Any(s =>
                        !s.IsDelete.Value &&
                        s.StartDate.HasValue && s.EndDate.HasValue &&
                        !(s.EndDate.Value < newSemester.StartDate || s.StartDate.Value > newSemester.EndDate));

                    if (overlappingSemester)
                    {
                        return new ApiResponse<AcademicYearResponse>(1,
                            $"Thời gian của {newSemester.Name} trùng lấn với học kỳ hiện có.");
                    }

                    newSemesters.Add(newSemester);
                }
            }

            // Kiểm tra chồng lấn giữa tất cả các học kỳ (hiện có và mới)
            var allSemesters = existingSemesters.Where(s => !s.IsDelete.Value).Concat(newSemesters).ToList();
            var sortedSemesters = allSemesters.OrderBy(s => s.StartDate).ToList();
            for (int i = 0; i < sortedSemesters.Count - 1; i++)
            {
                var currentSemester = sortedSemesters[i];
                var nextSemester = sortedSemesters[i + 1];
                if (currentSemester.EndDate > nextSemester.StartDate)
                {
                    return new ApiResponse<AcademicYearResponse>(1,
                        $"Thời gian của {currentSemester.Name} không hợp lệ. " +
                        $"Ngày kết thúc ({currentSemester.EndDate:yyyy-MM-dd}) " +
                        $"không thể lớn hơn ngày bắt đầu của {nextSemester.Name} ({nextSemester.StartDate:yyyy-MM-dd}).");
                }
            }

            // Cập nhật niên khóa và học kỳ
            try
            {
                await _academicYearRepository.UpdateAsync(academicYear);
                if (updatedSemesters.Any())
                {
                    await _semesterRepository.UpdateRangeAsync(updatedSemesters);
                }
                if (newSemesters.Any())
                {
                    await _semesterRepository.AddRangeAsync(newSemesters);
                }

                var response = _mapper.Map<AcademicYearResponse>(academicYear);
                response.Semesters = _mapper.Map<List<SemesterResponse>>(allSemesters);
                return new ApiResponse<AcademicYearResponse>(0, "Niên khóa đã được cập nhật thành công", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AcademicYearResponse>(1, $"Lỗi khi cập nhật niên khóa: {ex.Message}");
            }
        }
        public async Task<ApiResponse<AcademicYearResponse>> DeleteLessonAsync(DeleteRequest deleteRequest)
        {
            if (deleteRequest == null || deleteRequest.ids == null || deleteRequest.ids.Count == 0)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Danh sách ID rỗng. Vui lòng kiểm tra lại.", null);
            }

            try
            {
                var academicYears = await _academicYearRepository.GetByIdsAsync(deleteRequest.ids);

                if (academicYears == null || academicYears.Count == 0)
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Không tìm thấy niên khóa nào cần xóa.", null);
                }

                var alreadyDeletedIds = new List<int>();

                foreach (var lesson in academicYears)
                {
                    if (lesson.IsDelete.HasValue && lesson.IsDelete.Value)
                    {
                        alreadyDeletedIds.Add(lesson.Id);
                        continue;
                    }

                    lesson.IsDelete = true;
                }

                if (alreadyDeletedIds.Count > 0)
                {
                    return new ApiResponse<AcademicYearResponse>(1,
                        $"Các niên khóa có ID {string.Join(", ", alreadyDeletedIds)} đã bị xóa trước đó.", null);
                }

                await _academicYearRepository.UpdateRangeAsync(academicYears);

                return new ApiResponse<AcademicYearResponse>(0, "Niên khóa đã xóa thành công");
            }
            catch (Exception ex)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Xóa niên khóa thất bại.", null);
            }
        }

        public async Task<List<AcademicYearNameResponse>> GetAcademicYearNamesAsync()
        {
            var academicYears = await _academicYearRepository.GetQueryable()
                .Where(ay => ay.IsDelete == null || ay.IsDelete == false)
                .OrderBy(ay => ay.StartDate)
                .ToListAsync();

            var result = academicYears.Select(ay => new AcademicYearNameResponse
            {
                Id = ay.Id,
                Name = ay.StartDate.HasValue && ay.EndDate.HasValue && ay.StartDate.Value.Year == ay.EndDate.Value.Year
                    ? $"{ay.StartDate.Value.Year}"
                    : $"{ay.StartDate?.Year}-{ay.EndDate?.Year}"
            }).ToList();

            return result;
        }
        public async Task<ApiResponse<List<AcademicYearNameResponse>>> DropdownAcademicYearsForStudent()
        {
            try
            {
                int studentId = 0;

                // Lấy thông tin người dùng từ AuthService
                var user = await _authService.GetUserAsync();
                if (user != null)
                {
                    var studentAuth = await _studentRepository.FindStudentById(user.Id);
                    if (studentAuth != null && studentAuth.Role.Name == "Student")
                    {
                        studentId = studentAuth.Id;
                    }
                }

                if (studentId == 0)
                {
                    return new ApiResponse<List<AcademicYearNameResponse>>(1, "Không tìm thấy học sinh hoặc học sinh chưa đăng nhập.", null);
                }

                // Lấy tất cả ClassStudent của học sinh
                var classStudents = await _classStudentRepository.FindAllClassStudentByUserId(studentId);

                // Lọc và sắp xếp ClassStudent: chỉ lấy IsActive == true, IsDelete == false, và sắp xếp theo năm học mới nhất
                var filteredClassStudents = classStudents
                    .Where(cs => cs.IsActive == true && (cs.IsDelete == null || cs.IsDelete == false))
                    .OrderByDescending(cs => cs.Class?.AcademicYear?.EndDate)
                    .ToList();

                // Lấy danh sách niên khóa (id và name)
                var academicYears = filteredClassStudents
                    .Select(cs => cs.Class?.AcademicYear)
                    .Where(ay => ay != null)
                    .Distinct()
                    .Select(ay => new AcademicYearNameResponse
                    {
                        Id = ay.Id,
                        Name = $"{ay.StartDate?.ToString("yyyy")} - {ay.EndDate?.ToString("yyyy")}"
                    })
                    .ToList();

                return new ApiResponse<List<AcademicYearNameResponse>>(0, "Lấy danh sách niên khóa thành công.", academicYears);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AcademicYearNameResponse>>(1, $"Đã xảy ra lỗi: {ex.Message}", null);
            }
        }
    }
}