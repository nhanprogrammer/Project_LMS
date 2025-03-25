
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;


namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemptionController : ControllerBase
    {
        private readonly IExemptionService _exemptionService;

        public ExemptionController(IExemptionService exemptionService)
        {
            _exemptionService = exemptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]ExemptionRequest request)
        {
            var result = await _exemptionService.AddAsync(request);
            return Ok(result);
        }
    }
}
