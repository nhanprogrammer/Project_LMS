using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class ClassOnlineService : IClassOnlineService
    {
        private readonly IClassOnlineRepository _classOnlineRepository;
        private readonly ApplicationDbContext _context;

        public ClassOnlineService(IClassOnlineRepository classOnlineRepository, ApplicationDbContext context)
        {
            _classOnlineRepository = classOnlineRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<ClassOnlineResponse>>> GetAllClassOnlineAsync()
        {
            var classes = await _classOnlineRepository.GetAllAsync();
            var data = classes.Select(c => new ClassOnlineResponse
            {
                Id = c.Id,
                // TeacherName = c.Teacher.FullName, Sửa code chỗ này
                ClassCode = c.ClassCode,
                ClassDescription = c.ClassDescription,
                // ClassLink = c.ClassLink,
                ClassStatus = c.ClassStatus,
                ClassTitle = c.ClassTitle,
                MaxStudents = c.MaxStudents,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CurrentStudents = c.CurrentStudents,
            }).ToList();

            return new ApiResponse<List<ClassOnlineResponse>>(0, "Lấy dữ liệu lớp học trực tuyến thành công", data);
        }

        public async Task<ApiResponse<ClassOnlineResponse>> CreateClassStudentAsync(CreateClassOnlineRequest createClassStudentRequest)
        {
            var classOnline = new ClassOnline
            {
                ClassCode = createClassStudentRequest.ClassCode,
                ClassDescription = createClassStudentRequest.ClassDescription,
                ClassTitle = createClassStudentRequest.ClassTitle,
                MaxStudents = createClassStudentRequest.MaxStudents,
                StartDate = DateTime.SpecifyKind(createClassStudentRequest.StartDate, DateTimeKind.Unspecified),
                EndDate = DateTime.SpecifyKind((DateTime)createClassStudentRequest.EndDate, DateTimeKind.Unspecified),
                ClassStatus = createClassStudentRequest.ClassStatus,
                ClassPassword = createClassStudentRequest.ClassPassword,
                UserId = createClassStudentRequest.TeacherId,
                CreateAt = DateTime.Now,
            };
            await _classOnlineRepository.AddAsync(classOnline);
            
            var response = new ClassOnlineResponse
            {
                Id = classOnline.Id,
                // TeacherName = classOnline.Teacher.FullName, Sửa code chỗ này
                ClassCode = classOnline.ClassCode,
                ClassDescription = classOnline.ClassDescription,
                ClassStatus = classOnline.ClassStatus,
                ClassTitle = classOnline.ClassTitle,
                MaxStudents = classOnline.MaxStudents,
                StartDate = classOnline.StartDate,
                EndDate = classOnline.EndDate,
                CurrentStudents =  classOnline.CurrentStudents,
            };
            
            return new ApiResponse<ClassOnlineResponse>(0, "thêm thành công", response);

        }

        public async Task<ApiResponse<ClassOnlineResponse>> UpdateClassStudentAsync(string id, UpdateClassOnlineRequest updateClassStudentRequest)
        {
            int classOnlineId = int.Parse(id);
            var classOnline = await _classOnlineRepository.GetByIdAsync(classOnlineId);
            classOnline.ClassCode = updateClassStudentRequest.ClassCode;
            classOnline.ClassDescription = updateClassStudentRequest.ClassDescription;
            classOnline.ClassTitle = updateClassStudentRequest.ClassTitle;
            classOnline.MaxStudents = updateClassStudentRequest.MaxStudents;
            classOnline.StartDate = DateTime.SpecifyKind(updateClassStudentRequest.StartDate, DateTimeKind.Unspecified);
            classOnline.EndDate = DateTime.SpecifyKind(updateClassStudentRequest.EndDate, DateTimeKind.Unspecified);
            classOnline.ClassStatus = updateClassStudentRequest.ClassStatus;
            classOnline.ClassPassword = updateClassStudentRequest.ClassPassword;
            // classOnline.TeacherId = updateClassStudentRequest.TeacherId; Sửa code chỗ này
            await _classOnlineRepository.UpdateAsync(classOnline);
            var response = new ClassOnlineResponse
            {
                Id = classOnline.Id,
                // TeacherName = classOnline.Teacher.FullName, Sửa code chỗ này
                ClassCode = classOnline.ClassCode,
                ClassDescription = classOnline.ClassDescription,
                ClassStatus = classOnline.ClassStatus,
                ClassTitle = classOnline.ClassTitle,
                MaxStudents = classOnline.MaxStudents,
                StartDate = classOnline.StartDate,
                EndDate = classOnline.EndDate,
                CurrentStudents =  classOnline.CurrentStudents,
            };
            
            return new ApiResponse<ClassOnlineResponse>(0, "thêm thành công", response);
        }

        public async Task<ApiResponse<ClassOnlineResponse>> DeleteClassStudentAsync(string id)
        {
            int classOnlineId = int.Parse(id);
            var classOnline = await _classOnlineRepository.GetByIdAsync(classOnlineId);
            classOnline.IsDelete = true;
            await _classOnlineRepository.UpdateAsync(classOnline);

            return new ApiResponse<ClassOnlineResponse>(0, "đã xóa thành công ");
        }
    }
}