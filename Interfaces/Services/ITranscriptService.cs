using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ITranscriptService
    {
        Task<ApiResponse<object>> GetTranscriptAsync(TranscriptRequest transcriptRequest);
        Task<ApiResponse<object>> ExportExcelTranscriptAsync(TranscriptRequest transcriptRequest);

        Task<ApiResponse<object>> GetTranscriptByTeacherAsync(TranscriptTeacherRequest request);
        Task<ApiResponse<object>> ExportExcelTranscriptByTeacherAsync(TranscriptTeacherRequest request);
        Task<ApiResponse<object>> ExportPdfTranscriptByTeacherAsync(TranscriptTeacherRequest request);


    }
}
