using Cookbook_1.Models;

namespace Cookbook_1.TempStorage
{
    public class IngredientStorage
    {
        public readonly List<Ingredient> AllSavedIngridients = [
            new Ingredient
            {
                Id = 1,
                Name = "Соль",
                Amount = 1,
                Units = ENums.Units.SmallSpoon
            },
            new Ingredient 
            {
                Id = 2,
                Name = "Сахар",
                Amount = 1,
                Units = ENums.Units.BigSpoon
            }
            ];

        public void AddNewIngridient(Ingredient ingridient)
        {
            AllSavedIngridients.Add(ingridient);
        }
        public List<Ingredient> GetIngredients()
        {
            return AllSavedIngridients;
        }
    }
}
