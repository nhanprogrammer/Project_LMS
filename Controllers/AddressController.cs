using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _addressService.GetProvincesAsync();
            return Ok(new ApiResponse<List<ProvinceDropdownResponse>>(0, "Lấy danh sách tỉnh/thành phố thành công!", provinces));
        }

        [HttpGet("districts/{provinceCode}")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceCode)
        {
            try
            {
                var districts = await _addressService.GetDistrictsByProvinceAsync(provinceCode);
                return Ok(new ApiResponse<List<DistrictDropdownResponse>>(0, "Lấy danh sách quận/huyện thành công!", districts));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách quận/huyện", ex.Message));
            }
        }

        [HttpGet("wards/{districtCode}")]
        public async Task<IActionResult> GetWardsByDistrict(int districtCode)
        {
            try
            {
                var wards = await _addressService.GetWardsByDistrictAsync(districtCode);
                return Ok(new ApiResponse<List<WardDropdownResponse>>(0, "Lấy danh sách xã/phường thành công!", wards));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách xã/phường", ex.Message));
            }
        }
    }
}