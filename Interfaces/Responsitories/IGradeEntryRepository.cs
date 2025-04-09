using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Responsitories;

public interface IGradeEntryRepository
{
    Task<GradingDataResponse> GetGradingDataAsync(int testId, int teacherId, int? classId = null);
    Task<bool> SaveGradesAsync(SaveGradesRequest request, int teacherId);
}