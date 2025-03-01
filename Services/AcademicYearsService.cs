using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicYearsService : IAcademicYearsService
    {
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IMapper _mapper;

        public AcademicYearsService(IAcademicYearRepository academicYearRepository, IMapper mapper)
        {
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AcademicYearResponse>> GetAllAcademicYears()
        {
            var academicYears = await _academicYearRepository.GetAllAsync();
            return _mapper.Map<List<AcademicYearResponse>>(academicYears);
        }

        public async Task<AcademicYearResponse> GetByIdAcademicYear(int id)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(id);
            return _mapper.Map<AcademicYearResponse>(academicYear);
        }

        public async Task AddAcademicYear(CreateAcademicYearRequest request)
        {
            var academicYear = _mapper.Map<AcademicYear>(request);
            await _academicYearRepository.AddAsync(academicYear);
        }

        public async Task UpdateAcademicYear(int id, UpdateAcademicYearRequest request)
        {
            var existingAcademicYear = await _academicYearRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                throw new Exception("Academic Year not found");
            }

            _mapper.Map(request, existingAcademicYear);
            await _academicYearRepository.UpdateAsync(existingAcademicYear);
        }

        public async Task<bool> DeleteAcademicYear(int id)
        {
            var existingAcademicYear = await _academicYearRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                return false;
            }

            await _academicYearRepository.DeleteAsync(id);
            return true;
        }
    }
}