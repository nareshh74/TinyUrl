using Serilog;
using TinyUrl.Controllers;
using TinyUrl.Logic;
using TinyUrl.Repository;

try
{

    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .WriteTo.File("logs.txt")
        .CreateLogger();
    

    // Add services to the container.

    var services = builder.Services;
    services.AddSerilog();
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

    app.UseSerilogRequestLogging();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    return 0;
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
    return 1;

}
finally
{
    await Log.CloseAndFlushAsync();
}

// to test using web application factory
public partial class Program { }