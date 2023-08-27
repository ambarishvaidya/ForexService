using BenchmarkDotNet.Attributes;
using System.Text.Json;

namespace BenchmarkForexPublisher;

[MemoryDiagnoser]
public class DataProducerForex
{
    //public string tickData = "GBPUSD: 110.012, 110.018, 110.0101";

    [Benchmark]
    [ArgumentsSource(nameof(TickData))]
    public void WithoutTaskSplit(string tickData)
    {
        string[] keyValue = tickData.Split(':');
        string[] values = keyValue[1].Split(',');
        (string pairKey, double[] rates) = (keyValue[0].Trim(), new double[] { double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]) });
        string json = ToRecordJson(pairKey, rates[0], rates[1], rates[2]);        
    }

    [Benchmark]
    [ArgumentsSource(nameof(TickData))]
    public void WithoutTaskSpan(string tickData)
    {
        ReadOnlySpan<char> tickDataSpan = tickData.AsSpan();
        int colon = tickDataSpan.IndexOf(':');
        var ccyPair = tickDataSpan.Slice(0, colon).ToString().Trim();        

        tickDataSpan = tickDataSpan.Slice(colon + 1);        
        colon = tickDataSpan.IndexOf(',');        
        double bid = double.Parse(tickDataSpan.Slice(0, colon));        

        tickDataSpan = tickDataSpan.Slice(colon + 1);        
        colon = tickDataSpan.IndexOf(',');        
        double ask = double.Parse(tickDataSpan.Slice(0, colon));        

        tickDataSpan = tickDataSpan.Slice(colon + 1);        
        colon = tickDataSpan.IndexOf(',');                
        double last = double.Parse(tickDataSpan);
        
        string json = ToRecordJson(ccyPair, bid, ask, last);
    }

    [Benchmark]
    [ArgumentsSource(nameof(TickData))]
    public void WithTask(string tickData)
    {
        Task.Run(() =>
        {
            string[] keyValue = tickData.Split(':');
            string[] values = keyValue[1].Split(',');
            (string pairKey, double[] rates) = (keyValue[0].Trim(), new double[] { double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]) });
            string json = ToRecordJson(pairKey, rates[0], rates[1], rates[2]);            
        });
    }

    [Benchmark]
    [ArgumentsSource(nameof(TickData))]
    public Task WithTaskAsync(string tickData)
    {
        return Task.Run(async () =>
        {
            string[] keyValue = tickData.Split(':');
            string[] values = keyValue[1].Split(',');
            (string pairKey, double[] rates) = (keyValue[0].Trim(), new double[] { double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]) });
            string json = ToRecordJson(pairKey, rates[0], rates[1], rates[2]);            
        });
    }

    private (string pairKey, double[] rates) Deserialize(string tickData)
    {
        string[] keyValue = tickData.Split(':');
        string[] values = keyValue[1].Split(',');
        return (keyValue[0].Trim(), new double[] { double.Parse(values[0]), double.Parse(values[1]), double.Parse(values[2]) });
    }

    private string ToRecordJson(string tkr, double bid, double ask, double last)
    {
        return JsonSerializer.Serialize(new TickerRecord(tkr, bid, ask, last));
    }

    public IEnumerable<string> TickData => new string[]
    {
        "GBPUSD: 110.012, 110.018, 110.0101"
        //,"EURUSD: 1.175, 1.180, 1.1751"
        ,"  USDJPY : 109.50      , 109.55        , 109.5051"
        ,"AUDUSD: 0.735, 0.740, 0.7351"
        //,"USDCAD: 1.245, 1.250, 1.2451"
        //,"NZDUSD: 0.695, 0.700, 0.6951"
        //,"USDCHF: 0.915, 0.920, 0.9151"
        //,"GBPJPY: 150.80, 150.85, 150.8051"
        //,"EURGBP: 0.845, 0.850, 0.8451"
        //,"AUDJPY: 80.50, 80.55, 80.5051"
        //,"EURJPY: 128.20, 128.25, 128.2051"
        //,"USDCNH: 6.480, 6.485, 6.4801"
        //,"GBPCHF: 1.005, 1.010, 1.0051"
        //,"AUDNZD: 1.055, 1.060, 1.0551"
        //,"EURAUD: 1.595, 1.600, 1.5951"
        //,"USDSEK: 8.655, 8.660, 8.6551"
        //,"USDNOK: 8.895, 8.900, 8.8951"
        //,"CADJPY: 87.75, 87.80, 87.7551"
        //,"NZDJPY: 56.80, 56.85, 56.8051"
        //,"AUDCHF: 0.675, 0.680, 0.6751"
    };
}

/**


|           Method |             tickData |     Mean |     Error |    StdDev |   Gen0 |   Gen1 |   Gen2 | Allocated |
|----------------- |--------------------- |---------:|----------:|----------:|-------:|-------:|-------:|----------:|
| WithoutTaskSplit |   USD(...).5051 [49] | 1.674 us | 0.0414 us | 0.1189 us | 0.3128 |      - |      - |     656 B |
|  WithoutTaskSpan |   USD(...).5051 [49] | 1.346 us | 0.0392 us | 0.1054 us | 0.1297 |      - |      - |     272 B |
|         WithTask |   USD(...).5051 [49] | 1.563 us | 0.1069 us | 0.3068 us | 0.2136 | 0.0362 | 0.0057 |     652 B |
|    WithTaskAsync |   USD(...).5051 [49] | 3.852 us | 0.0715 us | 0.1539 us | 0.4272 |      - |      - |     897 B |
| WithoutTaskSplit | AUDUS(...).7351 [28] | 1.199 us | 0.0204 us | 0.0181 us | 0.2594 |      - |      - |     544 B |
|  WithoutTaskSpan | AUDUS(...).7351 [28] | 1.076 us | 0.0184 us | 0.0282 us | 0.1068 |      - |      - |     224 B |
|         WithTask | AUDUS(...).7351 [28] | 1.884 us | 0.1808 us | 0.5329 us | 0.1411 | 0.0496 | 0.0095 |     550 B |
|    WithTaskAsync | AUDUS(...).7351 [28] | 4.516 us | 0.2366 us | 0.6975 us | 0.3738 |      - |      - |     785 B |
| WithoutTaskSplit | GBPUS(...).0101 [34] | 2.058 us | 0.1929 us | 0.5689 us | 0.2747 |      - |      - |     576 B |
|  WithoutTaskSpan | GBPUS(...).0101 [34] | 2.408 us | 0.2115 us | 0.6237 us | 0.1144 |      - |      - |     240 B |
|         WithTask | GBPUS(...).0101 [34] | 2.049 us | 0.1454 us | 0.4196 us | 0.1717 | 0.0420 | 0.0076 |     647 B |
|    WithTaskAsync | GBPUS(...).0101 [34] | 6.949 us | 0.2082 us | 0.6139 us | 0.3815 |      - |      - |     817 B |

// * Warnings *
MultimodalDistribution
  DataProducerForex.WithoutTaskSplit: Default -> It seems that the distribution can have several modes (mValue = 2.85)
  DataProducerForex.WithTask: Default         -> It seems that the distribution can have several modes (mValue = 2.83)
  DataProducerForex.WithTaskAsync: Default    -> It seems that the distribution can have several modes (mValue = 3.15)
  DataProducerForex.WithoutTaskSpan: Default  -> It seems that the distribution is bimodal (mValue = 3.92)
  DataProducerForex.WithTaskAsync: Default    -> It seems that the distribution can have several modes (mValue = 2.96)

// * Hints *
Outliers
  DataProducerForex.WithoutTaskSplit: Default -> 5 outliers were removed (2.35 us..3.56 us)
  DataProducerForex.WithoutTaskSpan: Default  -> 16 outliers were removed (1.81 us..2.13 us)
  DataProducerForex.WithTask: Default         -> 5 outliers were removed, 6 outliers were detected (708.35 ns, 2.41 us..3.31 us)
  DataProducerForex.WithTaskAsync: Default    -> 8 outliers were removed (4.39 us..6.39 us)
  DataProducerForex.WithoutTaskSplit: Default -> 1 outlier  was  removed (1.31 us)
  DataProducerForex.WithoutTaskSpan: Default  -> 6 outliers were removed (1.20 us..1.32 us)
  DataProducerForex.WithTask: Default         -> 4 outliers were removed (3.18 us..3.95 us)

// * Legends *
  tickData  : Value of the 'tickData' parameter
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Gen0      : GC Generation 0 collects per 1000 operations
  Gen1      : GC Generation 1 collects per 1000 operations
  Gen2      : GC Generation 2 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 us      : 1 Microsecond (0.000001 sec)

**/

public record struct TickerRecord(string Ticker, double Bid, double Ask, double Last);
