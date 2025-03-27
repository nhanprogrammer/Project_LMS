using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IEducationInformationService
{
    // Lấy tất cả EducationInformation
    Task<IEnumerable<EducationInformationsResponse>> GetAllAsync(EducationInformationRequest request);
    Task<EducationInformationResponse> GetById(EducationInformationDeleteRequest request);
    // Lấy tất cả TrainingProgram không bao gồm các TrainingProgram có Id trong chuỗi ids
    Task<IEnumerable<WorkUnitResponse>> GetTrainingProgramsExcluding(TrainingProgramRequest request);
    // Thêm mới EducationInformation
    Task<bool> CreateAsync(EducationInformationCreateRequest request);
    // Cập nhật EducationInformation
    Task<bool> UpdateAsync(EducationInformationUpdateRequest request);
    // Xóa mềm EducationInformation
    Task<bool> DeleteAsync(EducationInformationDeleteRequest request);
}