using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Filters;
using Project_LMS.Helpers;
using System.Text.Json;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ValidationFilter))]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SchoolResponse>>>> GetAll()
        {
            try
            {
                var schools = await _schoolService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<SchoolResponse>>(1, "Lấy danh sách trường thành công", schools));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách trường", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolResponse>>> GetById(int id)
        {
            try
            {
                var school = await _schoolService.GetByIdAsync(id);
                if (school == null)
                {
                    return NotFound(new ApiResponse<SchoolResponse>(0, "Không tìm thấy trường"));
                }
                return Ok(new ApiResponse<SchoolResponse>(1, "Lấy trường thành công", school));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy trường", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SchoolResponse>>> Create([FromBody] SchoolRequest schoolRequest)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(schoolRequest);

                if (!JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(0, "Định dạng JSON không hợp lệ", null));
                }
                if (schoolRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
                }

                var school = await _schoolService.CreateAsync(schoolRequest);
                return CreatedAtAction(nameof(GetById), new { id = school.Id }, new ApiResponse<SchoolResponse>(1, "Tạo trường thành công", school));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo trường", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolResponse>>> Update(int id, [FromBody] SchoolRequest schoolRequest)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(schoolRequest);

                if (!JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(0, "Định dạng JSON không hợp lệ", null));
                }

                if (schoolRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
                }
                var school = await _schoolService.UpdateAsync(id, schoolRequest);
                if (school == null)
                {
                    return NotFound(new ApiResponse<SchoolResponse>(0, "Không tìm thấy trường"));
                }
                return Ok(new ApiResponse<SchoolResponse>(1, "Cập nhật trường thành công", school));
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
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật trường", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolResponse>>> Delete(int id)
        {
            try
            {
                var school = await _schoolService.DeleteAsync(id);
                if (school == null)
                {
                    return NotFound(new ApiResponse<SchoolResponse>(0, "Không tìm thấy trường"));
                }
                return Ok(new ApiResponse<SchoolResponse>(1, "Xóa trường thành công", school));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa trường", ex.Message));
            }
        }
    }
}