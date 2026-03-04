namespace Cookbook_1.Models
{
    public class Recipe // потом добавить пользователя
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string CookingDescription { get; set; }
        public List<Ingredient> RequieredIngredients { get; set; } // Наверно надо будт создавать свой список ингредиентов для каждого рецепта и добавлять их туда
        public double Rating { get; set; }
        public List<double> ListOfRatings { get; set; } = []; //наверно, при создании бд это пропадёт
    }
}
