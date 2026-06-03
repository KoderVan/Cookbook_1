using Cookbook_1.Abstractions;
using Cookbook_1.Database;
using Cookbook_1.Services;
using Microsoft.EntityFrameworkCore;


namespace Cookbook_1
{
    public static class Composer
    {
        //Тут добавляются инфраструктурные сервисы
        //которые нужны для передачи данных внутри приложения
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);
            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Добавляем автомаппер и регистрируем в нем все классы из
            //нашего проекта, которые мы наследовали от Profile
            services.AddAutoMapper(typeof(Program));

            //Подключение БД
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                //Указываю, что использую PostgreSql
                //Указываю ссылку на строку подключения из secrets.json, чтобы пароль не передать
                options.UseNpgsql(
                    configuration.GetConnectionString("ConnectionString")
                    );
            });


            services.AddScoped<IIngredientService, IngredientService>();
            
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddExceptionHandler<ExceptionHandler>();
            services.AddControllers();
            return services;
        }
    }
}
