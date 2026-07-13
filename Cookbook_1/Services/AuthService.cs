using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.Database;
using Cookbook_1.Models;
using Cookbook_1.Utils;
using Microsoft.EntityFrameworkCore;


namespace Cookbook_1.Services
{
    public class AuthService (ApplicationDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator) : IAuthService
    {
        //Возвращаем nullable, чтобы в контроллере независимо от проблем возвращать NotFound. 
        //Таким образом, даже если login верный, пользователь со стороны не узнат, существует ли у нас этот пользователь

        public LogInResponse SignUp(CreateUserDto dto)
        {
            var user = new User { Login = dto.Login, Password = PasswordHasher.HashPassword(dto.Password) };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var (jwt, refresh) = UpdateToken(user);
            dbContext.SaveChanges();
            //Сразу возвращаю токен после регистрации, чтобы его можно было сразу использовать
            return CreateResponse(jwt, refresh);
        }

        public LogInResponse? LogIn(string login, string password)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Login == login);
            if (user == null)
                return null;

            //сравниваем пароль из бд с переданным паролем
            if (!PasswordHasher.VerifyPassword(user.Password, password))
                return null;

            var (jwt, refresh) = UpdateToken(user);
            dbContext.SaveChanges();

            return CreateResponse(jwt, refresh);
        }

        public bool LogOut(int userId)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null) return false;

            var token = dbContext.JwtTokens.FirstOrDefault(x => x.Id == userId);
            if (token is null) return false;

            dbContext.Remove(token);
            dbContext.SaveChanges();

            return true;
        }

        public bool VerifyToken(int userId, string token)
        {
            var jwtToken = dbContext.JwtTokens.FirstOrDefault(x => x.UserId == userId);
            if (jwtToken is null) 
                return false;

            return jwtToken.Token == token && jwtToken.ExpiresAt > DateTime.UtcNow;
        }
        public LogInResponse? Refresh(string refreshToken)
        {
            var existingRefreshToken = dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefault(rt => rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow);
            if (existingRefreshToken is null)
                return null;
            var (jwt, refresh) = UpdateToken(existingRefreshToken.User);
            dbContext.SaveChanges();
            return CreateResponse(jwt, refresh);
        }
        private (JwtToken Jwt, RefreshToken Refresh) UpdateToken(User user)
        {
            var token = jwtTokenGenerator.Generate(user);
            var oldToken = dbContext.JwtTokens.FirstOrDefault(t => t.UserId == user.Id);
            if(oldToken is not null)
            {
                dbContext.Remove(oldToken);
            }
            dbContext.JwtTokens.Add(token);

            var RefreshToken = jwtTokenGenerator.GetRefreshToken(user.Id);
            dbContext.Add(RefreshToken);
            return (token, RefreshToken);
        }

        private static LogInResponse CreateResponse(JwtToken jwt, RefreshToken refresh) => new(jwt.UserId, jwt.Token, refresh.Token);
    }
}
