using Cookbook_1.Abstractions;
using Cookbook_1.Services;
using Cookbook_1.TempStorage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IngredientStorage>();
builder.Services.AddSingleton<IRecipeService, RecipeService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler("/error");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
