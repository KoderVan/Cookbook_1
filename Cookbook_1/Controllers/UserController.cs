using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.Services;
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
        public ActionResult<JwtTokenVm> CreateNewUser([FromBody] CreateUserDto userDto)
        {
            var token = authService.SignUp(userDto);
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<JwtTokenVm> LogIn([FromBody] LogInUserDto dto)
        {
            var result = authService.LogIn(dto.Login, dto.Password);
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
        
        //это потом убрать
        [HttpGet("/user/{userId}")]
        public IActionResult GetUser(int userId)
        {
            var user = userService.GetUserProfile(userId);
            return Ok(user);
        }

    }
}
