using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleResponse>>>> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<RoleResponse>>(1, "Lấy danh sách quyền thành công", roles));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách quyền", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleResponse>>> GetById(int id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new ApiResponse<RoleResponse>(0, "Không tìm thấy quyền"));
                }
                return Ok(new ApiResponse<RoleResponse>(1, "Lấy quyền thành công", role));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy quyền", ex.Message));
            }
        }

       
    }
}