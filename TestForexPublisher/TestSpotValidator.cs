using FluentValidation.TestHelper;
using ForexPublisher.Validations;
namespace TestForexPublisher;

public class TestSpotValidator
{
    private SpotValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new SpotValidator();
    }

    [Test]
    public void CurrencyPair_Null_ShouldHaveValidationError()
    {
        var spot = new ForexPublisher.Domain.Spot(null, 1.0, 1.1, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.CurrencyPair);
    }

    [Test]
    public void CurrencyPair_Empty_ShouldHaveValidationError()
    {
        var spot = new ForexPublisher.Domain.Spot("", 1.0, 1.1, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.CurrencyPair);
    }

    [TestCase(0.0)]
    [TestCase(-1.0)]
    [TestCase(-0.1)]
    public void Bid_InvalidValues_ShouldHaveValidationError(double bid)
    {
        var spot = new ForexPublisher.Domain.Spot("EURUSD", bid, 1.1, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.Bid);
    }

    [TestCase(0.0)]
    [TestCase(-1.0)]
    [TestCase(-0.1)]
    public void Ask_InvalidValues_ShouldHaveValidationError(double ask)
    {
        var spot = new ForexPublisher.Domain.Spot("EURUSD", 1.0, ask, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.Ask);
    }

    [TestCase(10.0, 9.8)]
    public void BidAsk_BidShouldBeLessThanAsk_ShouldHaveValidationError(double bid, double ask)
    {
        var spot = new ForexPublisher.Domain.Spot("EURUSD", bid, ask, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.Bid);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void PublishFrequency_InvalidValues_ShouldHaveValidationError(int publishedFreq)
    {
        var spot = new ForexPublisher.Domain.Spot("EURUSD", 1.0, 1.1, 0.1, publishedFreq);
        var result = _validator.TestValidate(spot);
        result.ShouldHaveValidationErrorFor(s => s.PublishFrequencyInMs);
    }

    [Test]
    public void ValidSpot_NoValidationErrors()
    {
        var spot = new ForexPublisher.Domain.Spot("EURUSD", 1.0, 1.1, 0.1, 1000);
        var result = _validator.TestValidate(spot);
        result.ShouldNotHaveValidationErrorFor(s => s.CurrencyPair);
        result.ShouldNotHaveValidationErrorFor(s => s.Bid);
        result.ShouldNotHaveValidationErrorFor(s => s.Ask);
        result.ShouldNotHaveValidationErrorFor(s => s.Spread);
        result.ShouldNotHaveValidationErrorFor(s => s.PublishFrequencyInMs);
    }
}