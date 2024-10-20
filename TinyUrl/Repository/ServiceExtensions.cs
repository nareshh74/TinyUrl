using Microsoft.EntityFrameworkCore;

namespace TinyUrl.Repository
{
    public static class ServiceExtensions
    {
        internal static void AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:Sql"]);
            });

            services.AddScoped<IUrlRepository, SqlRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["ConnectionStrings:Redis"];
            });
        }
    }
}
