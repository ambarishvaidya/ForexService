using DeskDashboard.Endpoints;
using DeskDashboard.Hubs;
using DeskDashboard.Services;
using Forex;
using Microsoft.AspNetCore.Mvc;

namespace DeskDashboard;

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

        builder.Services.AddScoped<ISpotService, SpotService>();
                
        var app = builder.Build();

        app.UseCors("ForexCorsPolicy");

        app.MapHub<ForexHub>("/forexdata");
        
        app.MapGet("/", () => "Hello World!");

        var _ = app.Services.GetService<DeskDashboard.DataProducers.Forex>();

        app.MapPost("/api/v1/addsubscription", async(Domain.Spot spot, ISpotService spotService) =>
        {
            if(spotService.AddSubscription(spot))
            {
                return await Task.FromResult<IActionResult>(new OkResult());
            }
            else
            {
                return await Task.FromResult<IActionResult>(new BadRequestResult());
            }
        });

        app.MapPost("/api/v1/stop", async (ISpotService spotService) =>
        {
            spotService.Stop();
            return await Task.FromResult<IActionResult>(new OkResult());
        });

        app.MapPost("/api/v1/pause", async (ISpotService spotService) =>
        {
            spotService.Pause();
            return await Task.FromResult<IActionResult>(new OkResult());
        });

        app.MapPost("/api/v1/resume", async (ISpotService spotService) =>
        {
            spotService.Resume();
            return await Task.FromResult<IActionResult>(new OkResult());
        });

        app.Run();
    }
}