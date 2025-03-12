
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        [HttpGet("group-list")]
        public async Task<IActionResult> GetPermissionListGroup([FromQuery] PermissionListRequest request)
        {
            try
            {
                var response = await _permissionService.GetPermissionListGroup(request.Key, request.PageNumber, request.PageSize);

                return Ok(new ApiResponse<PaginatedResponse<PermissionListGroupResponse>>(0, "Lấy danh sách nhóm quyền thành công.", response));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(3, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpPost("save")]
        public async Task<IActionResult> UpdateGroupPermissions([FromBody] SaveGroupPermissionRequest request)
        {
            try
            {
                bool result = await _permissionService.SaveGroupPermission(
                    request.GroupRoleId,
                    request.GroupRoleName,
                    request.Description,
                    request.AllPermission,
                    request.Permissions
                );

                return Ok(new ApiResponse<bool>(0, "Lưu quyền nhóm thành công.", result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new ApiResponse<string>(1, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng thử lại.", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetGroupPermission([FromQuery] int groupRoleId)
        {
            try
            {
                var result = await _permissionService.GetGroupPermissionById(groupRoleId);
                return Ok(new ApiResponse<GroupPermissionResponse>(0, "Lấy thông tin nhóm quyền thành công.", result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGroupPermission([FromQuery] int groupRoleId)
        {
            try
            {
                bool result = await _permissionService.DeleteGroupPermission(groupRoleId);
                return Ok(new ApiResponse<bool>(0, "Xóa nhóm quyền thành công.", result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new ApiResponse<string>(1, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng thử lại.", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpGet("user-list")]
        public async Task<IActionResult> GetPermissionUserList([FromQuery] PermissionListRequest request)
        {
            try
            {
                var response = await _permissionService.GetPermissionUserList(request.Key, request.PageNumber, request.PageSize);

                return Ok(new ApiResponse<PaginatedResponse<PermissionUserResponse>>(0, "Lấy danh sách người dùng thành công.", response));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(3, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetPermissionUser([FromQuery] PermissionUserRequest request)
        {
            try
            {
                var response = await _permissionService.GetUserPermission(request.UserId, request.GroupId, request.Disable);

                return Ok(new ApiResponse<PermissionUserRequest>(0, "Lấy danh sách người dùng thành công.", response));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(3, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }



        [HttpPost("user-save")]
        public async Task<IActionResult> SaveUserPermission([FromBody] PermissionUserRequest request)
        {
            try
            {
                bool success = await _permissionService.SaveUserPermission(request.UserId, request.GroupId, request.Disable);

                if (success)
                {
                    return Ok(new ApiResponse<PermissionUserRequest>(0, "Cập nhật quyền người dùng thành công.", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<string>(2, "Không thể cập nhật quyền người dùng.", null));
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(3, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpDelete("user-delete")]
        public async Task<IActionResult> DeleteUserPermission([FromQuery] int userId)
        {
            try
            {
                bool success = await _permissionService.DeleteUser(userId);

                if (success)
                {
                    return Ok(new ApiResponse<string>(0, "Xóa người dùng thành công.", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<string>(2, "Không thể xóa người dùng.", null));
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(3, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }

    }
}