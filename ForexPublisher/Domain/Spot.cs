namespace ForexPublisher.Domain;

public record Spot (string CurrencyPair, double Bid, double Ask, double Spread, int PublishFrequencyInMs);