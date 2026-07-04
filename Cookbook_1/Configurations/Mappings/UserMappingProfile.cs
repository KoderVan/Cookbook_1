using AutoMapper;
using Cookbook_1.Contracts;
using Cookbook_1.Models;

namespace Cookbook_1.Configurations.Mappings
{
    public class UserMappingProfile :Profile
    {
        public UserMappingProfile()
        {
            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>();

            CreateMap<User, UserVm>();


        }
    }
}
