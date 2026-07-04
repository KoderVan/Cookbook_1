using Cookbook_1.Contracts;

namespace Cookbook_1.Abstractions
{
    public interface IAuthService
    {
        JwtTokenVm SignUp(CreateUserDto dto);
        JwtTokenVm LogIn(string login, string password);
        bool LogOut(int userId);
        bool VerifyToken(int userId, string token);
    }
}
