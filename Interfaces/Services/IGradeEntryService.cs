using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface IGradeEntryService
{
    Task<ApiResponse<GradingDataResponse>> GetGradingData(int testId, int teacherId, int? classId = null);

    Task<ApiResponse<bool>> SaveGrades(SaveGradesRequest request, int teacherId);
}