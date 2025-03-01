using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AssignmentsService : IAssignmentsService
    {
        private readonly IAssignmentRepository _assignmentsRepository;
        private readonly IMapper _mapper;

        public AssignmentsService(IAssignmentRepository assignmentsRepository, IMapper mapper)
        {
            _assignmentsRepository = assignmentsRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AssignmentsResponse>> GetAllAssignments()
        {
            var assignments = await _assignmentsRepository.GetAllAsync();
            return _mapper.Map<List<AssignmentsResponse>>(assignments);
        }

        public async Task<AssignmentsResponse?> GetAssignmentById(int id)
        {
            var assignment = await _assignmentsRepository.GetByIdAsync(id);
            return _mapper.Map<AssignmentsResponse>(assignment);
        }

        public async Task AddAssignment(CreateAssignmentRequest request)
        {
            var assignment = _mapper.Map<Assignment>(request);
            await _assignmentsRepository.AddAsync(assignment);
        }

        public async Task UpdateAssignment(int id, UpdateAssignmentRequest request)
        {
            var existingAssignment = await _assignmentsRepository.GetByIdAsync(id);
            if (existingAssignment == null)
            {
                throw new Exception("Assignment not found");
            }
            _mapper.Map(request, existingAssignment);
            await _assignmentsRepository.UpdateAsync(existingAssignment);
        }

        public async Task<bool> DeleteAssignment(int id)
        {
            var existingAssignment = await _assignmentsRepository.GetByIdAsync(id);
            if (existingAssignment == null)
            {
                return false;
            }
            await _assignmentsRepository.DeleteAsync(id);
            return true;
        }
    }
}