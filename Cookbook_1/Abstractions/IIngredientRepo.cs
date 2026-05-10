using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Cookbook_1.Models;

namespace Cookbook_1.Abstractions
{
    public interface IIngredientRepo
    {
        public void AddNewIngridient(string name);

        public List<Ingredient> GetAllIngredients();

        List<Ingredient> GetRecipeIngredients(int recipeId);

        public Ingredient GetIngredientById(int id);

        public List<Ingredient> GetIngredientsForRecipeById(List<int> ingredientsId);

        //public List<IngredientInRecipe> SetIngredientsForRecipe(int recipeId, List<Ingredient> listOfingrediens);

        public void AddIngredientToRecipe(AddIngredientToRecipeDto dto);

        public List<Ingredient> ShowAllIngredientsForTest();

        public List<IngredientInRecipe> ShowAllIngredientsInRecipeForTest();
    }
}
