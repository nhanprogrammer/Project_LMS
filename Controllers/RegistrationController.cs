using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationResponse>>>> GetAll()
        {
            try
            {
                var registrations = await _registrationService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<RegistrationResponse>>(1, "Lấy danh sách đăng ký thành công", registrations));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách đăng ký", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationResponse>>> GetById(int id)
        {
            try
            {
                var registration = await _registrationService.GetByIdAsync(id);
                if (registration == null)
                {
                    return NotFound(new ApiResponse<RegistrationResponse>(0, "Không tìm thấy đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationResponse>(1, "Lấy đăng ký thành công", registration));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy đăng ký", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RegistrationResponse>>> Create(RegistrationRequest request)
        {
            try
            {
                var registration = await _registrationService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = registration.Id }, new ApiResponse<RegistrationResponse>(1, "Tạo đăng ký thành công", registration));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo đăng ký", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationResponse>>> Update(int id, RegistrationRequest request)
        {
            try
            {
                var registration = await _registrationService.UpdateAsync(id, request);
                if (registration == null)
                {
                    return NotFound(new ApiResponse<RegistrationResponse>(0, "Không tìm thấy đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationResponse>(1, "Cập nhật đăng ký thành công", registration));
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
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật đăng ký", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationResponse>>> Delete(int id)
        {
            try
            {
                var registration = await _registrationService.DeleteAsync(id);
                if (registration == null)
                {
                    return NotFound(new ApiResponse<RegistrationResponse>(0, "Không tìm thấy đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationResponse>(1, "Xóa đăng ký thành công", registration));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa đăng ký", ex.Message));
            }
        }
    }
}