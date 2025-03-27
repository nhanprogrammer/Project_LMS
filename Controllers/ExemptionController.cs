
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;


namespace Project_LMS.Controllers
{
    [Authorize(Policy = "STUDENT-REC-VIEW")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExemptionController : ControllerBase
    {
        private readonly IExemptionService _exemptionService;

        public ExemptionController(IExemptionService exemptionService)
        {
            _exemptionService = exemptionService;
        }

        [Authorize(Policy = "STUDENT-REC-INSERT")]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]ExemptionRequest request)
        {
            var result = await _exemptionService.AddAsync(request);
            return Ok(result);
        }
    }
}
