using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using System.Text.Json;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolBranchController : ControllerBase
    {
        private readonly ISchoolBranchService _schoolBranchService;

        public SchoolBranchController(ISchoolBranchService schoolBranchService)
        {
            _schoolBranchService = schoolBranchService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SchoolBranchResponse>>>> GetAll()
        {
            try
            {

                var schoolBranches = await _schoolBranchService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<SchoolBranchResponse>>(0, "Lấy danh sách chi nhánh trường thành công", schoolBranches));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách chi nhánh trường", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolBranchResponse>>> GetById(int id)
        {
            try
            {
                var schoolBranch = await _schoolBranchService.GetByIdAsync(id);
                if (schoolBranch == null)
                {
                    return NotFound(new ApiResponse<SchoolBranchResponse>(1, "Không tìm thấy chi nhánh trường"));
                }
                return Ok(new ApiResponse<SchoolBranchResponse>(0, "Lấy chi nhánh trường thành công", schoolBranch));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy chi nhánh trường", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SchoolBranchResponse>>> Create(SchoolBranchRequest schoolBranchRequest)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(schoolBranchRequest);

                if (!JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(1, "Định dạng JSON không hợp lệ", null));
                }

                if (schoolBranchRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(1, "Request body cannot be null", null));
                }

                var schoolBranch = await _schoolBranchService.CreateAsync(schoolBranchRequest);
                return CreatedAtAction(nameof(GetById), new { id = schoolBranch.Id }, new ApiResponse<SchoolBranchResponse>(0, "Tạo chi nhánh trường thành công", schoolBranch));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi tạo chi nhánh trường", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolBranchResponse>>> Update(int id, SchoolBranchRequest schoolBranchRequest)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(schoolBranchRequest);

                if (!JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(1, "Định dạng JSON không hợp lệ", null));
                }

                if (schoolBranchRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(1, "Request body cannot be null", null));
                }
                var schoolBranch = await _schoolBranchService.UpdateAsync(id, schoolBranchRequest);
                if (schoolBranch == null)
                {
                    return NotFound(new ApiResponse<SchoolBranchResponse>(1, "Không tìm thấy chi nhánh trường"));
                }
                return Ok(new ApiResponse<SchoolBranchResponse>(0, "Cập nhật chi nhánh trường thành công", schoolBranch));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi cập nhật chi nhánh trường", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolBranchResponse>>> Delete(int id)
        {
            try
            {
                var schoolBranch = await _schoolBranchService.DeleteAsync(id);
                if (schoolBranch == null)
                {
                    return NotFound(new ApiResponse<SchoolBranchResponse>(1, "Không tìm thấy chi nhánh trường"));
                }
                return Ok(new ApiResponse<SchoolBranchResponse>(0, "Xóa chi nhánh trường thành công", schoolBranch));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi xóa chi nhánh trường", ex.Message));
            }
        }
    }
}