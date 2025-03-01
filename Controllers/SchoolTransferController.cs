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
    public class SchoolTransferController : ControllerBase
    {
        private readonly ISchoolTransferService _schoolTransferService;

        public SchoolTransferController(ISchoolTransferService schoolTransferService)
        {
            _schoolTransferService = schoolTransferService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SchoolTransferResponse>>>> GetAll()
        {
            try
            {
                var schoolTransfers = await _schoolTransferService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<SchoolTransferResponse>>(1, "Lấy danh sách chuyển trường thành công", schoolTransfers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách chuyển trường", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolTransferResponse>>> GetById(int id)
        {
            try
            {
                var schoolTransfer = await _schoolTransferService.GetByIdAsync(id);
                if (schoolTransfer == null)
                {
                    return NotFound(new ApiResponse<SchoolTransferResponse>(0, "Không tìm thấy chuyển trường"));
                }
                return Ok(new ApiResponse<SchoolTransferResponse>(1, "Lấy chuyển trường thành công", schoolTransfer));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy chuyển trường", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SchoolTransferResponse>>> Create(SchoolTransferRequest schoolTransferRequest)
        {
            try
            {
                var jsonString = schoolTransferRequest?.ToString();
                if (string.IsNullOrEmpty(jsonString) || !JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(0, "Invalid JSON format", null));
                }

                if (schoolTransferRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
                }
                var schoolTransfer = await _schoolTransferService.CreateAsync(schoolTransferRequest);
                return CreatedAtAction(nameof(GetById), new { id = schoolTransfer.Id }, new ApiResponse<SchoolTransferResponse>(1, "Tạo chuyển trường thành công", schoolTransfer));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo chuyển trường", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolTransferResponse>>> Update(int id, SchoolTransferRequest schoolTransferRequest)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(schoolTransferRequest);

                if (!JsonValidator.IsValidJson(jsonString))
                {
                    return BadRequest(new ApiResponse<string>(0, "Định dạng JSON không hợp lệ", null));
                }

                if (schoolTransferRequest == null)
                {
                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
                }
                var schoolTransfer = await _schoolTransferService.UpdateAsync(id, schoolTransferRequest);
                if (schoolTransfer == null)
                {
                    return NotFound(new ApiResponse<SchoolTransferResponse>(0, "Không tìm thấy chuyển trường"));
                }
                return Ok(new ApiResponse<SchoolTransferResponse>(1, "Cập nhật chuyển trường thành công", schoolTransfer));
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
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật chuyển trường", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<SchoolTransferResponse>>> Delete(int id)
        {
            try
            {
                var schoolTransfer = await _schoolTransferService.DeleteAsync(id);
                if (schoolTransfer == null)
                {
                    return NotFound(new ApiResponse<SchoolTransferResponse>(0, "Không tìm thấy chuyển trường"));
                }
                return Ok(new ApiResponse<SchoolTransferResponse>(1, "Xóa chuyển trường thành công", schoolTransfer));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa chuyển trường", ex.Message));
            }
        }
    }
}