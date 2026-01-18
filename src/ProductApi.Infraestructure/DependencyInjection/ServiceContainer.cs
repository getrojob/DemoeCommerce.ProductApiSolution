using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infraestructure.Data;
using ProductApi.Infraestructure.Repositories;

namespace ProductApi.Infraestructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create DependencyInjection (DI)
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: handle external errors
            // Listen to Only Api Gateway: blocks all outsider calls
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
