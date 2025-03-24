using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public AcademicYearsService(IAcademicYearRepository academicYearRepository,  ISemesterRepository semesterRepository, IUserRepository userRepository, IMapper mapper, ILogger<LessonsService> logger)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<AcademicYearResponse>> GetPagedAcademicYears(PaginationRequest request)
        {
            var query = _academicYearRepository.GetQueryable();

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

        public async Task<AcademicYearResponse> GetByIdAcademicYear(int id)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(id);
            return _mapper.Map<AcademicYearResponse>(academicYear);
        }

        public async Task<ApiResponse<AcademicYearResponse>> AddAcademicYear(CreateAcademicYearRequest request, int userId)
        {
            var user = await _userRepository.FindAsync(userId);
            
            if (user == null)
            {
                return new ApiResponse<AcademicYearResponse>(1, "User không tồn tại.");
            }

            var academicYear = _mapper.Map<AcademicYear>(request);
            academicYear.UserCreate = userId;
            academicYear.CreateAt = TimeHelper.Now;

            if (request.StartDate > request.EndDate)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Ngày kết thúc của Niên Khóa không thể thấp hơn ngày bắt đầu.");
            }

            if (request.Semesters != null && request.Semesters.Any())
            {
                if (request.Semesters == null || !request.Semesters.Any())
                {
                    return new ApiResponse<AcademicYearResponse>(1, "Danh sách học kỳ không hợp lệ.");
                }

                foreach (var semesterRequest in request.Semesters)
                {
                    if (semesterRequest.DateStart < request.StartDate)
                    {
                        return new ApiResponse<AcademicYearResponse>(1, $"`Ngày bắt đầu` của Niên Khóa không thể thấp hơn `Ngày bắt đầu` của {semesterRequest.Name}");
                    }

                    if (semesterRequest.DateStart > semesterRequest.DateEnd)
                    {
                        return new ApiResponse<AcademicYearResponse>(1, $"`Ngày kết thúc` của {semesterRequest.Name} không thể thấp hơn `Ngày bắt đầu`.");
                    }

                    if(semesterRequest.DateEnd > request.EndDate)
                    {
                        return new ApiResponse<AcademicYearResponse>(1, $"`Ngày kết thúc` của Niên Khóa không thể thấp hơn `Ngày kết thúc` của {semesterRequest.Name}");
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

        public async Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest academicYearRequest, int userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<AcademicYearResponse>(1, "User không tồn tại.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearRequest.Id);
            if (academicYear == null)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Niên Khóa không tồn tại.");
            }

            if (academicYearRequest.StartDate > academicYearRequest.EndDate)
            {
                return new ApiResponse<AcademicYearResponse>(1, "Ngày kết thúc của Niên Khóa không thể thấp hơn ngày bắt đầu.");
            }

            _mapper.Map(academicYearRequest, academicYear);
            academicYear.UserUpdate = userId;
            academicYear.UpdateAt = TimeHelper.Now;
            await _academicYearRepository.UpdateAsync(academicYear);

            var existingSemesters = (await _semesterRepository.GetByAcademicYearIdAsync(academicYearRequest.Id)).ToList();
            var updatedSemesters = new List<Semester>();
            var newSemesters = new List<Semester>();

            var sortedSemesters = academicYearRequest.Semesters.OrderBy(s => s.DateStart).ToList();
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
                        $"không thể lớn hơn ngày bắt đầu của {nextSemester.Name} ({nextSemester.DateStart:yyyy-MM-dd}).");
                }
            }

            foreach (var semester in academicYearRequest.Semesters)
            {
                var existingSemester = existingSemesters.FirstOrDefault(s => s.Id == semester.Id);

                if (semester.DateStart > semester.DateEnd)
                {
                    return new ApiResponse<AcademicYearResponse>(1, $"Ngày kết thúc của {semester.Name} không thể thấp hơn ngày bắt đầu.");
                }

                if (semester.DateStart < academicYearRequest.StartDate || semester.DateEnd > academicYearRequest.EndDate)
                {
                    return new ApiResponse<AcademicYearResponse>(1, $"Thời gian của {semester.Name} không hợp lệ với Niên Khóa.");
                }

                if (existingSemester != null)
                {
                    _mapper.Map(semester, existingSemester);
                    existingSemester.AcademicYearId = academicYearRequest.Id;
                    existingSemester.UserUpdate = userId;
                    existingSemester.UpdateAt = TimeHelper.Now;
                    updatedSemesters.Add(existingSemester);
                }
                else
                {
                    var newSemester = _mapper.Map<Semester>(semester);
                    newSemester.Id = 0;
                    newSemester.AcademicYearId = academicYearRequest.Id;
                    newSemester.UserCreate = userId;
                    newSemester.CreateAt = TimeHelper.Now;
                    newSemesters.Add(newSemester);
                }
            }

            var semesterIdsFromRequest = academicYearRequest.Semesters.Where(s => s.Id > 0).Select(s => s.Id).ToList();
            var semestersToDelete = existingSemesters.Where(s => !semesterIdsFromRequest.Contains(s.Id)).ToList();
            if (semestersToDelete.Any())
            {
                await _semesterRepository.DeleteRangeAsync(semestersToDelete);
            }

            if (updatedSemesters.Any())
            {
                await _semesterRepository.UpdateRangeAsync(updatedSemesters);
            }

            if (newSemesters.Any())
            {
                await _semesterRepository.AddRangeAsync(newSemesters);
            }

            var response = _mapper.Map<AcademicYearResponse>(academicYear);
            return new ApiResponse<AcademicYearResponse>(0, "Niên khóa đã cập nhật thành công", response);
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
    }
}