namespace Cookbook_1.Exceptions
{
    public class RecipeAlreadyExistsException : Exception
    {
        public RecipeAlreadyExistsException(int id) // наверно, тут в идеале работать не с id а названием рецепта.
            : base($" Recipe with ID {id} already exists") // а можно как то вывести этот рецепт после обработки ошибки?
        {

        }
    }
}
