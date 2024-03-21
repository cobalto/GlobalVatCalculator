
using GlobalVatCalculator.Api.Extensions;

namespace GlobalVatCalculator.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureDefaultServices();
            builder.Services.ConfigureSwaggerGen(builder.Configuration);
            builder.Services.ConfigureCustomServices();

            var app = builder.Build();

            app.ConfigureAppBuilder();

            if (app.Environment.IsDevelopment())
            {
                app.ConfigureSwagger();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
