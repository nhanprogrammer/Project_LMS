
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
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
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroupPermission([FromBody] SaveGroupPermissionRequest request)
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

                return Ok(new ApiResponse<string>(0, "Tạo nhóm quyền thành công.", null));
            }
            catch (NotFoundException ex)
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
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new ApiResponse<string>(1, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng thử lại.", null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroupPermission([FromBody] SaveGroupPermissionRequest request)
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

                return Ok(new ApiResponse<string>(0, "Cập nhật nhóm quyền thành công.", null));
            }
            catch (NotFoundException ex)
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
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new ApiResponse<string>(1, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng thử lại.", null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetGroupPermission([FromQuery] PermissionIdRequest groupRoleId)
        {
            try
            {
                var result = await _permissionService.GetGroupPermissionById(groupRoleId.Id);
                return Ok(new ApiResponse<GroupPermissionResponse>(0, "Lấy thông tin nhóm quyền thành công.", result));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau."+ex.Message, null));
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGroupPermission([FromBody] PermissionIdRequest groupRoleId)
        {
            try
            {
                bool result = await _permissionService.DeleteGroupPermission(groupRoleId.Id);
                return Ok(new ApiResponse<string>(0, "Xóa nhóm quyền thành công.", null));
            }
            catch (NotFoundException ex)
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
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetPermissionUser([FromQuery] PermissionIdRequest request)
        {
            try
            {
                var response = await _permissionService.GetUserPermission(request.Id);

                return Ok(new ApiResponse<PermissionUserRequest>(0, "Lấy danh sách người dùng thành công.", response));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
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
                    return BadRequest(new ApiResponse<string>(1, "Không thể cập nhật quyền người dùng.", null));
                }
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }
        [HttpDelete("user-delete")]
        public async Task<IActionResult> DeleteUserPermission([FromBody] PermissionIdRequest userId)
        {
            try
            {
                bool success = await _permissionService.DeleteUser(userId.Id);

                if (success)
                {
                    return Ok(new ApiResponse<string>(0, "Xóa người dùng thành công.", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<string>(1, "Không thể xóa người dùng.", null));
                }
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại sau.", null));
            }
        }


        //Demo
        [HttpGet("u")]
        public async Task<IActionResult> GetUserPer([FromQuery] PermissionIdRequest userId)
        {
            var permissions = await _permissionService.ListPermission(userId.Id);

            if (permissions == null || permissions.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy quyền cho người dùng này." });
            }

            return Ok(new { userId, permissions });
        }
    }
}