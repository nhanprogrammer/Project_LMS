using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IModulesService
    {
        Task<ApiResponse<List<ModuleResponse>>> GetAllModuleAsync();
        Task<ApiResponse<ModuleResponse>> CreateModuleAsync(CreateModuleRequest createModuleRequest);
        Task<ApiResponse<ModuleResponse>> UpdateModuleAsync(string id, UpdateModuleRequest updateModuleRequest);
        Task<ApiResponse<ModuleResponse>> DeleteModuleAsync(string id);
    }
}