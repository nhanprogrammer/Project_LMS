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
        //private readonly IAuthService _authService;

        public TranscriptController(ITranscriptService transcriptService)
        {
            _transcriptService = transcriptService;
            //_authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }


        [HttpGet]
        public async Task<IActionResult> GetTranscriptAsync([FromQuery]TranscriptRequest transcriptRequest)
        {
            //var user = await _authService.GetUserAsync();
            //if (user == null)
            //    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            //var userId = user.Id;

            var result = await _transcriptService.GetTranscriptAsync(transcriptRequest);
            return Ok(result);
        }     
        [HttpGet("exportexcel")]
        public async Task<IActionResult> ExportExcelTranscriptAsync([FromQuery]TranscriptRequest transcriptRequest)
        {
            //var user = await _authService.GetUserAsync();
            //if (user == null)
            //    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            //var userId = user.Id;

            var result = await _transcriptService.ExportExcelTranscriptAsync(transcriptRequest);
            return Ok(result);
        }


    }
}
