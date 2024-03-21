using GlobalVatCalculator.Api.Interfaces;
using GlobalVatCalculator.Api.Services;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace GlobalVatCalculator.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class StartupConfigExtensions
    {
        public static IServiceCollection ConfigureDefaultServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection ConfigureSwaggerGen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(configuration["AppDetails:Version"], new OpenApiInfo
                {
                    Title = configuration["AppDetails:Title"],
                    Version = configuration["AppDetails:Version"],
                    Description = configuration["AppDetails:Description"],
                    Contact = new OpenApiContact
                    {
                        Name = configuration["AppDetails:AuthorName"],
                        Url = new Uri(configuration["AppDetails:AuthorUrl"] ?? string.Empty)
                    }
                });
            });

            return services;
        }

        public static IServiceCollection ConfigureCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ITaxService, TaxService>();

            return services;
        }

        public static IApplicationBuilder ConfigureAppBuilder(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            return app;
        }

        public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}