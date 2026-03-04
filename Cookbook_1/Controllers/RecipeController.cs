using Cookbook_1.Abstractions;
using Cookbook_1.ENums;
using Cookbook_1.Models;
using Cookbook_1.TempStorage;
using Microsoft.AspNetCore.Mvc;

namespace Cookbook_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {  
        private IRecipeService _recipeService;
        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }
        [HttpGet("recipelist")]
        public IActionResult GetRecipeList()
        {
            var recipes = _recipeService.GetAllRecipes();
            return Ok(recipes);
        }

        [HttpPost("newrecipe")]
        public IActionResult CreateNewRecipe(int id,  string name, string description)
        {
            var newRecipe = _recipeService.CreateRecipe(id, name, description);
            return Ok(newRecipe);
        }

        [HttpGet("{id}")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _recipeService.GetRecipe(id);
            return Ok(recipe);
        }

        [HttpPut("{id}/update")]
        public IActionResult UpdateRecipe(int id, string? name, string? description)
        {
            _recipeService.UpdateRecipe(id, name, description); //Возвращать новый рецепт
            return Ok();
        }

        [HttpDelete("{id}/delete")]
        public IActionResult DeleteRecipe(int id)
        {
            _recipeService.DeleteRecipe(id);
            return Ok();
        }

        [HttpPut("{id}/rate")]
        public IActionResult RateRecipe(int id, ResipeRating rating)
        {
            _recipeService.RateTheRecipe(id, rating);
            return Ok();
        }
        [HttpPost("/newingredient")]
        public IActionResult AddIngredient(int id, string name, double amount, Units units)
        {
            _recipeService.AddIngredient(id, name, amount, units);
            return Ok();
        } 
    }
}
