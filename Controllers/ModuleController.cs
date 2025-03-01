using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ModuleController : ControllerBase
{
   private readonly IModulesService _modulesService;

   public ModuleController(IModulesService modulesService)
   {
      _modulesService = modulesService;
   }
   
   
   
   [HttpGet]
   public async Task<IActionResult>  GetAllLessonAsync()
   {
      var response = await _modulesService.GetAllModuleAsync();

      if (response.Status == 1)
      {
         return BadRequest(new ApiResponse<List<ModuleResponse>>(response.Status, response.Message,response.Data)); 
      }

      return Ok(new ApiResponse<List<ModuleResponse>>(response.Status, response.Message, response.Data));
   }

   [HttpPost]
   public async Task<IActionResult> CreateFavouriteAsync([FromBody] CreateModuleRequest request)
   {
      var response = await _modulesService.CreateModuleAsync(request);

      if (response.Status == 1)
      {
         return BadRequest(new ApiResponse<ModuleResponse>(response.Status, response.Message,response.Data)); 
      }

      return Ok(new ApiResponse<ModuleResponse>(response.Status, response.Message, response.Data));
   }

   [HttpPut("{id?}")]
   public async Task<IActionResult> UpdateFavourite(String id, [FromBody] UpdateModuleRequest request)
   {
      var response =   await _modulesService.UpdateModuleAsync(id, request);
      if (response.Status == 1)
      {
         return BadRequest(new ApiResponse<ModuleResponse>(response.Status, response.Message,response.Data)); 
      }

      return Ok(new ApiResponse<ModuleResponse>(response.Status, response.Message, response.Data));
   }

   [HttpDelete("{id?}")]
   public async Task<IActionResult> DeleteDepartment(String id)
   {
      var response = await _modulesService.DeleteModuleAsync(id);
      if (response.Status == 1)
      {
         return BadRequest(new ApiResponse<ModuleResponse>(response.Status, response.Message,response.Data)); 
      }

      return Ok(new ApiResponse<ModuleResponse>(response.Status, response.Message, response.Data));
   }

}