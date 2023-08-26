using DeskDashboard.Domain;
using Forex;

namespace DeskDashboard.Services;

public class SpotService : ISpotService
{
    private readonly ILogger<SpotService> _logger;
    private readonly ISpot _spot;

    public SpotService(ILogger<SpotService> logger, ISpot spot)
    {
        _logger = logger;
        _spot = spot;
    }
    public bool AddSubscription(Domain.Spot spot)
    {
        bool response = true;
        try
        {
            _spot.AddTickDefinition(new TickDefinition(spot.CurrencyPair, spot.Bid, spot.Ask, spot.Spread, spot.PublishFrequencyInMs));
            Start();
            var resp = _spot.GetScheduledTicks().First(tick => tick.ccyPair == spot.CurrencyPair && tick.frequency == spot.PublishFrequencyInMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding subscription");
            response = false;
        }
        return response;
    }

    public void Start()
    {
        _spot.Stop();
        _spot.Start();
    }
}

