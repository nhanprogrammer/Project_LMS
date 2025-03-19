using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
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
        public async Task<IActionResult> GetTranscriptAsync([FromBody] TranscriptRequest transcriptRequest)
        {
            var result = await _transcriptService.GetTranscriptAsync(transcriptRequest);
            return Ok(result);
        }
    }
}
