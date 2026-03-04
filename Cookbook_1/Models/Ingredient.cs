using Cookbook_1.ENums;

namespace Cookbook_1.Models
{
    public class Ingredient //Чёрт, мне это не нравится. Надо как то отделить кол-во и юниты от модели.
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public Units Units { get; set; } 
    }
}
