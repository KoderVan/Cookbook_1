using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {  
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        public RecipeController(IRecipeService recipeService, IIngredientService ingredientService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
        }

        [Authorize]
        [HttpPost("/newrecipe")]
        public IActionResult CreateNewRecipe(CreateRecipeDto dto)
        {
            var newRecipe = _recipeService.CreateRecipe(dto);
            return Ok(newRecipe);
            
        }

        [AllowAnonymous]
        [HttpGet("/recipe/{id}")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _recipeService.GetRecipe(id);
            return Ok(recipe);
        }

        //Тут наверно надо как то ограничить, что это может делать только создатель рецепта
        [Authorize]
        [HttpPut("/{id}/update")]
        public IActionResult UpdateRecipe(UpdateRecipeDto dto)
        {
            _recipeService.UpdateRecipe(dto); 
            return Ok();
        }

        [Authorize]
        [HttpDelete("/{id}/delete")]
        public IActionResult DeleteRecipe(int id)
        {
            _recipeService.DeleteRecipe(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("/{id}/rate")]
        public IActionResult RateRecipe(int id, RecipeRating rating)
        {
            _recipeService.RateTheRecipe(id, rating);
            return Ok();
        }

        [Authorize]
        [HttpPost("/newingredient")]
        public IActionResult AddIngredient(string name)
        {
            _ingredientService.AddIngredient(name);
            return Ok();
        } 
    }
}
