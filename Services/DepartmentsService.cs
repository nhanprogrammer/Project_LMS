using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
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

        public async Task<ApiResponse<PaginatedResponse<DepartmentResponse>>> GetAllCoursesAsync(
            int? pageNumber,
            int? pageSize,
            string? sortDirection // "asc" hoặc "desc"
        )
        {
            // 0. Kiểm tra dữ liệu đầu vào
            if (pageNumber.HasValue && pageNumber <= 0)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị pageNumber phải lớn hơn 0",
                    null
                );
            }

            if (pageSize.HasValue && pageSize <= 0)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị pageSize phải lớn hơn 0",
                    null
                );
            }

            // Nếu bạn muốn trả về lỗi khi sortDirection không đúng:
            // (nếu chỉ muốn mặc định 'asc' thì bạn có thể bỏ đoạn này)
            if (!string.IsNullOrEmpty(sortDirection) &&
                !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị sortDirection phải là 'asc' hoặc 'desc'",
                    null
                );
            }

            try
            {
                // 1. Xác định pageNumber, pageSize mặc định 
                var currentPage = pageNumber ?? 1;
                var currentPageSize = pageSize ?? 10;

                // 2. Lấy danh sách departments
                var departments = await _departmentRepository.GetAllAsync();
                var queryableDepartments = departments.AsQueryable();

                // 3. Nếu không nhập sortDirection, mặc định là "asc"
                if (string.IsNullOrEmpty(sortDirection))
                {
                    sortDirection = "asc";
                }

                // 4. Áp dụng sắp xếp dựa trên sortDirection
                //    Ở đây luôn sắp xếp theo cột Name, sau đó ThenByDescending DepartmentCode
                if (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    queryableDepartments = queryableDepartments
                        .OrderByDescending(d => d.Name)
                        .ThenByDescending(d => d.DepartmentCode);
                }
                else
                {
                    // Mặc định là asc
                    queryableDepartments = queryableDepartments
                        .OrderBy(d => d.Name)
                        .ThenByDescending(d => d.DepartmentCode);
                }

                // 5. Tính toán tổng số dòng, số trang
                var totalItems = queryableDepartments.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

                // 6. Phân trang
                var pagedDepartments = queryableDepartments
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                // 7. Map sang DTO
                var mappedData = _mapper.Map<List<DepartmentResponse>>(pagedDepartments);

                // 8. Tạo đối tượng phân trang
                var paginatedResponse = new PaginatedResponse<DepartmentResponse>
                {
                    Items = mappedData,
                    PageNumber = currentPage,
                    PageSize = currentPageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = currentPage > 1,
                    HasNextPage = currentPage < totalPages
                };

                // 9. Trả về
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    0,
                    "Lấy dữ liệu thành công",
                    paginatedResponse
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    $"Lỗi: {ex.Message}",
                    null
                );
            }
        }

        public async Task<ApiResponse<DepartmentResponse>> CreateDepartmentAsync(
            CreateDepartmentRequest createDepartmentRequest)
        {
            try
            {
                // Chuyển đổi dữ liệu từ DTO sang entity Department
                var department = _mapper.Map<Department>(createDepartmentRequest);

                // Cập nhật thời gian tạo và thông tin người tạo từ request
                department.CreateAt = TimeHelper.NowUsingTimeZone;
                department.UserCreate = createDepartmentRequest.userId;

                // Thêm phòng ban vào cơ sở dữ liệu thông qua repository
                await _departmentRepository.AddAsync(department);

                // Chuyển đổi đối tượng Department vừa được thêm thành DepartmentResponse
                var response = _mapper.Map<DepartmentResponse>(department);

                // Lấy thông tin User dựa trên UserId từ request để lấy tên của User
                var user = _context.Users.FirstOrDefault(x => x.Id == createDepartmentRequest.userId);
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

        public async Task<ApiResponse<DepartmentResponse>> UpdateDepartmentAsync(UpdateDepartmentRequest updateDepartmentRequest)
        {
            try
            {
                // 1. Lấy thông tin phòng ban từ cơ sở dữ liệu theo id được cung cấp
                var department = await _departmentRepository.GetByIdAsync(updateDepartmentRequest.id);
                if (department == null)
                {
                    // Nếu không tìm thấy, trả về thông báo lỗi
                    return new ApiResponse<DepartmentResponse>(1, "Department không tìm thấy", null);
                }

                // 2. Kiểm tra giá trị userUpdate (nếu cần)
                if (updateDepartmentRequest.userUpdate.HasValue && updateDepartmentRequest.userUpdate < 0)
                {
                    // Trả về lỗi do giá trị âm
                    return new ApiResponse<DepartmentResponse>(1, "UserUpdate không hợp lệ (phải >= 0)", null);
                }

                // 3. Sử dụng AutoMapper để cập nhật các thuộc tính của department từ updateDepartmentRequest
                _mapper.Map(updateDepartmentRequest, department);

                // 4. Cập nhật thời gian cập nhật (UpdateAt)
                //    Nếu updateDepartmentRequest.UpdateAt là null, sử dụng thời gian hiện tại (DateTime.Now)
                department.UpdateAt = DateTime.SpecifyKind(
                    updateDepartmentRequest.updateAt ?? DateTime.Now,
                    DateTimeKind.Unspecified);

                // 5. Lưu các thay đổi vào cơ sở dữ liệu thông qua repository
                await _departmentRepository.UpdateAsync(department);

                // 6. Chuyển đổi đối tượng Department đã cập nhật sang DepartmentResponse dùng AutoMapper
                var response = _mapper.Map<DepartmentResponse>(department);

                // 7. Lấy thông tin của User (người cập nhật) từ bảng Users dựa trên updateDepartmentRequest.UserUpdate
                //    và gán tên của User vào thuộc tính UserName trong response
                if (updateDepartmentRequest.userUpdate.HasValue && updateDepartmentRequest.userUpdate > 0)
                {
                    var updatingUser = _context.Users.FirstOrDefault(x => x.Id == updateDepartmentRequest.userUpdate);
                    response.UserName = updatingUser?.FullName;
                }
                else
                {
                    // Trường hợp UserUpdate = 0 hoặc null, có thể bỏ qua hoặc gán chuỗi trống
                    response.UserName = null;
                }

                // 8. Trả về phản hồi thành công kèm dữ liệu DepartmentResponse đã được cập nhật
                return new ApiResponse<DepartmentResponse>(0, "Department đã cập nhật thành công", response);
            }
            catch (Exception ex)
            {
                // 9. Trong trường hợp xảy ra lỗi, lấy thông báo lỗi chi tiết (ưu tiên InnerException nếu có)
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

        public async Task<ApiResponse<PaginatedResponse<DepartmentResponse>>> SearchDepartmentsAsync
        (
            string? keyword, int? pageNumber, int? pageSize, string? sortDirection
        )
        {
            // 0. Kiểm tra dữ liệu đầu vào
            if (pageNumber.HasValue && pageNumber <= 0)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị pageNumber phải lớn hơn 0",
                    null
                );
            }

            if (pageSize.HasValue && pageSize <= 0)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị pageSize phải lớn hơn 0",
                    null
                );
            }

            if (!string.IsNullOrEmpty(sortDirection) &&
                !sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    "Giá trị sortDirection phải là 'asc' hoặc 'desc'",
                    null
                );
            }

            try
            {
                // 1. Xác định pageNumber, pageSize mặc định 
                var currentPage = pageNumber ?? 1;
                var currentPageSize = pageSize ?? 10;

                // 2. Lấy danh sách departments từ repository
                var departments = await _departmentRepository.GetAllAsync();
                var queryableDepartments = departments.AsQueryable();

                // 3. Nếu có keyword, lọc danh sách departments dựa trên keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    queryableDepartments = queryableDepartments
                        .Where(d => d.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                    d.DepartmentCode.ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase));
                }

                // 4. Nếu không nhập sortDirection, mặc định là "asc"
                if (string.IsNullOrEmpty(sortDirection))
                {
                    sortDirection = "asc";
                }

                // 5. Áp dụng sắp xếp dựa trên sortDirection
                if (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    queryableDepartments = queryableDepartments
                        .OrderByDescending(d => d.Name)
                        .ThenByDescending(d => d.DepartmentCode);
                }
                else
                {
                    queryableDepartments = queryableDepartments
                        .OrderBy(d => d.Name)
                        .ThenByDescending(d => d.DepartmentCode);
                }

                // 6. Tính toán tổng số dòng, số trang
                var totalItems = queryableDepartments.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / currentPageSize);

                // 7. Phân trang
                var pagedDepartments = queryableDepartments
                    .Skip((currentPage - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                // 8. Map sang DTO
                var mappedData = _mapper.Map<List<DepartmentResponse>>(pagedDepartments);

                // 9. Tạo đối tượng phân trang
                var paginatedResponse = new PaginatedResponse<DepartmentResponse>
                {
                    Items = mappedData,
                    PageNumber = currentPage,
                    PageSize = currentPageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasPreviousPage = currentPage > 1,
                    HasNextPage = currentPage < totalPages
                };

                // 10. Trả về
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    0,
                    "Tìm kiếm thành công",
                    paginatedResponse
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    1,
                    $"Lỗi: {ex.Message}",
                    null
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<object>>> GetAllClassesAsync(int departmentId)
        {
            // Kiểm tra id hợp lệ
            if (departmentId <= 0)
            {
                return new ApiResponse<IEnumerable<object>>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            // Kiểm tra departmentId có tồn tại hay không
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);
            if (!departmentExists)
            {
                return new ApiResponse<IEnumerable<object>>(1, "Department không tồn tại.", null);
            }

            var result = await _context.Classes
                .Where(c => c.IsDelete == false && c.DepartmentId == departmentId)
                .Select(c => new
                {
                    ClassId = c.Id,
                    DepartmentName = c.Department.Name,
                    ClassName = c.Name,
                })
                .ToListAsync();

            return new ApiResponse<IEnumerable<object>>(0, "Lấy thông tin lớp học thành công!", result);
        }

        public async Task<ApiResponse<string>> DeleteClassById(List<int> classIds)
        {
            try
            {
                if (classIds == null || classIds.Count == 0)
                {
                    return new ApiResponse<string>(1, "Danh sách ID không hợp lệ! Vui lòng nhập danh sách số nguyên.");
                }

                if (classIds.Any(id => id <= 0))
                {
                    return new ApiResponse<string>(1, "Danh sách ID không hợp lệ! Chỉ chấp nhận số nguyên dương.");
                }

                var classesToDelete = await _context.Classes
                    .Where(c => classIds.Contains(c.Id) && c.IsDelete == false)
                    .ToListAsync();

                if (!classesToDelete.Any())
                {
                    return new ApiResponse<string>(1, "Không có lớp hợp lệ để xóa!");
                }

                // Soft-delete
                classesToDelete.ForEach(c => c.IsDelete = true);

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(0, "Xóa lớp thành công!");
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                return new ApiResponse<string>(1, $"Lỗi khi xóa lớp: {innerExceptionMessage}");
            }
        }
    }
}