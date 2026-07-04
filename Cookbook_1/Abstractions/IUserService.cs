using Cookbook_1.Contracts;
using Cookbook_1.Models;

namespace Cookbook_1.Abstractions
{
    public interface IUserService
    {
        public UserVm CreateNewUser(CreateUserDto user);

        public UserVm GetUserProfile(int userId);
        
        public UserVm UpdateUser(UpdateUserDto user);

        public void UpdateUserPassword(UpdateUserPasswordDto userPassword);

        public List<Recipe> ShowUserRecipes(int userId);
    }
}
