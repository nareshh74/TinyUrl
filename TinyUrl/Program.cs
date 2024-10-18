using System.Text.Json;
using TinyUrl;
using TinyUrl.Logic;
using TinyUrl.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddSingleton<IUrlRepository, InMemoryRepository>();
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddSingleton<IUrlConverter, UrlConverter>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

// to test using web application factory
public partial class Program { }