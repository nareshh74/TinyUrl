using Microsoft.EntityFrameworkCore;

namespace TinyUrl.Repository
{
    public static class ServiceExtensions
    {
        internal static void AddDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUrlRepository, SqlRepository>();
        }
    }
}
