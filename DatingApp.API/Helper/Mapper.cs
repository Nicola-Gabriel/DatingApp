using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<User, UserForListDTO>()
                .ForMember( p=> p.PhotoURL, dest => {
                    dest.MapFrom(src => src.Photos.FirstOrDefault(g => g.IsMain).Url);
            })
                .ForMember(p => p.Age, dest => {
                    dest.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailDTO>()
                .ForMember( p=> p.PhotoURL, dest => {
                    dest.MapFrom(src => src.Photos.FirstOrDefault(g => g.IsMain).Url);
            })
                .ForMember(p => p.Age, dest => {
                    dest.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailedDTO>();
            CreateMap<UserForUpdateDTO, User>();
            CreateMap<Photo, PhotoToReturnDto>();
            CreateMap<PhotoForUploadDto, Photo>();
            CreateMap<UserFromRepoDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(p => p.SenderPhotoUrl, dest => {
                    dest.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(p => p.ReceiverPhotoUrl, dest => {
                    dest.MapFrom(u => u.Receiver.Photos.FirstOrDefault(p => p.IsMain).Url);
                });
        }

    }
}