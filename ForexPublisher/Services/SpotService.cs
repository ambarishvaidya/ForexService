using Forex;

namespace ForexPublisher.Services;

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
            var resp = _spot.GetScheduledTicks().First(tick => tick.ccyPair == spot.CurrencyPair);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding subscription");
            response = false;
        }
        return response;
    }

    public void Pause()
    {
        _spot.Pause();
    }

    public void Resume()
    {
        _spot.Resume();
    }

    public void Start()
    {
        _spot.Stop();
        _spot.Start();
    }

    public void Stop()
    {
        _spot.Stop();
    }
}

