using AutoMapper;
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
        private readonly IMapper _mapper;

        public DepartmentsService(IDepartmentRepository departmentRepository, ApplicationDbContext context,
            IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<DepartmentResponse>>> GetAllCoursesAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            var data = _mapper.Map<List<DepartmentResponse>>(departments);
            return new ApiResponse<List<DepartmentResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<DepartmentResponse>> CreateDepartmentAsync(
            CreateDepartmentRequest createDepartmentRequest)
        {
            try
            {
                // Chuyển đổi dữ liệu từ DTO sang entity Department
                var department = _mapper.Map<Department>(createDepartmentRequest);

                // Cập nhật thời gian tạo và thông tin người tạo từ request
                department.CreateAt = DateTime.Now;
                department.UserCreate = createDepartmentRequest.UserId;

                // Thêm phòng ban vào cơ sở dữ liệu thông qua repository
                await _departmentRepository.AddAsync(department);

                // Chuyển đổi đối tượng Department vừa được thêm thành DepartmentResponse
                var response = _mapper.Map<DepartmentResponse>(department);

                // Lấy thông tin User dựa trên UserId từ request để lấy tên của User
                var user = _context.Users.FirstOrDefault(x => x.Id == createDepartmentRequest.UserId);
                response.UserName = user?.FullName;

                // Gán DepartmentID từ entity vào response
                response.DepartmentID = department.Id;

                // Trả về phản hồi thành công với mã status 0 và dữ liệu DepartmentResponse
                return new ApiResponse<DepartmentResponse>(0, "Department đã thêm thành công", response);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, lấy thông tin chi tiết lỗi (nếu có InnerException thì ưu tiên sử dụng message của nó)
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                // Trả về phản hồi lỗi với status 1 và thông báo lỗi
                return new ApiResponse<DepartmentResponse>(1, $"Lỗi khi thêm department: {innerExceptionMessage}",
                    null);
            }
        }

        public async Task<ApiResponse<DepartmentResponse>> UpdateDepartmentAsync(
            int id, UpdateDepartmentRequest updateDepartmentRequest)
        {
            try
            {
                // 1. Lấy thông tin phòng ban từ cơ sở dữ liệu theo id được cung cấp
                var department = await _departmentRepository.GetByIdAsync(id);
                if (department == null)
                {
                    // Nếu không tìm thấy, trả về thông báo lỗi
                    return new ApiResponse<DepartmentResponse>(1, "Department không tìm thấy", null);
                }

                // 2. Sử dụng AutoMapper để cập nhật các thuộc tính của department từ updateDepartmentRequest
                _mapper.Map(updateDepartmentRequest, department);

                // 3. Cập nhật thời gian cập nhật (UpdateAt)
                //    Nếu updateDepartmentRequest.UpdateAt là null, sử dụng thời gian hiện tại (DateTime.Now)
                //    DateTime.SpecifyKind giúp đặt DateTimeKind cho giá trị UpdateAt (ở đây là Unspecified)
                department.UpdateAt = DateTime.SpecifyKind(
                    updateDepartmentRequest.UpdateAt ?? DateTime.Now,
                    DateTimeKind.Unspecified);

                // 4. Lưu các thay đổi vào cơ sở dữ liệu thông qua repository
                await _departmentRepository.UpdateAsync(department);

                // 5. Chuyển đổi đối tượng Department đã cập nhật sang DepartmentResponse dùng AutoMapper
                var response = _mapper.Map<DepartmentResponse>(department);

                // 6. Lấy thông tin của User (người cập nhật) từ bảng Users dựa trên updateDepartmentRequest.UserUpdate
                //    và gán tên của User vào thuộc tính UserName trong response
                var updatingUser = _context.Users.FirstOrDefault(x => x.Id == updateDepartmentRequest.UserUpdate);
                response.UserName = updatingUser?.FullName;

                // 7. Trả về phản hồi thành công kèm dữ liệu DepartmentResponse đã được cập nhật
                return new ApiResponse<DepartmentResponse>(0, "Department đã cập nhật thành công", response);
            }
            catch (Exception ex)
            {
                // 8. Trong trường hợp xảy ra lỗi, lấy thông báo lỗi chi tiết (ưu tiên InnerException nếu có)
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                // Trả về phản hồi lỗi với mã lỗi 1 và thông báo lỗi
                return new ApiResponse<DepartmentResponse>(1, $"Lỗi khi cập nhật department: {innerExceptionMessage}",
                    null);
            }
        }
        public async Task<ApiResponse<DepartmentResponse>> DeleteDepartmentAsync(string id)
        {
            // 1. Kiểm tra định dạng ID (chuỗi) có hợp lệ hay không, nếu không hợp lệ trả về lỗi.
            if (!int.TryParse(id, out int departmentId))
            {
                return new ApiResponse<DepartmentResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            // 2. Lấy đối tượng Department từ repository dựa trên departmentId
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                return new ApiResponse<DepartmentResponse>(1, "Department không tìm thấy", null);
            }

            try
            {
                // 3. Đánh dấu phòng ban là đã xóa (soft delete)
                department.IsDelete = true;
        
                // 4. Lưu thay đổi vào cơ sở dữ liệu thông qua repository
                await _departmentRepository.UpdateAsync(department);

                // 5. Trả về phản hồi thành công
                return new ApiResponse<DepartmentResponse>(0, "Department đã xóa thành công", null);
            }
            catch (Exception ex)
            {
                // 6. Nếu có lỗi xảy ra, bắt exception và trả về thông báo lỗi chi tiết
                return new ApiResponse<DepartmentResponse>(1, $"Lỗi khi xóa department: {ex.Message}", null);
            }
        }

    }
}