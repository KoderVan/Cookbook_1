using Cookbook_1;





var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddSwaggerGen();   




var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler("/error");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
