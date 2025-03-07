// Services/SubjectsGroupService.cs
using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class SubjectsGroupService : ISubjectsGroupService
    {
        private readonly ISubjectsGroupRepository _repository;
        private readonly IMapper _mapper;

        public SubjectsGroupService(ISubjectsGroupRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<SubjectsGroupResponse>>> GetAllSubjectsGroupsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var subjectsGroups = await _repository.GetAll(pageNumber, pageSize);
                var responses = _mapper.Map<List<SubjectsGroupResponse>>(subjectsGroups)
                    .OrderByDescending(sg => sg.Id)
                    .ToList();
                
                var paginatedResponse = new PaginatedResponse<SubjectsGroupResponse>
                {
                    Items = responses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = responses.Count,
                    TotalPages = (int)Math.Ceiling(responses.Count / (double)pageSize),
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = responses.Count == pageSize
                };

                return new ApiResponse<PaginatedResponse<SubjectsGroupResponse>>(0, "Success", paginatedResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedResponse<SubjectsGroupResponse>>(1, $"Error getting subjects groups: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<SubjectsGroupResponse>> GetSubjectsGroupByIdAsync(int id)
        {
            var subjectsGroup = await _repository.GetById(id);
            if (subjectsGroup == null)
                return new ApiResponse<SubjectsGroupResponse>(1, "SubjectsGroup not found", null);

            var response = _mapper.Map<SubjectsGroupResponse>(subjectsGroup);
            return new ApiResponse<SubjectsGroupResponse>(0, "Success", response);
        }

        public async Task<ApiResponse<SubjectsGroupResponse>> CreateSubjectsGroupAsync(SubjectsGroupRequest request)
        {
            var subjectsGroup = _mapper.Map<SubjectsGroup>(request);
            var created = await _repository.Add(subjectsGroup);
            var response = _mapper.Map<SubjectsGroupResponse>(created);
            return new ApiResponse<SubjectsGroupResponse>(0, "SubjectsGroup created successfully", response);
        }

        public async Task<ApiResponse<SubjectsGroupResponse>> UpdateSubjectsGroupAsync(int id, SubjectsGroupRequest request)
        {
            var subjectsGroup = _mapper.Map<SubjectsGroup>(request);
            var updated = await _repository.Update(id, subjectsGroup);
            if (updated == null)
                return new ApiResponse<SubjectsGroupResponse>(1, "SubjectsGroup not found", null);

            var response = _mapper.Map<SubjectsGroupResponse>(updated);
            return new ApiResponse<SubjectsGroupResponse>(0, "SubjectsGroup updated successfully", response);
        }

        public async Task<ApiResponse<bool>> DeleteSubjectsGroupAsync(int id)
        {
            var result = await _repository.Delete(id);
            if (!result)
                return new ApiResponse<bool>(1, "SubjectsGroup not found", false);

            return new ApiResponse<bool>(0, "SubjectsGroup deleted successfully", true);
        }
    }
}