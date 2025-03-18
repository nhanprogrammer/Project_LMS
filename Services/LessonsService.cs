// using Project_LMS.Data;
// using Project_LMS.DTOs.Request;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Interfaces;
// using Project_LMS.Interfaces.Responsitories;
// using Project_LMS.Models;

// namespace Project_LMS.Services
// {
//     public class LessonsService : ILessonsService
//     {
//         private readonly ILessonRepository _lessonRepository;
//         private readonly ApplicationDbContext _context;

//         public LessonsService(ILessonRepository lessonRepository, ApplicationDbContext context)
//         {
//             _lessonRepository = lessonRepository;
//             _context = context;
//         }

//         public async Task<ApiResponse<List<LessonResponse>>> GetAllLessonAsync()
//         {
//             var lessons = await _lessonRepository.GetAllAsync();
//             var data = lessons.Select(c => new LessonResponse
//             {
//                 ClassId = c.ClassId,
//                 TeacherId = c.UserId,
//                 ClassLessonCode = c.ClassLessonCode,
//                 Description = c.Description,
//                 Topic = c.Topic,
//                 StartDate = c.StartDate,
//                 EndDate = c.EndDate,
//                 Duration = c.Duration,
//                 Password = c.PaswordLeassons,
//                 IsSave = c.IsSave,
//                 IsAutoStart = c.IsAutoStart,
//                 IsResearchable = c.IsResearchable,
//             }).ToList();
    
//             return new ApiResponse<List<LessonResponse>>(0, "Fill dữ liệu thành công ", data);
//         }

//         public async Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest createLessonRequest)
//         {
//             var lesson = new Lesson
//             {
//                 ClassId = createLessonRequest.ClassId,
//                 UserId = createLessonRequest.TeacherId,
//                 ClassLessonCode = createLessonRequest.ClassLessonCode,
//                 Description = createLessonRequest.Description,
//                 Topic = createLessonRequest.Topic,
//                 StartDate = DateTime.SpecifyKind(createLessonRequest.StartDate, DateTimeKind.Unspecified),
//                 EndDate = DateTime.SpecifyKind(createLessonRequest.EndDate, DateTimeKind.Unspecified),
//                 Duration = createLessonRequest.Duration,
//                 PaswordLeassons = createLessonRequest.Password,
//                 IsSave = createLessonRequest.IsSave,
//                 IsAutoStart = createLessonRequest.IsAutoStart,
//                 IsResearchable = createLessonRequest.IsResearchable,
//                 CreateAt = DateTime.Now,
//             };
                    
//             await _lessonRepository.AddAsync(lesson);
//             var response = new LessonResponse
//             {
//                 ClassId = lesson.ClassId,
//                 TeacherId = lesson.UserId,
//                 ClassLessonCode = lesson.ClassLessonCode,
//                 Description = lesson.Description,
//                 Topic = lesson.Topic,
//                 StartDate = lesson.StartDate,
//                 EndDate = lesson.EndDate,
//                 Duration = lesson.Duration,
//                 Password = lesson.PaswordLeassons,
//                 IsSave = lesson.IsSave,
//                 IsAutoStart = lesson.IsAutoStart,
//                 IsResearchable = lesson.IsResearchable,
//             };
//             return new ApiResponse<LessonResponse>(0, "Department đã thêm thành công", response);
//         }

//         public async Task<ApiResponse<LessonResponse>> UpdateLessonAsync(string id, UpdateLessonRequest updateLessonRequest)
//         {
//             if (!int.TryParse(id, out int lessonId))
//             {
//                 return new ApiResponse<LessonResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
//             }

//             var lesson = await _lessonRepository.GetByIdAsync(lessonId);
//             if (lesson == null)
//             {
//                 return new ApiResponse<LessonResponse>(1, "Không tìm thấy lesson.", null);
//             }

//             lesson.ClassId = updateLessonRequest.ClassId;
//             lesson.UserId = updateLessonRequest.TeacherId;
//             lesson.ClassLessonCode = updateLessonRequest.ClassLessonCode;
//             lesson.Description = updateLessonRequest.Description;
//             lesson.Topic = updateLessonRequest.Topic;
//             lesson.StartDate = DateTime.SpecifyKind(updateLessonRequest.StartDate, DateTimeKind.Unspecified);
//             lesson.EndDate = DateTime.SpecifyKind(updateLessonRequest.EndDate, DateTimeKind.Unspecified);
//             lesson.Duration = updateLessonRequest.Duration;
//             lesson.PaswordLeassons = updateLessonRequest.Password;
//             lesson.IsSave = updateLessonRequest.IsSave;
//             lesson.IsAutoStart = updateLessonRequest.IsAutoStart;
//             lesson.IsResearchable = updateLessonRequest.IsResearchable;
//             lesson.UpdateAt = DateTime.Now;
    
//             await _lessonRepository.UpdateAsync(lesson);
//             var response = new LessonResponse
//             {
//                 ClassId = lesson.ClassId,
//                 TeacherId = lesson.UserId,
//                 ClassLessonCode = lesson.ClassLessonCode,
//                 Description = lesson.Description,
//                 Topic = lesson.Topic,
//                 StartDate = lesson.StartDate,
//                 EndDate = lesson.EndDate,
//                 Duration = lesson.Duration,
//                 Password = lesson.PaswordLeassons,
//                 IsSave = lesson.IsSave,
//                 IsAutoStart = lesson.IsAutoStart,
//                 IsResearchable = lesson.IsResearchable,
//             };

//             return new ApiResponse<LessonResponse>(0, "Department đã cập nhật thành công", response);
//         }

//         public async Task<ApiResponse<LessonResponse>> DeleteLessonAsync(string id)
//         {
//             if (!int.TryParse(id, out int lessonId))
//             {
//                 return new ApiResponse<LessonResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
//             }

//             var lesson = await _lessonRepository.GetByIdAsync(lessonId);
//             if (lesson == null)
//             {
//                 return new ApiResponse<LessonResponse>(1, "Không tìm thấy lesson.", null);
//             }
//             lesson.IsDelete = true;
//             await _lessonRepository.UpdateAsync(lesson);

//             return new ApiResponse<LessonResponse>(0, "Lesson đã xóa thành công ");
//         }
//     }
// }