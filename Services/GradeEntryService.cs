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
            _logger.LogInformation($"Lấy dữ liệu chấm điểm cho testId: {testId}, teacherId: {teacherId}");
            var gradingData = await _gradeEntryRepository.GetGradingDataAsync(testId, teacherId);
            return new ApiResponse<GradingDataResponse>(0, "Lấy dữ liệu chấm điểm thành công!", gradingData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy dữ liệu chấm điểm: {ex.Message}");
            string detailedMessage = ex.Message;

            if (ex.Message.Contains("Không tìm thấy giáo viên"))
            {
                detailedMessage = "Không tìm thấy thông tin giáo viên trong hệ thống";
            }
            else if (ex.Message.Contains("Không tìm thấy bài kiểm tra"))
            {
                detailedMessage = "Không tìm thấy bài kiểm tra hoặc kỳ thi này trong hệ thống";
            }
            else if (ex.Message.Contains("Bài kiểm tra không được liên kết"))
            {
                detailedMessage = "Bài kiểm tra hoặc kỳ thi này chưa được gán cho lớp học nào";
            }
            else if (ex.Message.Contains("Không tìm thấy lớp học"))
            {
                detailedMessage = "Không tìm thấy thông tin lớp học trong hệ thống";
            }
            else if (ex.Message.Contains("Lớp này không học môn"))
            {
                detailedMessage = "Lớp học này không được gán môn học của bài kiểm tra hoặc kỳ thi";
            }
            else if (ex.Message.Contains("Bạn không có quyền"))
            {
                detailedMessage =
                    "Giáo viên không có quyền chấm điểm bài kiểm tra này vì không phải Admin, không phải giáo viên chủ nhiệm của lớp, và không được phân công dạy môn học này cho lớp vào thời điểm bài kiểm tra";
            }
            else if (ex.Message.Contains("chưa được phê duyệt"))
            {
                detailedMessage = "Bài kiểm tra hoặc kỳ thi này chưa được phê duyệt, không thể chấm điểm";
            }
            else if (ex.Message.Contains("chưa hoàn thành hoặc chưa kết thúc"))
            {
                detailedMessage =
                    "Bài kiểm tra hoặc kỳ thi này chưa hoàn thành hoặc chưa kết thúc, không thể chấm điểm";
            }
            else if (ex.Message.Contains("Chỉ Admin hoặc giáo viên chủ nhiệm"))
            {
                detailedMessage = "Chỉ Admin hoặc giáo viên chủ nhiệm của lớp mới có thể chấm điểm cho kỳ thi này";
            }
            else
            {
                detailedMessage = "Có lỗi xảy ra khi lấy dữ liệu chấm điểm, vui lòng thử lại sau";
            }

            return new ApiResponse<GradingDataResponse>(1, detailedMessage);
        }
    }

    public async Task<ApiResponse<bool>> SaveGrades(SaveGradesRequest request, int teacherId)
    {
        // Kiểm tra null cho request
        if (request == null)
        {
            _logger.LogWarning("SaveGradesRequest is null");
            return new ApiResponse<bool>(1, "Dữ liệu gửi lên không hợp lệ, vui lòng kiểm tra lại thông tin", false);
        }

        try
        {
            _logger.LogInformation($"Lưu điểm cho testId: {request.TestId}, teacherId: {teacherId}");

            var success = await _gradeEntryRepository.SaveGradesAsync(request, teacherId);
            if (success)
            {
                return new ApiResponse<bool>(0, "Lưu điểm thành công!", true);
            }

            return new ApiResponse<bool>(1, "Không thể lưu điểm, vui lòng thử lại sau", false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lưu điểm: {ex.Message}");
            string detailedMessage = "Có lỗi xảy ra khi lưu điểm, vui lòng thử lại sau";

            // Xử lý các trường hợp lỗi cụ thể từ SaveGradesAsync
            if (ex.Message.Contains("Không tìm thấy thông tin giáo viên"))
            {
                detailedMessage = "Không tìm thấy thông tin giáo viên trong hệ thống";
            }
            else if (ex.Message.Contains("Không tìm thấy bài kiểm tra"))
            {
                detailedMessage = "Không tìm thấy bài kiểm tra hoặc kỳ thi này trong hệ thống";
            }
            else if (ex.Message.Contains("Bài kiểm tra hoặc kỳ thi này chưa được gán cho lớp học"))
            {
                detailedMessage = "Bài kiểm tra hoặc kỳ thi này chưa được gán cho lớp học nào";
            }
            else if (ex.Message.Contains("Không tìm thấy thông tin lớp học"))
            {
                detailedMessage = "Không tìm thấy thông tin lớp học trong hệ thống";
            }
            else if (ex.Message.Contains("Lớp học này không được gán môn học"))
            {
                detailedMessage = "Lớp học này không được gán môn học của bài kiểm tra hoặc kỳ thi";
            }
            else if (ex.Message.Contains("Giáo viên không có quyền chấm điểm"))
            {
                detailedMessage =
                    "Giáo viên không có quyền chấm điểm bài kiểm tra này vì không phải Admin, không phải giáo viên chủ nhiệm của lớp, và không được phân công dạy môn học này cho lớp vào thời điểm bài kiểm tra";
            }
            else if (ex.Message.Contains("Bài kiểm tra hoặc kỳ thi này chưa hoàn thành hoặc chưa kết thúc"))
            {
                detailedMessage =
                    "Bài kiểm tra hoặc kỳ thi này chưa hoàn thành hoặc chưa kết thúc, không thể chấm điểm";
            }
            else if (ex.Message.Contains("Bài kiểm tra hoặc kỳ thi này vẫn đang diễn ra"))
            {
                detailedMessage = "Bài kiểm tra hoặc kỳ thi này vẫn đang diễn ra, chưa thể chấm điểm";
            }
            else if (ex.Message.Contains("Một hoặc nhiều học sinh trong danh sách điểm không thuộc lớp học"))
            {
                detailedMessage =
                    "Một hoặc nhiều học sinh trong danh sách điểm không thuộc lớp học này hoặc đã bị xóa khỏi hệ thống";
            }
            else if (ex.Message.Contains("Điểm số của một hoặc nhiều học sinh không hợp lệ"))
            {
                detailedMessage =
                    "Điểm số của một hoặc nhiều học sinh không hợp lệ, điểm phải nằm trong khoảng từ 0 đến 10";
            }

            return new ApiResponse<bool>(1, detailedMessage, false);
        }
    }
}