
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("list")]
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
    }
}