using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AssignmentDetailsService : IAssignmentDetailsService
    {
        private readonly IAssignmentDetailRepository _assignmentDetailRepository;
        private readonly IMapper _mapper;

        public AssignmentDetailsService(IAssignmentDetailRepository assignmentDetailRepository, IMapper mapper)
        {
            _assignmentDetailRepository = assignmentDetailRepository;
            _mapper = mapper;
        }

        public async Task<AssignmentDetailResponse> GetAssignmentDetailById(int id)
        {
            var assignmentDetail = await _assignmentDetailRepository.GetByIdAsync(id);
            return _mapper.Map<AssignmentDetailResponse>(assignmentDetail);
        }

        public async Task<IEnumerable<AssignmentDetailResponse>> GetAllAssignmentDetails()
        {
            var assignmentDetails = await _assignmentDetailRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AssignmentDetailResponse>>(assignmentDetails);
        }

        public async Task AddAssignmentDetail(CreateAssignmentDetailRequest request)
        {
            var assignmentDetail = _mapper.Map<AssignmentDetail>(request);
            await _assignmentDetailRepository.AddAsync(assignmentDetail);
        }

        public async Task UpdateAssignmentDetail(int id, UpdateAssignmentDetailRequest request)
        {
            var assignmentDetail = await _assignmentDetailRepository.GetByIdAsync(id);
            if (assignmentDetail == null)
            {
                throw new Exception("Assignment Detail not found");
            }
            _mapper.Map(request, assignmentDetail);
            await _assignmentDetailRepository.UpdateAsync(assignmentDetail);
        }

        public async Task<bool> DeleteAssignmentDetail(int id)
        {
            var assignmentDetail = await _assignmentDetailRepository.GetByIdAsync(id);
            if (assignmentDetail == null)
            {
                throw new Exception("Assignment Detail not found");
            }
            await _assignmentDetailRepository.DeleteAsync(id);
            return true;
        }
    }
}