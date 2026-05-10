using AutoMapper;
using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook_1.Repositories
{
    public class RecipeRepo : IRecipeRepo
    {
        public List<Recipe> recipeList = [];
        private readonly IIngredientRepo _ingredientRepo;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _applicationDbContext;

        public RecipeRepo(IIngredientRepo ingredientRepo, IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _ingredientRepo = ingredientRepo;
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
        }
        public List<RecipeVm> ReturnRecipeListVm()
        {
           
            var recipeListVm = _mapper.Map<List<RecipeVm>>(_applicationDbContext.Recipes.ToList());
            return recipeListVm;
        }
        

        public Recipe CreateRecipe(CreateRecipeDto dto)
        {
            var newRecipe = _mapper.Map<Recipe>(dto);

            var recipeExists = _applicationDbContext.Recipes.AsNoTracking().FirstOrDefault(recipe => recipe.Name == newRecipe.Name);
            if (recipeExists != null)
            {
                throw new RecipeAlreadyExistsException(newRecipe.Name);
            }

            newRecipe.Name = newRecipe.Name.Trim();
            
            //Этим, возможно, стоит заниматься в IngredientService
            //foreach (var ingredient in newRecipe.RequiredIngredients)
            //
                
            //    var ingredientExists = _applicationDbContext.Ingredients.AsNoTracking().FirstOrDefault(i => i.Id == ingredient.IngredientId);

            //    var newIngredientInRecipeDto = new AddIngredientToRecipeDto
            //        (
            //            newRecipe.Id,
            //            ingredient.IngredientId,
            //            ingredient.Amount,
            //            ingredient.Units
            //        );

            //    var newIngredientInRecipe = _mapper.Map<IngredientInRecipe>(newIngredientInRecipeDto);
            //    newRecipe.RequiredIngredients.Add(newIngredientInRecipe);
                //_applicationDbContext.IngredientsInRecipes.Add(newIngredientInRecipe);
                
                //if (ingredientExists == null)
                //{
                //    var simpleIngredient = new Ingredient
                //    {
                //        Name = ingredient.IngredientName
                //    };

                //    _applicationDbContext.Ingredients.Add(simpleIngredient);
                //    
                //}
                //else
                //{
                //    _applicationDbContext.IngredientsInRecipes.Add(ingredient);                   
                //}

            //}
            _applicationDbContext.Recipes.Add(newRecipe);
            _applicationDbContext.SaveChanges();
            return newRecipe;
        }

        public RecipeVm GetRecipe(int id)
        {
            var recipe = _applicationDbContext.Recipes.FirstOrDefault(recipe => recipe.Id == id) ?? throw new RecipeNotFoundException(id);
            var recipeVm = _mapper.Map<RecipeVm>(recipe);
            return recipeVm;
        }

        public RecipeVm UpdateRecipe(UpdateRecipeDto dto)
        {
            var recipeToUpdate = recipeList.Where(recipe => recipe.Id == dto.Id).FirstOrDefault() ?? throw new RecipeNotFoundException(dto.Id);
            recipeToUpdate.Name = dto.NewName ?? recipeToUpdate.Name;
            recipeToUpdate.CookingDescription = dto.NewDescription ?? recipeToUpdate.CookingDescription;

            if (dto.NewIngredients != null)
            {
                foreach(var ingredient in dto.NewIngredients)
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

            if(dto.EditIngredients != null)
            {
                foreach(var ingredient in dto.EditIngredients)
                {
                    //Было бы круто иметь  dto без recipeId, и присваивать этот ID беря его из рецепта
                    var ingredientToUpdate = recipeToUpdate.RequiredIngredients.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId) 
                        ?? throw new IngredientNotFoundException(ingredient.IngredientId);
                    ingredientToUpdate.Ingredient.Name = ingredient.NewName ?? ingredientToUpdate.Ingredient.Name;
                    ingredientToUpdate.Amount = ingredient.Amount ?? ingredientToUpdate.Amount;
                    ingredientToUpdate.Units = ingredient.Units ?? ingredientToUpdate.Units;
                }
            }
            var recipeVm = _mapper.Map<RecipeVm>(recipeToUpdate);
            return recipeVm;
        }

        public void DeleteRecipe(int id)
        {
            var recipe = recipeList.Where(recipe => recipe.Id == id).FirstOrDefault() ?? throw new RecipeNotFoundException(id);
            recipeList.Remove(recipe);
        }

        public void RateTheRecipe(int id, RecipeRating rating)
        {
            var recipe = recipeList.Where(recipe => recipe.Id == id).FirstOrDefault() ?? throw new RecipeNotFoundException(id);
            double ratingValue = (double)rating;
            recipe.ListOfRatings.Add(ratingValue);
            recipe.Rating = recipe.ListOfRatings.Sum() / recipe.ListOfRatings.Count();
        }
    }
}
