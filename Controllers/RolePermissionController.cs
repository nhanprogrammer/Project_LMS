using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RolePermissionResponse>>>> GetAll()
        {
            try
            {
                var rolePermissions = await _rolePermissionService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<RolePermissionResponse>>(1, "Lấy danh sách quyền và phân quyền thành công", rolePermissions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách quyền và phân quyền", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RolePermissionResponse>>> GetById(int id)
        {
            try
            {
                var rolePermission = await _rolePermissionService.GetByIdAsync(id);
                if (rolePermission == null)
                {
                    return NotFound(new ApiResponse<RolePermissionResponse>(0, "Không tìm thấy quyền và phân quyền"));
                }
                return Ok(new ApiResponse<RolePermissionResponse>(1, "Lấy quyền và phân quyền thành công", rolePermission));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy quyền và phân quyền", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RolePermissionResponse>>> Create(RolePermissionRequest request)
        {
            try
            {
                var rolePermission = await _rolePermissionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = rolePermission.Id }, new ApiResponse<RolePermissionResponse>(1, "Tạo quyền và phân quyền thành công", rolePermission));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo quyền và phân quyền", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RolePermissionResponse>>> Update(int id, RolePermissionRequest request)
        {
            try
            {
                var rolePermission = await _rolePermissionService.UpdateAsync(id, request);
                if (rolePermission == null)
                {
                    return NotFound(new ApiResponse<RolePermissionResponse>(0, "Không tìm thấy quyền và phân quyền"));
                }
                return Ok(new ApiResponse<RolePermissionResponse>(1, "Cập nhật quyền và phân quyền thành công", rolePermission));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật quyền và phân quyền", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RolePermissionResponse>>> Delete(int id)
        {
            try
            {
                var rolePermission = await _rolePermissionService.DeleteAsync(id);
                if (rolePermission == null)
                {
                    return NotFound(new ApiResponse<RolePermissionResponse>(0, "Không tìm thấy quyền và phân quyền"));
                }
                return Ok(new ApiResponse<RolePermissionResponse>(1, "Xóa quyền và phân quyền thành công", rolePermission));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa quyền và phân quyền", ex.Message));
            }
        }
    }
}