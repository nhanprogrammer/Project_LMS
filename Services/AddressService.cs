using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<List<ProvinceDropdownResponse>> GetProvincesAsync()
        {
            return await _addressRepository.GetProvincesAsync();
        }

        public async Task<List<DistrictDropdownResponse>> GetDistrictsByProvinceAsync(int provinceCode)
        {
            return await _addressRepository.GetDistrictsByProvinceAsync(provinceCode);
        }

        public async Task<List<WardDropdownResponse>> GetWardsByDistrictAsync(int districtCode)
        {
            return await _addressRepository.GetWardsByDistrictAsync(districtCode);
        }
    }
}