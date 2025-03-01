using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IFavouritesService
    {
        Task<ApiResponse<List<FavouriteResponse>>> GetAllFavouriteAsync();
        Task<ApiResponse<FavouriteResponse>> CreateFavouriteAsync(CreateFavouriteRequest createFavouriteRequest);
        Task<ApiResponse<FavouriteResponse>> UpdateFavouriteAsync(string id, UpdateFavouriteRequest updateFavouriteRequest);
        Task<ApiResponse<FavouriteResponse>> DeleteFavouriteAsync(string id);
    }
}