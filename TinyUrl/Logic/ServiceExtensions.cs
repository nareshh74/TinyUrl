namespace TinyUrl.Logic
{
    public static class ServiceExtensions
    {
        internal static void AddBusinessLogicDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUrlConverter, UrlConverter>();
        }
    }
}
