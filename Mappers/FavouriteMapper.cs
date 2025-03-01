using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Mappers
{
    public class FavouriteMapper : Profile
    {
        public FavouriteMapper()
        {
            CreateMap<Favourite, FavouriteResponse>()
                .ForMember(dest => dest.QuestionsAnswerId, opt => opt.MapFrom(src => src.QuestionsAnswerId)) // Map QuestionsAnswerId
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserId
                .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicId)); // Map TopicId
            CreateMap<Favourite, CreateFavouriteRequest>();
            CreateMap<Favourite, UpdateFavouriteRequest>();
        }
    }
}