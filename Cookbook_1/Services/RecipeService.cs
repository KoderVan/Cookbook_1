using AutoMapper;
using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;



namespace Cookbook_1.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _applicationDbContext;

        public RecipeService(IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
        }
        public List<RecipeVm> ReturnRecipeListVm()
        {

            var recipeListVm = _mapper.Map<List<RecipeVm>>(_applicationDbContext.Recipes.ToList());
            return recipeListVm;
        }


        public RecipeVm CreateRecipe(CreateRecipeDto dto)
        {
            var newRecipe = _mapper.Map<Recipe>(dto);

            var recipeExists = _applicationDbContext.Recipes.AsNoTracking().FirstOrDefault(recipe => recipe.Name == newRecipe.Name);
            if (recipeExists != null)
            {
                throw new RecipeAlreadyExistsException(newRecipe.Name);
            }

            var existingIngredients = GetExistingIngredients(newRecipe.RequiredIngredients);
            newRecipe.Name = newRecipe.Name.Trim();

            foreach (var ingredientInRecipe in newRecipe.RequiredIngredients)
            {
                ingredientInRecipe.Recipe = newRecipe;
                ingredientInRecipe.Ingredient = existingIngredients[ingredientInRecipe.IngredientId];
            }

            _applicationDbContext.Recipes.Add(newRecipe);
            _applicationDbContext.SaveChanges();
            var newRecipeVm = _mapper.Map<RecipeVm>(newRecipe);
            return newRecipeVm;
        }

        public RecipeVm GetRecipe(int id)
        {
            var recipe = GetRecipeWithIngredientsOrThrowException(id);
            var recipeVm = _mapper.Map<RecipeVm>(recipe); 

            return recipeVm;
        }

        public RecipeVm UpdateRecipe(UpdateRecipeDto dto)
        {
            var recipeToUpdate = GetRecipeWithIngredientsOrThrowException(dto.Id);
            recipeToUpdate.Name = dto.NewName ?? recipeToUpdate.Name;
            recipeToUpdate.CookingDescription = dto.NewDescription ?? recipeToUpdate.CookingDescription;

            if (dto.NewIngredients != null)
            {
                foreach (var ingredient in dto.NewIngredients)
                {
                    var IngredientAlreadyInRecipe = recipeToUpdate.RequiredIngredients.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);
                    if (IngredientAlreadyInRecipe != null)
                    {
                        throw new IngredientAlreadyExistsException(IngredientAlreadyInRecipe.Ingredient.Name); //Тут он же продолжит работу и пойдёт дальше по списку, да?
                    }
                    var newIngredientMap = _mapper.Map<IngredientInRecipe>(ingredient);
                    recipeToUpdate.RequiredIngredients.Add(newIngredientMap);
                }
            }

            if (dto.EditIngredients != null)
            {
                foreach (var ingredient in dto.EditIngredients)
                {
                    //Было бы круто иметь  dto без recipeId, и присваивать этот ID беря его из рецепта
                    var ingredientToUpdate = recipeToUpdate.RequiredIngredients.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId)
                        ?? throw new IngredientNotFoundException(ingredient.IngredientId);
                    ingredientToUpdate.Amount = ingredient.Amount ?? ingredientToUpdate.Amount;
                    ingredientToUpdate.Units = ingredient.Units ?? ingredientToUpdate.Units;
                }
            }
            _applicationDbContext.SaveChanges();
            var recipeVm = _mapper.Map<RecipeVm>(recipeToUpdate);
            return recipeVm;
        }

        public void DeleteRecipe(int id)
        {
            var recipe = _applicationDbContext.Recipes.FirstOrDefault(recipe => recipe.Id == id) ?? throw new RecipeNotFoundException(id);
            _applicationDbContext.Recipes.Remove(recipe); 
            _applicationDbContext.SaveChanges();
        }

        public void RateTheRecipe(int id, RecipeRating rating)
        {
            var recipe = GetRecipeWithIngredientsOrThrowException(id); // Тут можно просто рецепт без ингредиентов, но есть ли смысл?   
            double ratingValue = (double)rating;
            recipe.ListOfRatings.Add(ratingValue);
            recipe.Rating = recipe.ListOfRatings.Average();
            _applicationDbContext.SaveChanges();
        }

        private Dictionary<int, Ingredient> GetExistingIngredients(List<IngredientInRecipe> ingredients)
        {
            List<int> IngredientsIds = [.. ingredients.Select(i => i.IngredientId)];
            var existingIngredients = _applicationDbContext.Ingredients.Where(i => IngredientsIds.Contains(i.Id)).ToDictionary(i => i.Id);
            return existingIngredients;
        }

        private Recipe GetRecipeWithIngredientsOrThrowException(int recipeId)
        {
            var recipe = _applicationDbContext.Recipes.Where(r => r.Id == recipeId)
                .Include(i => i.RequiredIngredients)
                .ThenInclude(i => i.Ingredient).FirstOrDefault() ?? throw new RecipeNotFoundException(recipeId);
            return recipe;
        }
    }
}