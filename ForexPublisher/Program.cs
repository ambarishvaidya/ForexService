using FluentValidation;
using Forex;
using ForexPublisher.Hubs;
using ForexPublisher.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForexPublisher;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adding CORS policy to allow the specigfic client to connect to the server.
        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("ForexCorsPolicy",
        //                           builder =>
        //                           {
        //                               builder.WithOrigins("http://localhost:3000")
        //                                   .AllowAnyHeader()
        //                                   .AllowAnyMethod()
        //                                   .AllowCredentials();
        //                           });
        //});

        builder.Services.AddCors();

        builder.Logging.ClearProviders().AddSimpleConsole();
        builder.Services.AddSignalR();

        builder.Services.AddSingleton<ISpot, Spot>();
        builder.Services.AddSingleton<ForexPublisher.DataProducers.Forex>();

        builder.Services.AddScoped<ISpotService, SpotService>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var app = builder.Build();

        //app.UseCors("ForexCorsPolicy");
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(origin => true));

        app.MapHub<ForexHub>("/ForexService");

        var _ = app.Services.GetService<ForexPublisher.DataProducers.Forex>();

        app.MapPost("/api/v1/addsubscription", async (ILogger<Program> _logger, IValidator<Domain.Spot> _spotValidator, ISpotService spotService, [FromBody] Domain.Spot spot) =>
        {
            var result = await _spotValidator.ValidateAsync(spot);
            if (result.IsValid && spotService.AddSubscription(spot))
            {
                return Results.Ok(Results.Empty);
            }
            else
            {
                return Results.BadRequest(!result.IsValid ? string.Join(Environment.NewLine, result.Errors) : Results.Empty);
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

        app.MapPost("/api/v1/start", (ILogger<Program> _logger, ISpotService spotService) =>
        {   
            _logger.LogInformation("Starting...");
            spotService.Start();
            return Results.Ok(Results.Empty);
        });

        app.Run();
    }
}