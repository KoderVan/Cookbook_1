using Cookbook_1.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Cookbook_1.Services
{
    public class ExceptionHandler : IExceptionHandler //Я так понял, это что-то встроенное
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) //ЧТо за ValueTask ещё
        {
            httpContext.Response.StatusCode = exception switch //Пока асинк плохо знаю, но примерно понимаю что это
            {
                RecipeNotFoundException => (int)HttpStatusCode.NotFound,
                RecipeAlreadyExistsException => (int)HttpStatusCode.Conflict,
                IngredientNotFoundException => (int)HttpStatusCode.NotFound,
                IngredientAlreadyExistsException => (int)HttpStatusCode.Conflict,
            };

            await httpContext.Response.WriteAsync(exception.Message); //Пришлось с асинком писать, не понял как правильно без него сделать (просто у Мишы всё переписал)
            return true;
            
        }
    }
}
