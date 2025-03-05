using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class ClassStudentsOnlineService : IClassStudentsOnlineService
    {
        private readonly IClassStudentOnlineRepository _classStudentOnlineRepository;
        private readonly ApplicationDbContext _context;

        public ClassStudentsOnlineService(IClassStudentOnlineRepository classStudentOnlineRepository, ApplicationDbContext context)
        {
            _classStudentOnlineRepository = classStudentOnlineRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<ClassStudentOnlineResponse>>> GetAllClassStudentOnlineAsync()
        {
            var classStudentOnline = await _classStudentOnlineRepository.GetAllAsync();
            var data = classStudentOnline.Select(c => new ClassStudentOnlineResponse
            {
                Id = c.Id,
                ClassId = c.ClassId,
                // StudentName = c.Student.FullName, Sửa code chỗ này
                IsCamera = c.IsCamera,
                IsMuted = c.IsMuted,
                IsAdmin = c.IsAdmin,
                JoinTime = c.JoinTime,
                LeaveTime = c.LeaveTime,
                
            }).ToList();

    
            return new ApiResponse<List<ClassStudentOnlineResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<ClassStudentOnlineResponse>> CreateClassStudentOnlineAsync(CreateClassStudentOnlineRequest createClassStudentOnlineRequest)
        {
            var classStudentOnline = new ClassStudentsOnline
            {
                ClassId = createClassStudentOnlineRequest.ClassId,
                UserId = createClassStudentOnlineRequest.StudentId,
                IsCamera = createClassStudentOnlineRequest.IsCamera,
                 IsMuted = createClassStudentOnlineRequest.IsMuted,
                 IsAdmin = createClassStudentOnlineRequest.IsAdmin,
                 JoinTime = DateTime.Now,
            };
            await _classStudentOnlineRepository.AddAsync(classStudentOnline);

            var response = new ClassStudentOnlineResponse
            {
                Id = classStudentOnline.Id,
                ClassId = classStudentOnline.ClassId,
                // StudentName = classStudentOnline.Student.FullName,
                IsCamera = classStudentOnline.IsCamera,
                IsMuted = classStudentOnline.IsMuted,
                IsAdmin = classStudentOnline.IsAdmin,
                JoinTime = classStudentOnline.JoinTime,
            

            };
            
            return new ApiResponse<ClassStudentOnlineResponse>(0, "thêm thành công", response);
        }

        public async Task<ApiResponse<ClassStudentOnlineResponse>> UpdateClassStudentOnlineAsync(string id, UpdateClassStudentOnlineRequest updateClassStudentOnlineRequest)
        {
            if (!int.TryParse(id, out int classStudentOnlineId))
            {
                return new ApiResponse<ClassStudentOnlineResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var classStudentOnline = await _classStudentOnlineRepository.GetByIdAsync(classStudentOnlineId);
            if (classStudentOnline == null)
            {
                return new ApiResponse<ClassStudentOnlineResponse>(1, "Không tìm thấy", null);
            }
             classStudentOnline.IsCamera = updateClassStudentOnlineRequest.IsCamera;
             classStudentOnline.IsMuted = updateClassStudentOnlineRequest.IsMuted;
             classStudentOnline.IsAdmin = updateClassStudentOnlineRequest.IsAdmin;
             classStudentOnline.LeaveTime = DateTime.Now;
             await _classStudentOnlineRepository.UpdateAsync(classStudentOnline);
             var response = new ClassStudentOnlineResponse
             {
                 Id = classStudentOnline.Id,
                 ClassId = classStudentOnline.ClassId,
                //  StudentName = classStudentOnline.Student.FullName, Sửa code chỗ này
                 IsCamera = classStudentOnline.IsCamera,
                 IsMuted = classStudentOnline.IsMuted,
                 IsAdmin = classStudentOnline.IsAdmin,
                 JoinTime = classStudentOnline.JoinTime,
                 LeaveTime = classStudentOnline.LeaveTime,

             };
            
             return new ApiResponse<ClassStudentOnlineResponse>(0, "Cập nhật thành công", response);
             
        }

        public async Task<ApiResponse<ClassStudentOnlineResponse>> DeleteClassStudentOnlineAsync(string id)
        {
            if (!int.TryParse(id, out int classStudentOnlineId))
            {
                return new ApiResponse<ClassStudentOnlineResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var classStudentOnline = await _classStudentOnlineRepository.GetByIdAsync(classStudentOnlineId);
            if (classStudentOnline == null)
            {
                return new ApiResponse<ClassStudentOnlineResponse>(1, "Không tìm thấy", null);
            }
            
            
            await _classStudentOnlineRepository.UpdateAsync(classStudentOnline);

            return new ApiResponse<ClassStudentOnlineResponse>(0, "đã xóa thành công ");
            
        }
    }
}