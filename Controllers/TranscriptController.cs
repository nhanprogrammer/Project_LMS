using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "DATA-MNG-VIEW")]
    [ApiController]
    [Route("api/[controller]")]
    public class TranscriptController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;
        private readonly IAuthService _authService;

        public TranscriptController(ITranscriptService transcriptService, IAuthService authService)
        {
            _transcriptService = transcriptService;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }


        [HttpGet]
        public async Task<IActionResult> GetTranscriptAsync([FromBody] TranscriptRequest transcriptRequest)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var userId = user.Id;

            Console.WriteLine($"TeacherId từ token: {userId}");
            var result = await _transcriptService.GetTranscriptAsync(transcriptRequest, userId);
            return Ok(result);
        }


    }
}
