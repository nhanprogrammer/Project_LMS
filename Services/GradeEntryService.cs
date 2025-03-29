using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Services;

public class GradeEntryService : IGradeEntryService
{
    private readonly IGradeEntryRepository _gradeEntryRepository;
    private readonly ILogger<GradeEntryService> _logger;

    public GradeEntryService(IGradeEntryRepository gradeEntryRepository, ILogger<GradeEntryService> logger)
    {
        _gradeEntryRepository = gradeEntryRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<GradingDataResponse>> GetGradingData(int testId, int teacherId)
    {
        try
        {
            _logger.LogInformation(
                $"Lấy dữ liệu chấm điểm cho testId: {testId}, teacherId: {teacherId}");

            var gradingData = await _gradeEntryRepository.GetGradingDataAsync(testId, teacherId);
            return new ApiResponse<GradingDataResponse>(0, "Lấy dữ liệu chấm điểm thành công!", gradingData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy dữ liệu chấm điểm: {ex.Message}");
            return new ApiResponse<GradingDataResponse>(1, ex.Message, null);
        }
    }

    public async Task<ApiResponse<bool>> SaveGrades(SaveGradesRequest request, int teacherId)
    {
        // Kiểm tra null cho request
        if (request == null)
        {
            _logger.LogWarning("SaveGradesRequest is null");
            return new ApiResponse<bool>(1, "Dữ liệu yêu cầu không hợp lệ", false);
        }

        try
        {
            _logger.LogInformation(
                $"Lưu điểm cho testId: {request.TestId}, teacherId: {teacherId}");

            var success = await _gradeEntryRepository.SaveGradesAsync(request, teacherId);
            if (success)
            {
                return new ApiResponse<bool>(0, "Lưu điểm thành công!", true);
            }

            return new ApiResponse<bool>(1, "Lưu điểm thất bại!", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lưu điểm: {ex.Message}");
            return new ApiResponse<bool>(1, ex.Message, false);
        }
    }
}