using Forex;
using ForexPublisher.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ForexPublisher.DataProducers;

public class Forex
{
    private readonly ILogger<Forex> _logger;
    private readonly IHubContext<ForexHub> _hub;
    private readonly ISpot _spot;

    public Forex(ILogger<Forex> logger, IHubContext<ForexHub> hub, ISpot spot)
    {
        _logger = logger;
        _hub = hub;
        _spot = spot;

        _logger.LogInformation("Forex Data Producer registered for callback.");
        _spot.OnTickUpdate += OnNewTickReceived;

        _spot.AddTickDefinition(new TickDefinition("EURUSD", 1.2000, 1.2001, 0.001, 1000));
        _spot.AddTickDefinition(new TickDefinition("EURGBP", 0.8500, 0.8501, 0.001, 1500));
        _spot.AddTickDefinition(new TickDefinition("EURJPY", 130.00, 130.01, 0.01, 1750));
        _spot.AddTickDefinition(new TickDefinition("EURCHF", 1.1000, 1.1001, 0.001, 800));
        _spot.AddTickDefinition(new TickDefinition("EURCAD", 1.5000, 1.5001, 0.001, 1200));
        _spot.AddTickDefinition(new TickDefinition("EURAUD", 1.6000, 1.6001, 0.001, 2000));
        _spot.AddTickDefinition(new TickDefinition("EURSEK", 10.000, 10.001, 0.001, 900));
        _spot.Start();
    }

    /// <summary>
    /// Callback from the market data producer. Adding the tick to a Queue for 
    /// processing. This way we do not block thread receiving the tick.
    /// </summary>
    /// <param name="tickData">Updated Tick</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal async Task OnNewTickReceived(string tickData)
    {
        try
        {
            TickerRecord record = DeserializeToRecord(tickData);
            var json = JsonSerializer.Serialize(record);
            await _hub.Clients.All.SendAsync("ForexTick", json);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error while processing tick data. {tickData}");
        }
    }

    /// <summary>
    /// Works only for formet CCY:Bid,Ask,Last.
    /// </summary>
    /// <param name="tickData"></param>
    /// <returns></returns>
    internal TickerRecord DeserializeToRecord(string tickData)
    {
        ReadOnlySpan<char> tickDataSpan = tickData.AsSpan();
        int delimiter = tickDataSpan.IndexOf(':');
        var ccyPair = tickDataSpan.Slice(0, delimiter).ToString().Trim();

        tickDataSpan = tickDataSpan.Slice(delimiter + 1);
        delimiter = tickDataSpan.IndexOf(',');
        double bid = double.Parse(tickDataSpan.Slice(0, delimiter));

        tickDataSpan = tickDataSpan.Slice(delimiter + 1);
        delimiter = tickDataSpan.IndexOf(',');
        double ask = double.Parse(tickDataSpan.Slice(0, delimiter));

        tickDataSpan = tickDataSpan.Slice(delimiter + 1);
        delimiter = tickDataSpan.IndexOf(',');
        double last = 0.0d;
        if (delimiter < 0)
            last = double.Parse(tickDataSpan);
        else
            last = double.Parse(tickDataSpan.Slice(0, delimiter));

        return new TickerRecord(ccyPair, bid, ask, last);
    }
}

internal record struct TickerRecord(string Ticker, double Bid, double Ask, double Last);
