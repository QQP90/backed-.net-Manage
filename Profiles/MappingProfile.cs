using AutoMapper;
using WebApplication1.Models.DTOs.User;
using WebApplication1.Models.Entities;
using WebApplication1.Models.DTOs;
using WebApplication1.Models.Entities;

namespace YourProject.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Entity -> UserDto
            CreateMap<User, UserDto>()
                .ReverseMap();

            // CreateUserDto -> User
            //CreateMap<CreateUserDto, User>();

            // UpdateUserDto -> User（只映射非null属性）
            CreateMap<UpdateUserDto, User>();
        }
    }
}