using ForexPublisher.Hubs;
using Forex;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace ForexPublisher.DataProducers;

[EnableCors("ForexCorsPolicy")]
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
                
        _spot.Start();
    }

    /// <summary>
    /// Callback from the market data producer. Adding the tick to a Queue for 
    /// processing. This way we do not block thread receiving the tick.
    /// </summary>
    /// <param name="tickData">Updated Tick</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task OnNewTickReceived(string tickData)
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
    private TickerRecord DeserializeToRecord(string tickData)
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
        if(delimiter < 0)
            last = double.Parse(tickDataSpan);
        else
            last = double.Parse(tickDataSpan.Slice(0, delimiter));

        return new TickerRecord(ccyPair, bid, ask, last);        
    }
}

public record struct TickerRecord(string Ticker, double Bid, double Ask, double Last);
