using TinyUrl.Controllers;
using TinyUrl.Logic;
using TinyUrl.Repository;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

var services = builder.Services;
services.AddDataDependencies(configuration);
services.AddApiDependencies();
services.AddBusinessLogicDependencies();
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