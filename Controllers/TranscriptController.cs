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
    public class TranscriptController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;

        public TranscriptController(ITranscriptService transcriptService)
        {
            _transcriptService = transcriptService;
        }


        [HttpGet]
        public async Task<IActionResult> GetTranscriptAsync([FromQuery]TranscriptRequest transcriptRequest)
        {

            var result = await _transcriptService.GetTranscriptAsync(transcriptRequest);
            return Ok(result);
        }     
        [HttpGet("exportexcel")]
        public async Task<IActionResult> ExportExcelTranscriptAsync([FromQuery]TranscriptRequest transcriptRequest)
        {
            var result = await _transcriptService.ExportExcelTranscriptAsync(transcriptRequest);
            return Ok(result);
        }

        [HttpGet("transcriptbyteacher")]
        public async Task<IActionResult> GetTranscriptByTeacherAsync([FromQuery] TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.GetTranscriptByTeacherAsync(request);
            return Ok(result);
        }
        [HttpGet("exportexceltranscriptbyteacher")]
        public async Task<IActionResult> ExportExcelTranscriptByTeacherAsync([FromQuery] TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.ExportExcelTranscriptByTeacherAsync(request);
            return Ok(result);
        }
        [HttpGet("exportpdftranscriptbyteacher")]
        public async Task<IActionResult> ExportPDFTranscriptByTeacherAsync([FromQuery] TranscriptTeacherRequest request)
        {

            var result = await _transcriptService.ExportPdfTranscriptByTeacherAsync(request);
            return Ok(result);
        }


    }
}
