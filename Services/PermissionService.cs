using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<PermissionListGroupResponse>> GetPermissionListGroup(string key, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Mặc định 10 bản ghi mỗi trang

            // Tạo truy vấn cơ bản
            var query = _context.GroupModulePermissons
                .Where(p => p.IsDelete == false &&
                    (string.IsNullOrEmpty(key) ||
                     p.Name.Contains(key) ||
                     p.Description.Contains(key)));

            // Lấy tổng số bản ghi sau khi lọc
            int totalItems = await query.CountAsync();

            if (totalItems == 0)
            {
                throw new KeyNotFoundException("Không tìm thấy nhóm quyền nào.");
            }

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy danh sách bản ghi theo trang
            var permissions = await query
                .OrderBy(p => p.Id) // Có thể thay đổi theo thứ tự bạn muốn
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermissionListGroupResponse
                {
                    Id = p.Id,
                    Name = p.Name ?? "NaN",
                    MemberCount = _context.Users.Count(u => u.GroupRolePermission == p.Id).ToString(),
                    Description = p.Description ?? "NaN"
                })
                .ToListAsync();

            // Trả về dữ liệu dạng phân trang
            return new PaginatedResponse<PermissionListGroupResponse>
            {
                Items = permissions, // Trả về danh sách chứ không phải một phần tử đơn lẻ
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }



    }
}