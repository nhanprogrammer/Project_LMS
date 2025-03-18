using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class AcademicYearsService : IAcademicYearsService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly ILogger<LessonsService> _logger;
        private readonly IMapper _mapper;

        public AcademicYearsService(IAcademicYearRepository academicYearRepository,  ISemesterRepository semesterRepository, IMapper mapper, ILogger<LessonsService> logger)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
            _logger = logger;
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

        public async Task AddAcademicYear(CreateAcademicYearRequest request)
        {
            var academicYear = _mapper.Map<AcademicYear>(request);
            if (request.Semesters != null && request.Semesters.Any())
            {
                academicYear.Semesters = _mapper.Map<List<Semester>>(request.Semesters);
                foreach (var semester in academicYear.Semesters)
                {
                    await _semesterRepository.AddAsync(semester);
                }
            }
            await _academicYearRepository.AddAsync(academicYear);
        }

        public async Task<ApiResponse<AcademicYearResponse>> UpdateAcademicYear(UpdateAcademicYearRequest request)
        {
            if (!int.TryParse(request.Id.ToString(), out int academicYearId))
            {
                return new ApiResponse<AcademicYearResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }
            try
            {
                var academicYear = _mapper.Map<AcademicYear>(request);
                academicYear.CreateAt = TimeHelper.Now;
                await _academicYearRepository.UpdateAsync(academicYear);
                var response = _mapper.Map<AcademicYearResponse>(academicYear);
                return new ApiResponse<AcademicYearResponse>(0, "Niên khóa đã cập nhật thành công", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating an academic year.");
                return new ApiResponse<AcademicYearResponse>(1, "Cập nhật niên khóa thất bại.", null);
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
                _logger.LogError(ex, "Error occurred while deleting the lessons.");
                return new ApiResponse<AcademicYearResponse>(1, "Xóa niên khóa thất bại.", null);
            }
        }
    }
}