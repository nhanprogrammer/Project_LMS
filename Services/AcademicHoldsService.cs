using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly IAcademicHoldRepository _academicHoldRepository;

        private readonly IMapper _mapper;

        public AcademicHoldsService(IAcademicHoldRepository academicHoldRepository, IMapper mapper)
        {
            _academicHoldRepository = academicHoldRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AcademicHoldResponse>> GetAllAcademicHold()
        {
            var academicHolds = await _academicHoldRepository.GetAllAsync();
            return _mapper.Map<List<AcademicHoldResponse>>(academicHolds);
        }

        public async Task<AcademicHoldResponse> GetByIdAcademicHold(int id)
        {
            var academicHold = await _academicHoldRepository.GetByIdAsync(id);
            return _mapper.Map<AcademicHoldResponse>(academicHold);
        }

        public async Task AddAcademicHold(CreateAcademicHoldRequest academicHoldRequest)
        {
            var academicHold = _mapper.Map<AcademicHold>(academicHoldRequest);
            await _academicHoldRepository.AddAsync(academicHold);
        }

        public async Task UpdateAcademicHold(UpdateAcademicHoldRequest academicHoldRequest)
        {
            var existingAcademicHold = await _academicHoldRepository.GetByStudentIdAsync((int)academicHoldRequest.StudentId);

            if (existingAcademicHold == null)
            {
                throw new Exception("Academic Hold not found");
            }

            _mapper.Map(academicHoldRequest, existingAcademicHold);
            await _academicHoldRepository.UpdateAsync(existingAcademicHold);
        }

        public async Task DeleteAcademicHold(int id)
        {
            await _academicHoldRepository.DeleteAsync(id);
        }
    }
}