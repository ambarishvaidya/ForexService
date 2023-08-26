using DeskDashboard.Hubs;
using Forex;

namespace DeskDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);            

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ForexCorsPolicy",
                                       builder =>
                                       {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            builder.Logging.ClearProviders().AddSimpleConsole();
            builder.Services.AddSignalR();

            builder.Services.AddSingleton<ISpot, Spot>();
            builder.Services.AddSingleton<DeskDashboard.DataProducers.Forex>();

            var app = builder.Build();

            app.UseCors("ForexCorsPolicy");

            app.MapHub<ForexHub>("/forexdata");
            app.MapGet("/", () => "Hello World!");

            var _ = app.Services.GetService<DeskDashboard.DataProducers.Forex>();

            app.Run();
        }
    }
}