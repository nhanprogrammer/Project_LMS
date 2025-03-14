using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Project_LMS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Project_LMS.Services
{
    public class LessonsService : ILessonsService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<LessonsService> _logger;
        private readonly IMapper _mapper;

        public LessonsService(ILessonRepository lessonRepository, ILogger<LessonsService> logger, IMapper mapper)
        {
            _lessonRepository = lessonRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<LessonResponse>> GetLessonAsync(PaginationRequest request)
        {
            try
            {
                var query = await _lessonRepository.GetQueryable();

                int totalItems = await query.CountAsync();
                int pageSize = request.PageSize > 0 ? request.PageSize : 10;

                var lessonList = await query
                    .Skip((request.PageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PaginatedResponse<LessonResponse>
                {
                    Items = _mapper.Map<List<LessonResponse>>(lessonList),
                    PageNumber = request.PageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber * pageSize < totalItems
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all lessons.");
                return new PaginatedResponse<LessonResponse>
                {
                    Items = new List<LessonResponse>(),
                    PageNumber = 0,
                    PageSize = 0,
                    TotalItems = 0,
                    TotalPages = 0,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
            }
        }

        public async Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest createLessonRequest)
        {
            if (createLessonRequest == null)
            {
                return new ApiResponse<LessonResponse>(1, "Invalid request data.", null);
            }

            try
            {
                var lesson = _mapper.Map<Lesson>(createLessonRequest);
                lesson.CreateAt = DateTime.Now;

                await _lessonRepository.AddAsync(lesson);
                var response = _mapper.Map<LessonResponse>(lesson);

                return new ApiResponse<LessonResponse>(0, "Department đã thêm thành công", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a lesson.");
                return new ApiResponse<LessonResponse>(1, "An error occurred while creating the lesson.", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> UpdateLessonAsync(string id, UpdateLessonRequest updateLessonRequest)
        {
            if (!int.TryParse(id, out int lessonId))
            {
                return new ApiResponse<LessonResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                {
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy lesson.", null);
                }

                _mapper.Map(updateLessonRequest, lesson);
                lesson.UpdateAt = DateTime.Now;

                await _lessonRepository.UpdateAsync(lesson);
                var response = _mapper.Map<LessonResponse>(lesson);

                return new ApiResponse<LessonResponse>(0, "Department đã cập nhật thành công", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the lesson.");
                return new ApiResponse<LessonResponse>(1, "An error occurred while updating the lesson.", null);
            }
        }

        public async Task<ApiResponse<LessonResponse>> DeleteLessonAsync(string id)
        {
            if (!int.TryParse(id, out int lessonId))
            {
                return new ApiResponse<LessonResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                {
                    return new ApiResponse<LessonResponse>(1, "Không tìm thấy lesson.", null);
                }
                lesson.IsDelete = true;
                await _lessonRepository.UpdateAsync(lesson);

                return new ApiResponse<LessonResponse>(0, "Lesson đã xóa thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the lesson.");
                return new ApiResponse<LessonResponse>(1, "An error occurred while deleting the lesson.", null);
            }
        }
    }
}
