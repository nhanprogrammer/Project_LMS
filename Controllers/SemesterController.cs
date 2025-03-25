//using Microsoft.AspNetCore.Mvc;
//using Project_LMS.Interfaces.Services;
//using Project_LMS.DTOs.Request;
//using Project_LMS.DTOs.Response;
//using Project_LMS.Exceptions;
//using Project_LMS.Helpers;

//namespace Project_LMS.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class SemesterController : ControllerBase
//    {
//        private readonly ISemesterService _semesterService;

//        public SemesterController(ISemesterService semesterService)
//        {
//            _semesterService = semesterService;
//        }

//        [HttpGet]
//        public async Task<ActionResult<ApiResponse<IEnumerable<SemesterResponse>>>> GetAll()
//        {
//            try
//            {
//                var semesters = await _semesterService.GetAllAsync();
//                return Ok(new ApiResponse<IEnumerable<SemesterResponse>>(1, "Lấy danh sách học kỳ thành công", semesters));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách học kỳ", ex.Message));
//            }
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetById(int id)
//        {
//            try
//            {
//                var semester = await _semesterService.GetByIdAsync(id);
//                if (semester == null)
//                {
//                    return NotFound(new ApiResponse<SemesterResponse>(0, "Không tìm thấy học kỳ"));
//                }
//                return Ok(new ApiResponse<SemesterResponse>(1, "Lấy học kỳ thành công", semester));
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new ApiResponse<string>(0, ex.Message, null));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy học kỳ", ex.Message));
//            }
//        }

//        [HttpPost]
//        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Create(CreateSemesterRequest semesterRequest)
//        {
//            try
//            {
//                var jsonString = semesterRequest?.ToString();
//                if (string.IsNullOrEmpty(jsonString) || !JsonValidator.IsValidJson(jsonString))
//                {
//                    return BadRequest(new ApiResponse<string>(0, "Invalid JSON format", null));
//                }

//                if (semesterRequest == null)
//                {
//                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
//                }
//                var semester = await _semesterService.CreateAsync(semesterRequest);
//                return CreatedAtAction(nameof(GetById), new { id = semester.Id }, new ApiResponse<SemesterResponse>(1, "Tạo học kỳ thành công", semester));
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo học kỳ", ex.Message));
//            }
//        }

//        [HttpPut("{id}")]
//        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Update(int id, CreateSemesterRequest semesterRequest)
//        {
//            try
//            {
//                var jsonString = semesterRequest?.ToString();
//                if (string.IsNullOrEmpty(jsonString) || !JsonValidator.IsValidJson(jsonString))
//                {
//                    return BadRequest(new ApiResponse<string>(0, "Invalid JSON format", null));
//                }

//                if (semesterRequest == null)
//                {
//                    return BadRequest(new ApiResponse<string>(0, "Request body cannot be null", null));
//                }
//                var semester = await _semesterService.UpdateAsync(id, semesterRequest);
//                if (semester == null)
//                {
//                    return NotFound(new ApiResponse<SemesterResponse>(0, "Không tìm thấy học kỳ"));
//                }
//                return Ok(new ApiResponse<SemesterResponse>(1, "Cập nhật học kỳ thành công", semester));
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new ApiResponse<string>(0, ex.Message, null));
//            }
//            catch (BadRequestException ex)
//            {
//                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật học kỳ", ex.Message));
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult<ApiResponse<SemesterResponse>>> Delete(int id)
//        {
//            try
//            {
//                var semester = await _semesterService.DeleteAsync(id);
//                if (semester == null)
//                {
//                    return NotFound(new ApiResponse<SemesterResponse>(0, "Không tìm thấy học kỳ"));
//                }
//                return Ok(new ApiResponse<SemesterResponse>(1, "Xóa học kỳ thành công", semester));
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new ApiResponse<string>(0, ex.Message, null));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa học kỳ", ex.Message));
//            }
//        }
//    }
//}