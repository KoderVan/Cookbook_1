using Cookbook_1.Abstractions;
using Cookbook_1.ENums;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Cookbook_1.TempStorage;

namespace Cookbook_1.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly List<Recipe> _recipeList = [];
        public static IngredientStorage Storage = new();
        public List<Ingredient> Ingredients = Storage.GetIngredients();
        public Recipe CreateRecipe(int id, string name, string description) //Добавить потом обработку ошибки ингредиента. Пока не хочу допом регать хранилище игредиентов чтобы с ним рабоать
        {
            var RecipeExists = _recipeList.Where(recipe => recipe.Id == id).FirstOrDefault();
            if (RecipeExists != null)
            {
                throw new RecipeAlreadyExistsException(id);
            }
            var Recipe = new Recipe
            {
                Id = id,
                Name = name,
                CookingDescription = description,
                RequieredIngredients = Ingredients,
            };
            _recipeList.Add(Recipe);
            return Recipe;
        }

        public Recipe GetRecipe(int id) 
        {
            var recipe = _recipeList.Where(recipe => recipe.Id == id).FirstOrDefault();
            if (recipe == null)
            {
                throw new RecipeNotFoundException(id);
            }
            return recipe;
            
        }

        public void UpdateRecipe(int id, string? newName, string? newDescription)
        {
            var recipe = GetRecipe(id);
            recipe.Name = newName ?? recipe.Name; 
            recipe.CookingDescription = newDescription ?? recipe.CookingDescription; //Надо бы запомнить эту констркцию, выглядит круто и по умному

        }
        public void DeleteRecipe(int id)
        {
            var recipe = GetRecipe(id);
            _recipeList.Remove(recipe);
            
        }

        public List<Recipe> GetAllRecipes()
        {
            return _recipeList;
        }

        public void RateTheRecipe(int id, ResipeRating rating)
        {
            var recipe = GetRecipe(id);
            double ratingValue = (double)rating; 
            recipe.ListOfRatings.Add(ratingValue);
            recipe.Rating = recipe.ListOfRatings.Sum() / recipe.ListOfRatings.Count();
        }

        public void AddIngredient(int id, string name, double amount, Units units) // В будущем буду автоматом формировать id (если вообще он нужен)
        {
            //IDEговорит, лучше использовать string.Equals, не понял, как юзать это с Linq
            var checkIfIngredientExists = Ingredients.Where(ingredient => ingredient.Name.ToLower() == name.ToLower()).FirstOrDefault(); //Как будто правильнее так, а не по id
            if (checkIfIngredientExists != null)
            {
                throw new IngredientAlreadyExistsException(name);
            }
            var newIngredient = new Ingredient
            {
                Id = id,
                Name = name,
                Amount = amount,
                Units = units // units бы в стринге показывать, а не числом
            };
            Ingredients.Add(newIngredient); 
        }
    }
}
