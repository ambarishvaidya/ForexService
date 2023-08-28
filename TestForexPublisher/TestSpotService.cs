using Forex;
using ForexPublisher.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestForexPublisher;

public class TestSpotService
{
    private Mock<ILogger<SpotService>> _mockLogger;
    private Mock<ISpot> _mockSpot;    

    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<SpotService>>();
        _mockSpot = new Mock<ISpot>();        
    }

    [Test]
    public void AddSubscription_ValidSpot_ShouldReturnTrue()
    {
        _mockSpot.Setup(s => s.AddTickDefinition(It.IsAny<TickDefinition>()));
        _mockSpot.Setup(s => s.GetScheduledTicks()).Returns(() => new (string, int)[] { ("EURUSD", 1000)});
        
        var spot = new ForexPublisher.Domain.Spot("EURUSD", 1.0, 1.1, 0.1, 1000);
        var spotService = new SpotService(_mockLogger.Object, _mockSpot.Object);
        var result = spotService.AddSubscription(spot);
        Assert.IsTrue(result);
    }

    [Test]
    public void AddSubscription_InValidSpot_ShouldReturnFalse()
    {
        _mockSpot.Setup(s => s.AddTickDefinition(It.IsAny<TickDefinition>()));
        _mockSpot.Setup(s => s.GetScheduledTicks()).Returns(() => new (string, int)[] { });

        var spot = new ForexPublisher.Domain.Spot("EURUSD", -1.0, 1.1, 0.1, 1000);
        var spotService = new SpotService(_mockLogger.Object, _mockSpot.Object);
        var result = spotService.AddSubscription(spot);
        Assert.IsFalse(result);
    }
}
