using AutoMapper;
using Entities.DTOs;
using Entities.Models;

namespace FeedbackPlatform
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserForRegistrationDTO>();
            CreateMap<UserForRegistrationDTO, User>();
            CreateMap<TagToAddDTO, Tag>();
            CreateMap<GroupToAddDTO, Group>();
            CreateMap<ReviewToAddDTO, Review>();
            CreateMap<ReviewForManipulationDTO, Review>();
            CreateMap<CommentToLeaveDTO, Comment>();
            CreateMap<Artwork, ArtworkDTO>();
            CreateMap<Group, GroupDTO>();
            CreateMap<ReviewImage, ReviewImageDTO>();
            CreateMap<User, UserDTO>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<ReviewTag, ReviewTagDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Tag.Text));
            CreateMap<Tag, ReviewTagDTO>();
            CreateMap<string, Tag>()
            .ConstructUsing(src => new Tag { Text = src });
            CreateMap<Review, ReviewDTO>()
            .ForMember(dest => dest.Artwork, opt => opt.MapFrom(src => src.Artwork))
            .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.ReviewImages, opt => opt.MapFrom(src => src.ReviewImages))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
        }
    }
}
