using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClassOnlineController : ControllerBase
{
    private readonly IClassOnlineService _classOnlineService;

    public ClassOnlineController(IClassOnlineService classOnlineService)
    {
        _classOnlineService = classOnlineService;
    }

     [HttpGet]
        public async Task<IActionResult>  GetAllDisciplinesAsync()
        {
            var response = await _classOnlineService.GetAllClassOnlineAsync();

            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<List<ClassOnlineResponse>>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<List<ClassOnlineResponse>>(response.Status, response.Message, response.Data));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateDisciplinesAsync([FromBody] CreateClassOnlineRequest request)
        {
            var response = await _classOnlineService.CreateClassStudentAsync(request);

            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message, response.Data));
        }
        
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateDiscipline(String id, [FromBody] UpdateClassOnlineRequest request)
        {
  
            var response = await _classOnlineService.UpdateClassStudentAsync(id, request);

            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message, response.Data));
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteDepartment(String id)
        {
            var response = await _classOnlineService.DeleteClassStudentAsync(id);
            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<ClassOnlineResponse>(response.Status, response.Message, response.Data));
        }

}