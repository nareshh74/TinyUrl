using System.Text.Json;

namespace TinyUrl.Controllers
{
    public static class ServiceExtensions
    {
        internal static void AddApiDependencies(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
