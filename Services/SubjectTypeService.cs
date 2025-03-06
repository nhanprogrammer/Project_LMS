using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectTypeService : ISubjectTypeService
    {
        private readonly ISubjectTypeRepository _repository;
        private readonly IMapper _mapper;

        public SubjectTypeService(ISubjectTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectTypeResponse>>> GetAllSubjectTypesAsync(int pageNumber, int pageSize)
        {
            var subjectTypes = await _repository.GetAll(pageNumber, pageSize);
            var responses = _mapper.Map<List<SubjectTypeResponse>>(subjectTypes);
            
            var paginatedResponse = new PaginatedResponse<SubjectTypeResponse>
            {
                Items = responses,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = responses.Count,
                TotalPages = (int)Math.Ceiling(responses.Count / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = responses.Count == pageSize
            };

            return new ApiResponse<PaginatedResponse<SubjectTypeResponse>>(0, "Success", paginatedResponse);
        }

        public async Task<ApiResponse<SubjectTypeResponse>> GetSubjectTypeByIdAsync(int id)
        {
            var subjectType = await _repository.GetById(id);
            if (subjectType == null)
                return new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null);

            var response = _mapper.Map<SubjectTypeResponse>(subjectType);
            return new ApiResponse<SubjectTypeResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<SubjectTypeResponse>> CreateSubjectTypeAsync(SubjectTypeRequest request)
        {
            var subjectType = _mapper.Map<SubjectType>(request);
            var created = await _repository.Add(subjectType);
            var response = _mapper.Map<SubjectTypeResponse>(created);
            return new ApiResponse<SubjectTypeResponse>(0, "SubjectType created successfully", response);
        }

        public async Task<ApiResponse<SubjectTypeResponse>> UpdateSubjectTypeAsync(int id, SubjectTypeRequest request)
        {
            var subjectType = _mapper.Map<SubjectType>(request);
            var updated = await _repository.Update(id, subjectType);
            if (updated == null)
                return new ApiResponse<SubjectTypeResponse>(1, "SubjectType not found", null);

            var response = _mapper.Map<SubjectTypeResponse>(updated);
            return new ApiResponse<SubjectTypeResponse>(0, "SubjectType updated successfully", response);
        }

        public async Task<ApiResponse<bool>> DeleteSubjectTypeAsync(int id)
        {
            var result = await _repository.Delete(id);
            if (!result)
                return new ApiResponse<bool>(1, "SubjectType not found", false);

            return new ApiResponse<bool>(0, "SubjectType deleted successfully", true);
        }
    }
}