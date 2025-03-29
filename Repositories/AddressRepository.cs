using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProvinceDropdownResponse>> GetProvincesAsync()
        {
            return await _context.Provinces
                .Select(p => new ProvinceDropdownResponse
                {
                    Code = p.Code,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<List<DistrictDropdownResponse>> GetDistrictsByProvinceAsync(int provinceCode)
        {
            var districts = await _context.Districts
                .Where(d => d.ProvinceCode == provinceCode)
                .Select(d => new DistrictDropdownResponse
                {
                    Code = d.Code,
                    Name = d.Name
                })
                .ToListAsync();

            if (!districts.Any())
            {
                throw new NotFoundException($"Không tìm thấy quận/huyện nào thuộc tỉnh có mã {provinceCode}.");
            }

            return districts;
        }

        public async Task<List<WardDropdownResponse>> GetWardsByDistrictAsync(int districtCode)
        {
            var wards = await _context.Wards
                .Where(w => w.DistrictCode == districtCode)
                .Select(w => new WardDropdownResponse
                {
                    Code = w.Code,
                    Name = w.Name
                })
                .ToListAsync();

            if (!wards.Any())
            {
                throw new NotFoundException($"Không tìm thấy xã/phường nào thuộc quận/huyện có mã {districtCode}.");
            }

            return wards;
        }
    }
}