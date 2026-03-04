using Cookbook_1.ENums;
using Cookbook_1.Models;

namespace Cookbook_1.Abstractions
{
    public interface IRecipeService
    {
        public Recipe CreateRecipe(int id, string name, string description);

        public Recipe GetRecipe(int id);

        public void UpdateRecipe(int id, string? newName, string? newDescription);

        public void DeleteRecipe(int id);

        public List<Recipe> GetAllRecipes();

        public void RateTheRecipe(int id, ResipeRating rating);

        public void AddIngredient(int id, string name, double amount, Units units); //Это нажо будет потом убрать

    }
}
