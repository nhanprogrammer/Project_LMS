using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ITranscriptService
    {
        Task<ApiResponse<List<TranscriptReponse>>> GetTranscriptAsync(TranscriptRequest transcriptRequest);
    }
}
