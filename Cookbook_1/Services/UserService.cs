using AutoMapper;
using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook_1.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _applicationDbContext;

        public UserService(IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _mapper = mapper; 
            _applicationDbContext = applicationDbContext;
        }
        public UserVm CreateNewUser(CreateUserDto user)
        {
            var newUser = _mapper.Map<User>(user);
            _applicationDbContext.Users.Add(newUser);
            _applicationDbContext.SaveChanges();
            var userVm = _mapper.Map<UserVm>(newUser);
            return userVm;
        }

        public UserVm GetUserProfile(int userId)
        {
            var user = GetUserWithIngredientsOrThrowException(userId);
            var userVm = _mapper.Map<UserVm>(user);
            return userVm;
        }

        public UserVm UpdateUser(UpdateUserDto user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPassword(UpdateUserPasswordDto userPassword)
        {
            throw new NotImplementedException();
        }

        List<Recipe> IUserService.ShowUserRecipes(int userId)
        {
            throw new NotImplementedException();
        }

        private User GetUserWithIngredientsOrThrowException(int userId)
        {
            var user = _applicationDbContext.Users.Where(user => user.Id == userId)
                .Include(r => r.UserRecipes).FirstOrDefault() ?? throw new UserNotFoundException(userId);
            return user;
        }
    }
}
