using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Filters;
using Project_LMS.Helpers;
using System.Text.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ValidationFilter))]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly IExcelService _excelService;
        private readonly ICloudinaryService _cloudinaryService;

        public SchoolController(ISchoolService schoolService, IExcelService excelService, ICloudinaryService cloudinaryService)
        {
            _schoolService = schoolService;
            _excelService = excelService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SchoolResponse>>>> GetAll()
        {
            try
            {
                var schools = await _schoolService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<SchoolResponse>>(1, "Lấy thông tin trường thành công", schools));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách trường", ex.Message));
            }
        }

        [HttpGet("education-models")]
        public async Task<ActionResult<ApiResponse<string[]>>> GetEducationModels()
        {
            try
            {
                var educationModelsResponse = await _schoolService.GetEducationModelsAsync();
                var educationModels = educationModelsResponse.Data;
                return Ok(new ApiResponse<string[]>(0, "Lấy danh sách mô hình đào tạo thành công", educationModels));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách mô hình đào tạo", ex.Message));
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

        [HttpPost("export-excel")]
        public async Task<ActionResult<ApiResponse<string>>> ExportExcel([FromBody] ExportSchoolExcelRequest request)
        {
            try
            {

                if (request == null || request.SchoolBranchIds == null || !request.SchoolBranchIds.Any())
                {
                    return BadRequest(new ApiResponse<string>(0, "SchoolId hoặc danh sách SchoolBranchIds không được để trống", null));
                }


                var school = await _schoolService.GetSchoolAndBranchesAsync(request.SchoolId, request.SchoolBranchIds);

                if (school == null)
                {
                    return NotFound(new ApiResponse<string>(0, "Không tìm thấy trường hoặc chi nhánh để xuất Excel", null));
                }

                var base64String = await _excelService.ExportSchoolAndBranchesToExcelAsync(school, request.SchoolId);

                return Ok(new ApiResponse<string>(1, "Xuất Excel thành công", base64String));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xuất Excel", ex.Message));
            }
        }

        [HttpPost("upload-excel")]
        public async Task<ActionResult<ApiResponse<string>>> UploadExcel([FromBody] UploadFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Base64String))
                {
                    return BadRequest(new ApiResponse<string>(0, "Base64 string không được để trống", null));
                }

                var fileUrl = await _cloudinaryService.UploadExcelAsync(request.Base64String);

                return Ok(new ApiResponse<string>(1, "Tải lên Excel thành công", fileUrl));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tải lên Excel", ex.Message));
            }
        }

        [HttpPost("upload-doc")]
        public async Task<ActionResult<ApiResponse<string>>> UploadDoc([FromBody] UploadFileRequest request)
        {
            return await UploadFile(request.Base64String, _cloudinaryService.UploadDocAsync);
        }

        [HttpPost("upload-powerpoint")]
        public async Task<ActionResult<ApiResponse<string>>> UploadPowerPoint([FromBody] UploadFileRequest request)
        {
            return await UploadFile(request.Base64String, _cloudinaryService.UploadPowerPointAsync);
        }

        private async Task<ActionResult<ApiResponse<string>>> UploadFile(string base64String, Func<string, Task<string>> uploadFunction)
        {
            try
            {
                if (string.IsNullOrEmpty(base64String))
                {
                    return BadRequest(new ApiResponse<string>(0, "Base64 string không được để trống", null));
                }

                var fileUrl = await uploadFunction(base64String);

                return Ok(new ApiResponse<string>(1, "Tải lên thành công", fileUrl));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tải lên", ex.Message));
            }
        }
        public class UploadFileRequest
        {
            [Required(ErrorMessage = "Base64 string là bắt buộc")]
            public string Base64String { get; set; } = null!;
        }
    }
}