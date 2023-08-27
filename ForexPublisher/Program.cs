using ForexPublisher.Hubs;
using ForexPublisher.Services;
using Forex;
using Microsoft.AspNetCore.Mvc;

namespace ForexPublisher;

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
        builder.Services.AddSingleton<ForexPublisher.DataProducers.Forex>();

        builder.Services.AddScoped<ISpotService, SpotService>();

        var app = builder.Build();

        app.UseCors("ForexCorsPolicy");

        app.MapHub<ForexHub>("/forexdata");

        app.MapGet("/", () => "Hello World!");

        var _ = app.Services.GetService<ForexPublisher.DataProducers.Forex>();

        app.MapPost("/api/v1/addsubscription", (ILogger<Program> _logger, ISpotService spotService, [FromBody] Domain.Spot spot ) =>
        {
            if (spotService.AddSubscription(spot))
            {
                return Results.Ok(Results.Empty);
            }
            else
            {
                return Results.BadRequest(Results.Empty);
            }
        });

        app.MapPost("/api/v1/stop", (ILogger<Program> _logger, ISpotService spotService) =>
        {
            spotService.Stop();
            return Results.Ok(Results.Empty);
        });

        app.MapPost("/api/v1/pause", (ILogger<Program> _logger, ISpotService spotService) =>
        {
            _logger.LogInformation(".....Pausing");
            spotService.Pause();
            return Results.Ok(Results.Empty);
        });

        app.MapPost("/api/v1/resume", (ILogger<Program> _logger, ISpotService spotService) =>
        {
            _logger.LogInformation("Resuming.....");
            spotService.Resume();
            return Results.Ok(Results.Empty);
        });

        app.Run();
    }
}