using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class ClassTypeService : IClassTypeService
    {
        private readonly IClassTypeRepository _classTypeRepository;

        public ClassTypeService(IClassTypeRepository classTypeRepository)
        {
            _classTypeRepository = classTypeRepository;
        }

        // Lấy tất cả ClassType
        public async Task<ApiResponse<IEnumerable<ClassType>>> GetAllAsync()
        {
            try
            {
                var classTypes = await _classTypeRepository.GetAllAsync();
                return new ApiResponse<IEnumerable<ClassType>>(0, "Lấy tất cả các loại lớp thành công.", classTypes);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ClassType>>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        // Lấy ClassType theo ID
        public async Task<ApiResponse<ClassType>> GetByIdAsync(int id)
        {
            try
            {
                var classType = await _classTypeRepository.GetByIdAsync(id);
                if (classType == null)
                {
                    return new ApiResponse<ClassType>(1, "Không tìm thấy loại lớp học.", null);
                }
                return new ApiResponse<ClassType>(0, "Lấy loại lớp học thành công.", classType);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClassType>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        // Thêm ClassType mới
        public async Task<ApiResponse<object>> AddAsync(ClassType classType)
        {
            try
            {
                await _classTypeRepository.AddAsync(classType);
                return new ApiResponse<object>(0, "Loại lớp học được tạo thành công.", classType);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<object>> UpdateAsync(ClassType classType)
        {
            try
            {
                var existingClassType = await _classTypeRepository.GetByIdAsync(classType.Id);
                if (existingClassType == null)
                {
                    return new ApiResponse<object>(1, "Không tìm thấy loại lớp học.", null);
                }

                // Cập nhật thông tin
                existingClassType.Name = classType.Name;
                existingClassType.UpdateAt = DateTime.UtcNow;
                existingClassType.UserUpdate = classType.UserUpdate;

                // Cập nhật vào cơ sở dữ liệu
                await _classTypeRepository.UpdateAsync(existingClassType);

                // Trả về thông báo thành công
                return new ApiResponse<object>(0, "Loại lớp học đã được cập nhật thành công.", existingClassType);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }


        // Xóa ClassType theo ID
        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            try
            {
                var classType = await _classTypeRepository.GetByIdAsync(id);
                if (classType == null)
                {
                    return new ApiResponse<object>(1, "Không tìm thấy loại lớp học.", null); // Trả về lỗi nếu không tìm thấy
                }

                await _classTypeRepository.DeleteAsync(id);
                return new ApiResponse<object>(0, "Loại lớp học đã được xóa thành công.", null); // Trả về thông báo thành công khi xóa
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, $"Có lỗi xảy ra: {ex.Message}", null); // Trả về lỗi nếu có
            }
        }
    }
}
