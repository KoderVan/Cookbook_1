using Cookbook_1.Contracts;

namespace Cookbook_1.Abstractions
{
    public interface IAuthService
    {
        LogInResponse SignUp(CreateUserDto dto);
        LogInResponse? LogIn(string login, string password);
        bool LogOut(int userId);
        bool VerifyToken(int userId, string token);

        LogInResponse? Refresh(string refreshToken);
    }
}
