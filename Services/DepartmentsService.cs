using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ApplicationDbContext _context;

        public DepartmentsService(IDepartmentRepository departmentRepository, ApplicationDbContext context)
        {
            _departmentRepository = departmentRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<DepartmentResponse>>> GetAllCoursesAsync()
        {
            var department = await _departmentRepository.GetAllAsync();
    
            var data = department.Select(c => new DepartmentResponse
            {
                DepartmentCode = c.Id,
                DepartmentName = c.Name
            }).ToList();
    
            return new ApiResponse<List<DepartmentResponse>>(0, "Fill dữ liệu thành ", data);
        }


        public async Task<ApiResponse<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest createDepartmentRequest)
        {
            var department = new Department
            {
                Name = createDepartmentRequest.Name,
                CreateAt = DateTime.Now,
            };

            
            await _departmentRepository.AddAsync(department);

            var response = new DepartmentResponse
            {  
                DepartmentCode = department.Id,
               DepartmentName = department.Name,
             
            };

            return new ApiResponse<DepartmentResponse>(0, "Department đã thêm thành công", response);
        }

        public async Task<ApiResponse<DepartmentResponse>> UpdateDepartmentAsync(string id, UpdateDepartmentRequest updateDepartmentRequest)
        {
            if (!int.TryParse(id, out int departmentId))
            {
                return new ApiResponse<DepartmentResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                return new ApiResponse<DepartmentResponse>(1, "Không tìm thấy department.", null);
            }

              department.Name = updateDepartmentRequest.Name;
             department.UpdateAt = DateTime.Now;
    
            await _departmentRepository.UpdateAsync(department);
            var response = new DepartmentResponse
            {  
                DepartmentCode = department.Id,
                DepartmentName = department.Name,
             
            };

            return new ApiResponse<DepartmentResponse>(0, "Department đã cập nhật thành công", response);
        }

        public async Task<ApiResponse<DepartmentResponse>> DeleteDepartmentAsync(string id)
        {
            if (!int.TryParse(id, out int departmentId))
            {
                return new ApiResponse<DepartmentResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                return new ApiResponse<DepartmentResponse>(1, "Department không tìm thấy");
            }

            department.IsDelete = true;
            await _departmentRepository.UpdateAsync(department);

            return new ApiResponse<DepartmentResponse>(0, "Department đã xóa thành công ");
        }
    }
}