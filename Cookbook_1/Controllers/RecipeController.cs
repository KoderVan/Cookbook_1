using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Cookbook_1.Extentions;
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
            var userId = HttpContext.ExtractUserIdFromClaims();

            if (userId is null)
                return Unauthorized();

            var newRecipe = _recipeService.CreateRecipe(userId.Value, dto);
            return Ok(newRecipe);
            
        }

        [AllowAnonymous]
        [HttpGet("/recipe/{id}")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _recipeService.GetRecipe(id);
            return Ok(recipe);
        }

        [Authorize(Policy = "PostOwner")]
        [HttpPut("/{id}/update")]
        public IActionResult UpdateRecipe(UpdateRecipeDto dto)
        {
            _recipeService.UpdateRecipe(dto); 
            return Ok();
        }

        [Authorize (Policy = "PostOwner")]
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
            var userId = HttpContext.ExtractUserIdFromClaims();

            if (userId is null)
                return Unauthorized();

            _recipeService.RateTheRecipe(id, userId.Value, rating);
            return Ok();
        }

        [Authorize]
        [HttpPost("/newingredient")]
        public IActionResult AddIngredient(string name)
        {
            _ingredientService.AddIngredient(name);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("/user{userId}/allrecipes")]
        public IActionResult GetUsersRecipes(int userId)
        {
            var usersRecipes = _recipeService.GetFilteredRecipesListByUser(userId);
            return Ok(usersRecipes);
        }

        [AllowAnonymous]
        [HttpGet("/filtered")]
        public IActionResult GetFilteredRecipeList(double? minRating, int userId, string? recipeName, RecipeFilterBy filterType, bool? ascending)
        {
            var filteredRecipeList = _recipeService.GetFilteredRecipeList(minRating, userId, recipeName, filterType, ascending);
            return Ok(filteredRecipeList);
        }
    }
}
