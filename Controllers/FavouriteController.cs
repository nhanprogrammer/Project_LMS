using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FavouriteController : ControllerBase
{
    private readonly IFavouritesService _favouritesService;

    public FavouriteController(IFavouritesService favouritesService)
    {
        _favouritesService = favouritesService;
    }


    [HttpGet]
        public async Task<IActionResult>  GetAllFavouriteAsync()
        {
            var response = await _favouritesService.GetAllFavouriteAsync();

            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<List<FavouriteResponse>>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<List<FavouriteResponse>>(response.Status, response.Message, response.Data));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateFavouriteAsync([FromBody] CreateFavouriteRequest request)
        {
            var response = await _favouritesService.CreateFavouriteAsync(request);

            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<FavouriteResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<FavouriteResponse>(response.Status, response.Message, response.Data));
        }
        
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateFavourite(String id, [FromBody] UpdateFavouriteRequest request)
        {
            var response =   await _favouritesService.UpdateFavouriteAsync(id, request);
            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<FavouriteResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<FavouriteResponse>(response.Status, response.Message, response.Data));
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteFavourite(String id)
        {
            var response = await _favouritesService.DeleteFavouriteAsync(id);
            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<FavouriteResponse>(response.Status, response.Message,response.Data)); 
            }

            return Ok(new ApiResponse<FavouriteResponse>(response.Status, response.Message, response.Data));
        }
    
    
}