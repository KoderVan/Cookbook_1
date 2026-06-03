using AutoMapper;
using Cookbook_1.Abstractions;
using Cookbook_1.Contracts;
using Cookbook_1.Exceptions;
using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook_1.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IApplicationDbContext _applicationDbContext; 
        private readonly IMapper _mapper;
        public IngredientService(IApplicationDbContext applicationDbContext, IMapper mapper) 
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public Ingredient GetIngredientById(int ingredientId)
        {
            var ingredient = _applicationDbContext.Ingredients.AsNoTracking().FirstOrDefault(ingredient => ingredient.Id == ingredientId)
                ?? throw new IngredientNotFoundException(ingredientId);
            return ingredient;
        }

        public void AddIngredient(string name)
        {
            var checkIfIngredientExists = _applicationDbContext.Ingredients.AsNoTracking().FirstOrDefault(ingredient => ingredient.Name.ToLower() ==  name.ToLower().Trim());
            if (checkIfIngredientExists != null)
            {
                throw new IngredientAlreadyExistsException(name); // Нет констистентности, другая ошибка возвращает int id
            }
            Ingredient ingredient = new()
            {
                Name = name,
            };
            _applicationDbContext.Ingredients.Add(ingredient);
            _applicationDbContext.SaveChanges();
        }

        public void AddIngredientToRecipe(AddIngredientToRecipeDto dto)
        {
            var ingredient = _applicationDbContext.Ingredients.AsNoTracking().FirstOrDefault(ingredient => ingredient.Id == dto.IngredientId)
                ?? throw new IngredientNotFoundException(dto.IngredientId);

            var newIngredientInRecipe = _mapper.Map<IngredientInRecipe>(dto);
            newIngredientInRecipe.Ingredient = ingredient;
            _applicationDbContext.IngredientsInRecipes.Add(newIngredientInRecipe);
            _applicationDbContext.SaveChanges();
        }
        
        public List<Ingredient> ShowAllIngredientsForTest()
        {
            var AllIngredients = _applicationDbContext.Ingredients.ToList();
            return AllIngredients;
        }
        public List<IngredientInRecipe> ShowAllIngredientsInRecipeForTest()
        {
            var AllIngredientsInRecipes = _applicationDbContext.IngredientsInRecipes.ToList();
            return AllIngredientsInRecipes;
        }
    }
}
