using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IAddressService
    {
        Task<List<ProvinceDropdownResponse>> GetProvincesAsync();
        Task<List<DistrictDropdownResponse>> GetDistrictsByProvinceAsync(int provinceCode);
        Task<List<WardDropdownResponse>> GetWardsByDistrictAsync(int districtCode);
    }
}