using Cookbook_1.Contracts;
using Cookbook_1.ENums;
using System.Linq.Expressions;

namespace Cookbook_1.Abstractions
{
    public interface IRecipeService
    {
        public RecipeVm CreateRecipe(int userId, CreateRecipeDto dto);

        public RecipeVm GetRecipe(int id);

        public RecipeVm UpdateRecipe(UpdateRecipeDto dto);

        public void DeleteRecipe(int id);

        public void RateTheRecipe(int id, int userId, RecipeRating rating);

        public List<RecipeVm> GetFilteredRecipesListByUser(int userId);

        public List<RecipeVm> GetFilteredRecipeList(double? minRating, int? userId, string? name, RecipeFilterBy filterType, bool? ascending);
    }
}
