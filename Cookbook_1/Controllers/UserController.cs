using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook_1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController(IAuthService authService, IUserService userService, IRecipeService recipeService) : ControllerBase
    {

        //Этот метод доступен без авторизации
        [AllowAnonymous]
        [HttpPost("/registration")]
        public ActionResult<LogInResponse> CreateNewUser([FromBody] CreateUserDto userDto)
        {
            var token = authService.SignUp(userDto);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<LogInResponse> LogIn([FromBody] LogInUserDto dto)
        {
            var result = authService.LogIn(dto.Login, dto.Password);
            if (result is null)
                return Unauthorized();

            return Ok(result); 
        }

        [HttpPost("logout")]
        public ActionResult<bool> LogOut([FromBody] int userId)
        {
            var result = authService.LogOut(userId);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("refresh")]
        public ActionResult<LogInResponse> Refresh([FromBody] string refreshToken)
        {
            var result = authService.Refresh(refreshToken);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        // тест гита
    }
}
