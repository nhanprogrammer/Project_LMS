using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationContactController : ControllerBase
    {
        private readonly IRegistrationContactsService _registrationContactService;

        public RegistrationContactController(IRegistrationContactsService registrationContactService)
        {
            _registrationContactService = registrationContactService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RegistrationContactResponse>>>> GetAll()
        {
            try
            {
                var registrationContacts = await _registrationContactService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<RegistrationContactResponse>>(1, "Lấy danh sách liên hệ đăng ký thành công", registrationContacts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách liên hệ đăng ký", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationContactResponse>>> GetById(int id)
        {
            try
            {
                var registrationContact = await _registrationContactService.GetByIdAsync(id);
                if (registrationContact == null)
                {
                    return NotFound(new ApiResponse<RegistrationContactResponse>(0, "Không tìm thấy liên hệ đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationContactResponse>(1, "Lấy liên hệ đăng ký thành công", registrationContact));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy liên hệ đăng ký", ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RegistrationContactResponse>>> Create(RegistrationContactRequest request)
        {
            try
            {
                var registrationContact = await _registrationContactService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = registrationContact.Id }, new ApiResponse<RegistrationContactResponse>(1, "Tạo liên hệ đăng ký thành công", registrationContact));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo liên hệ đăng ký", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationContactResponse>>> Update(int id, RegistrationContactRequest request)
        {
            try
            {
                var registrationContact = await _registrationContactService.UpdateAsync(id, request);
                if (registrationContact == null)
                {
                    return NotFound(new ApiResponse<RegistrationContactResponse>(0, "Không tìm thấy liên hệ đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationContactResponse>(1, "Cập nhật liên hệ đăng ký thành công", registrationContact));
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
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật liên hệ đăng ký", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RegistrationContactResponse>>> Delete(int id)
        {
            try
            {
                var registrationContact = await _registrationContactService.DeleteAsync(id);
                if (registrationContact == null)
                {
                    return NotFound(new ApiResponse<RegistrationContactResponse>(0, "Không tìm thấy liên hệ đăng ký"));
                }
                return Ok(new ApiResponse<RegistrationContactResponse>(1, "Xóa liên hệ đăng ký thành công", registrationContact));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(0, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa liên hệ đăng ký", ex.Message));
            }
        }
    }
}