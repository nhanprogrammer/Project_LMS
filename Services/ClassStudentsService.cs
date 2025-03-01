using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Project_LMS.Services
{
    public class ClassStudentsService : IClassStudentsService
    {
        private readonly IClassStudentRepository _classStudentRepository;

        public ClassStudentsService(IClassStudentRepository classStudentRepository)
        {
            _classStudentRepository = classStudentRepository;
        }

        // GET: api/ClassStudent
        public async Task<ApiResponse<IEnumerable<ClassStudent>>> GetAllAsync()
        {
            try
            {
                var classStudents = await _classStudentRepository.GetAllAsync();
                return new ApiResponse<IEnumerable<ClassStudent>>(0, "Lấy tất cả lớp học sinh thành công.", classStudents);
            }
            catch (System.Exception ex)
            {
                return new ApiResponse<IEnumerable<ClassStudent>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        // GET: api/ClassStudent/{id}
        public async Task<ApiResponse<ClassStudent>> GetByIdAsync(int id)
        {
            try
            {
                var classStudent = await _classStudentRepository.GetByIdAsync(id);
                if (classStudent == null)
                {
                    return new ApiResponse<ClassStudent>(1, "Không tìm thấy lớp học sinh.", null);
                }
                return new ApiResponse<ClassStudent>(0, "Lấy lớp học sinh thành công.", classStudent);
            }
            catch (System.Exception ex)
            {
                return new ApiResponse<ClassStudent>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        // POST: api/ClassStudent
        public async Task<ApiResponse<object>> AddAsync(CreateClassStudentRequest request)
        {
            try
            {
                var newClassStudent = new ClassStudent
                {
                    ClassId = request.ClassId,
                    StudentId = request.StudentId
                };

                await _classStudentRepository.AddAsync(newClassStudent);
                return new ApiResponse<object>(0, "Lớp học sinh được tạo thành công.", newClassStudent); // Trả về đối tượng đã tạo
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null); // Trả về lỗi nếu có
            }
        }

        public async Task<ApiResponse<object>> UpdateAsync(UpdateClassStudentRequest request)
        {
            try
            {
                var classStudent = await _classStudentRepository.GetByIdAsync(request.Id);
                if (classStudent == null)
                {
                    return new ApiResponse<object>(1, "Không tìm thấy lớp học sinh.", null); // Lỗi: không tìm thấy
                }

                // Cập nhật thông tin
                classStudent.ClassId = request.ClassId ?? classStudent.ClassId;
                classStudent.StudentId = request.StudentId ?? classStudent.StudentId;

                await _classStudentRepository.UpdateAsync(classStudent);

                // Trả về thông báo thành công và đối tượng đã cập nhật
                return new ApiResponse<object>(0, "Lớp học sinh đã được cập nhật thành công.", classStudent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null); // Lỗi: nếu có lỗi xảy ra
            }
        }


        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            try
            {
                var classStudent = await _classStudentRepository.GetByIdAsync(id);
                if (classStudent == null)
                {
                    return new ApiResponse<object>(1, "Không tìm thấy lớp học sinh.", null); // Trả về lỗi nếu không tìm thấy
                }

                await _classStudentRepository.DeleteAsync(id);
                return new ApiResponse<object>(0, "Lớp học sinh đã được xóa thành công.", null); // Trả về thông báo thành công khi xóa
            }
            catch (System.Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null); // Trả về lỗi nếu có
            }
        }
    }
}