using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IWorkProcessService
{
    // Lấy tất cả WorkProcess
    Task<IEnumerable<WorkProcessesResponse>> GetAllAsync(WorkProcessRequest request);
    Task<WorkProcessResponse> GetById(WorkProcessDeleteRequest request);
    // Lấy tất cả WorkUnit không bao gồm các WorkUnit có Id trong chuỗi ids
    Task<IEnumerable<WorkUnitResponse>> GetWorkUnitExcluding(WorkUnitRequest request);
    // Thêm mới WorkProcess
    Task<bool> CreateAsync(WorkProcessCreateRequest request);
    // Cập nhật WorkProcess
    Task<bool> UpdateAsync(WorkProcessUpdateRequest request);
    Task<bool> DeleteAsync(WorkProcessDeleteRequest request);

}