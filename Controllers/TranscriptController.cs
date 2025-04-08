using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TranscriptController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;


        public TranscriptController(ITranscriptService transcriptService)
        {
            _transcriptService = transcriptService;

        }


        [HttpGet]
        public async Task<IActionResult> GetTranscriptAsync(TranscriptRequest transcriptRequest)
        {
            try
            {
                var result = await _transcriptService.GetTranscriptAsync(transcriptRequest);
                if (result.Status == 1)
                {
                    return BadRequest(result); // Trả về lỗi nếu có vấn đề trong xử lý
                }
                return Ok(result); // Trả về kết quả thành công
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object>(1, ex.Message, null)); // Xử lý lỗi không có quyền truy cập
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(1, $"Đã xảy ra lỗi không mong muốn: {ex.Message}", null)); // Xử lý lỗi không mong muốn
            }
        }
        [HttpGet("exportexcel")]
        public async Task<IActionResult> ExportExcelTranscriptAsync(TranscriptRequest transcriptRequest)
        {
            var result = await _transcriptService.ExportExcelTranscriptAsync(transcriptRequest);
            return Ok(result);
        }

        [HttpGet("transcriptbyteacher")]
        public async Task<IActionResult> GetTranscriptByTeacherAsync(TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.GetTranscriptByTeacherAsync(request);
            return Ok(result);
        }
        [HttpGet("exportexceltranscriptbyteacher")]
        public async Task<IActionResult> ExportExcelTranscriptByTeacherAsync(TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.ExportExcelTranscriptByTeacherAsync(request);
            return Ok(result);
        }
        [HttpGet("exportpdftranscriptbyteacher")]
        public async Task<IActionResult> ExportPDFTranscriptByTeacherAsync(TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.ExportPdfTranscriptByTeacherAsync(request);
            return Ok(result);
        }
        [HttpGet("dropdownofstudent")]
        public async Task<IActionResult> GetAllDropdownOfStudent()
        {
            var result = await _transcriptService.DropdownTranscriptStudent();
            return Ok(result);
        }

    }
}
