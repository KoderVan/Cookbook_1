using AutoMapper;
using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;



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


        public RecipeVm CreateRecipe(int userId, CreateRecipeDto dto)
        {
            var newRecipe = _mapper.Map<Recipe>(dto);
            newRecipe.UserId = userId;

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

        public void RateTheRecipe(int id, int userId, RecipeRating rating)
        {
            var recipe = GetRecipeWithIngredientsOrThrowException(id);
            var existingRating = _applicationDbContext.Ratings.FirstOrDefault(r => r.RatedRecipeId == id && r.RatedUserId == userId);
            if (existingRating is not null)
                throw new InvalidOperationException("Пользователь уже оценил этот рецепт");

            Rating ratingValue = new Rating
            {
                Value = rating,
                RatedUserId = userId,
                RatedRecipeId = id

            };

            recipe.ListOfRatings.Add(ratingValue);
            _applicationDbContext.Ratings.Add(ratingValue);
            
           _applicationDbContext.SaveChanges();

            var ratings = _applicationDbContext.Ratings.Where(rating => rating.RatedRecipeId == id);
            recipe.Rating = ratings.Average(r => (int)r.Value);
            _applicationDbContext.SaveChanges();

        }

        private Dictionary<int, Ingredient> GetExistingIngredients(List<IngredientInRecipe> ingredients)
        {
            List<int> IngredientsIds = [.. ingredients.Select(i => i.IngredientId)]; //что за две точки?
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

        public List<RecipeVm> GetFilteredRecipesListByUser(int userId)
        {
            var filteredRecipes = _applicationDbContext.Recipes.Where(u => u.UserId == userId)
                .Include(i => i.RequiredIngredients)
                .ThenInclude(i => i.Ingredient).ToList();

            if (!filteredRecipes.Any())
            {
                throw new UserNotFoundException(userId);
            }

            var recipeListVm = _mapper.Map<List<RecipeVm>>(filteredRecipes);
            return recipeListVm;
        }

        public List<RecipeVm> GetFilteredRecipeList(double? minRating, int? userId, string? name, RecipeFilterBy filterType, bool? ascending)
        {

            List<RecipeVm> recipeListVm = new List<RecipeVm>();

            switch (filterType)
            {
                case RecipeFilterBy.Title:
                    recipeListVm = Filter(r => r.Name == name);
                    if (!recipeListVm.Any())
                        throw new FilterException("Рецептов с таким названием нет!");
                    return recipeListVm;
                case RecipeFilterBy.Rating:
                    recipeListVm = Filter(r => r.Rating >=  minRating);
                    if (!recipeListVm.Any())
                        throw new FilterException("Рецептов с таким рейтингом нет!");
                    return recipeListVm;
                case RecipeFilterBy.User:
                    recipeListVm = Filter(r => r.UserId == userId);
                    if (!recipeListVm.Any())
                        throw new FilterException("неверный id пользователя");
                    return recipeListVm;
                default:
                    throw new FilterException("Неверные настройки фильтра");
                    
            }
        }

        private List<RecipeVm> Filter(Expression<Func<Recipe, bool>> expression)
        {
            var filteredRecipes = _applicationDbContext.Recipes.Where(expression)
                .Include(i => i.RequiredIngredients)
                .ThenInclude(i => i.Ingredient).ToList();

            var recipeListVm = _mapper.Map<List<RecipeVm>>(filteredRecipes);
            return recipeListVm;
        }
    }
}