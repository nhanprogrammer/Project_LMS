using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class FavouritesService : IFavouritesService
    {
        private readonly IFavouriteRepository _favouriteRepository;
        private readonly ApplicationDbContext _context;

        public FavouritesService(IFavouriteRepository favouriteRepository, ApplicationDbContext context)
        {
            _favouriteRepository = favouriteRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<FavouriteResponse>>> GetAllFavouriteAsync()
        {
            var favourites = await _favouriteRepository.GetAllAsync();
            var data = favourites.Select(c => new FavouriteResponse
            {
               TopicId = c.TopicId,
               Id = c.Id,
               UserId = c.UserId,
               QuestionsAnswerId = c.QuestionsAnswerId,
                
            }).ToList();
    
            return new ApiResponse<List<FavouriteResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<FavouriteResponse>> CreateFavouriteAsync(CreateFavouriteRequest createFavouriteRequest)
        {
            var favourite = new Favourite
            {
                TopicId = createFavouriteRequest.TopicId,
                UserId = createFavouriteRequest.UserId,
                QuestionsAnswerId = createFavouriteRequest.QuestionsAnswerId,
            };
          await _favouriteRepository.AddAsync(favourite);
          var response = new FavouriteResponse
          {  
              TopicId = favourite.TopicId,
              Id = favourite.Id,
              UserId = favourite.UserId,
              QuestionsAnswerId =favourite.QuestionsAnswerId,
             
          };
          return new ApiResponse<FavouriteResponse>(0, "Đã thích thành công", response);
        }

        public async Task<ApiResponse<FavouriteResponse>> UpdateFavouriteAsync(string id, UpdateFavouriteRequest updateFavouriteRequest)
        {
            if (!int.TryParse(id, out int disciplineId))
            {
                return new ApiResponse<FavouriteResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var favourite = await _favouriteRepository.GetByIdAsync(disciplineId);
            if (favourite == null)
            {
                return new ApiResponse<FavouriteResponse>(1, "Không tìm thấy favourte.", null);
            }
            favourite.TopicId = updateFavouriteRequest.TopicId;
            favourite.UserId = updateFavouriteRequest.UserId;
            favourite.QuestionsAnswerId = updateFavouriteRequest.QuestionsAnswerId;
            await _favouriteRepository.UpdateAsync(favourite);
            
            var response = new FavouriteResponse
            {  
                TopicId = favourite.TopicId,
                Id = favourite.Id,
                UserId = favourite.UserId,
                QuestionsAnswerId =favourite.QuestionsAnswerId,
             
            };
            return new ApiResponse<FavouriteResponse>(0, "Đã thích thành công", response);
        }

        public async Task<ApiResponse<FavouriteResponse>> DeleteFavouriteAsync(string id)
        {
            if (!int.TryParse(id, out int favouriteId))
            {
                return new ApiResponse<FavouriteResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var favourite = await _favouriteRepository.GetByIdAsync(favouriteId);
            if (favourite == null)
            {
                return new ApiResponse<FavouriteResponse>(1, "Không tìm thấy favourte.", null);
            }
            await _favouriteRepository.DeleteAsync(favouriteId);
            
            
            return new ApiResponse<FavouriteResponse>(0, "Department đã xóa thành công ");
        }
    }
}