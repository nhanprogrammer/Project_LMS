using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;

        public ClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<Class>>> GetAllClassesAsync()
        {
            var classes = await _context.Classes.ToListAsync();
            return new ApiResponse<IEnumerable<Class>>(0, "Lấy danh sách lớp học thành công", classes);
        }

        public async Task<ApiResponse<Class>> GetClassByIdAsync(int id)
        {
            var classItem = await _context.Classes.FindAsync(id);
            if (classItem == null)
                return new ApiResponse<Class>(1, "Không tìm thấy lớp học");

            return new ApiResponse<Class>(0, "Lấy thông tin lớp học thành công", classItem);
        }

        public async Task<ClassDto> CreateClassAsync(CreateClassRequest request)
        {
            var newClass = new Class
            {
                AcademicYearId = request.AcademicYearId,
                DepartmentId = request.DepartmentId,
                ClassTypeId = request.ClassTypeId,
                Description = request.Description,
                ClassCode = request.ClassCode,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                IsDelete = false,
                UserCreate = request.UserCreate
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return MapToDto(newClass);
        }

        public async Task<ApiResponse<Class>> UpdateClassAsync(int id, UpdateClassRequest request)
        {
            // Tìm lớp học theo ID
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return new ApiResponse<Class>(1, "Lớp học không tồn tại");
            }

            // Kiểm tra nếu ClassCode bị null hoặc rỗng
            if (string.IsNullOrEmpty(request.ClassCode))
            {
                return new ApiResponse<Class>(1, "ClassCode không được để trống");
            }

            // Cập nhật thông tin lớp học
            classEntity.AcademicYearId = request.AcademicYearId;
            classEntity.ClassTypeId = request.ClassTypeId;
            classEntity.ClassCode = request.ClassCode;
            classEntity.Description = request.Description;
            classEntity.UpdateAt = DateTime.UtcNow;
            classEntity.UserUpdate = request.UserUpdate;

            _context.Classes.Update(classEntity);
            await _context.SaveChangesAsync();

            return new ApiResponse<Class>(0, "Cập nhật lớp học thành công", classEntity);
        }

        public async Task<ApiResponse<bool>> DeleteClassAsync(int id)
        {
            // Tìm lớp học theo ID
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return new ApiResponse<bool>(1, "Lớp học không tồn tại", false);
            }

            // Nếu có cờ `IsDelete`, chỉ cập nhật thay vì xóa cứng
            classEntity.IsDelete = true;
            _context.Classes.Update(classEntity);

            // Nếu muốn xóa cứng, dùng: _context.Classes.Remove(classEntity);

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(0, "Xóa lớp học thành công", true);
        }

        public ClassDto MapToDto(Class entity)
        {
            return new ClassDto
            {
                Id = entity.Id,
                AcademicYearId = entity.AcademicYearId,
                DepartmentId = entity.DepartmentId,
                ClassTypeId = entity.ClassTypeId,
                Description = entity.Description,
                ClassCode = entity.ClassCode,
            };
        }

        public Task<ClassDto> UpdateClassAsync(CreateClassRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ClassDto> UpdateClassAsync(int id, CreateClassRequest request)
        {
            throw new NotImplementedException();
        }
    }
}