using BenchmarkDotNet.Attributes;
using System.Text.Json;

namespace BenchmarkDeskDashboard;

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
| WithoutTaskSplit |   USD(...).5051 [49] | 1.547 us | 0.0129 us | 0.0107 us | 0.3128 |      - |      - |     656 B |
|  WithoutTaskSpan |   USD(...).5051 [49] | 1.223 us | 0.0201 us | 0.0178 us | 0.1297 |      - |      - |     272 B |
|         WithTask |   USD(...).5051 [49] | 1.452 us | 0.1031 us | 0.2990 us | 0.2155 | 0.0458 | 0.0076 |     709 B |
|    WithTaskAsync |   USD(...).5051 [49] | 4.027 us | 0.0763 us | 0.1900 us | 0.4311 |      - |      - |     896 B |
| WithoutTaskSplit | AUDUS(...).7351 [28] | 1.269 us | 0.0214 us | 0.0306 us | 0.2594 |      - |      - |     544 B |
|  WithoutTaskSpan | AUDUS(...).7351 [28] | 1.071 us | 0.0193 us | 0.0171 us | 0.1068 |      - |      - |     224 B |
|         WithTask | AUDUS(...).7351 [28] | 1.290 us | 0.1016 us | 0.2914 us | 0.2232 | 0.0305 | 0.0057 |     728 B |
|    WithTaskAsync | AUDUS(...).7351 [28] | 5.084 us | 0.2322 us | 0.6357 us | 0.3777 |      - |      - |     784 B |
| WithoutTaskSplit | GBPUS(...).0101 [34] | 1.641 us | 0.0328 us | 0.0449 us | 0.2747 |      - |      - |     576 B |
|  WithoutTaskSpan | GBPUS(...).0101 [34] | 1.401 us | 0.0227 us | 0.0233 us | 0.1144 |      - |      - |     240 B |
|         WithTask | GBPUS(...).0101 [34] | 1.593 us | 0.0965 us | 0.2739 us | 0.2232 | 0.0362 | 0.0076 |     709 B |
|    WithTaskAsync | GBPUS(...).0101 [34] | 4.509 us | 0.0892 us | 0.1515 us | 0.3891 |      - |      - |     816 B |

**/

public record struct TickerRecord(string Ticker, double Bid, double Ask, double Last);
