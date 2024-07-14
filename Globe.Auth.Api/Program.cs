using Globe.Auth.Api.Extensions;
using Globe.Auth.Api.Middlewares;
using Serilog;

namespace Globe.Auth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

            builder.Host.UseSerilog();

            builder.ConfigureServices();

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Globe API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthorization();

            //app.UseMiddleware<CustomAuthMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}