using Forex;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestForexPublisher;

public class TestForexDataProducer
{
    private Mock<ILogger<ForexPublisher.DataProducers.Forex>> _mockLogger;
    private Mock<ISpot> _mockSpot;




    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<ForexPublisher.DataProducers.Forex>>();
        _mockSpot = new Mock<ISpot>();
    }

    [TestCase("EURUSD:101.100, 101.102, 101.1015")]
    [TestCase("EURUSD:101.100, 101.102, 101.1015, abc")]
    [TestCase("EURUSD:101.100, 101.102, 101.1015, 101.225")]
    [TestCase("EURUSD:101.100, 101.102, 101.1015, 101.225,1,2,3,4.23")]
    [TestCase("EURUSD     :     101.100,   101.102, 101.1015    ")]
    [TestCase("EURUSD     :     101.100,   101.102, 101.1015    ,")]
    public void DeserializeToRecord_ValidInput_ReturnsTickerRecord(string newTick)
    {
        _mockSpot.Setup(s => s.Start());
        var forex = new ForexPublisher.DataProducers.Forex(_mockLogger.Object, null, _mockSpot.Object);
        var result = forex.DeserializeToRecord(newTick);
        Assert.IsTrue((result.Ticker == "EURUSD")
            && (result.Bid == 101.100)
            && (result.Ask == 101.102)
            && (result.Last == 101.1015));
    }

    [TestCase("EURUSD-101.100, 101.102, 101.1015")]
    [TestCase("EURUSD:101.100| 101.102| 101.1015")]
    [TestCase("EURUSD:101.100")]
    public void DeserializeToRecord_InValidInput_ThrowsException(string newTick)
    {
        _mockSpot.Setup(s => s.Start());
        var forex = new ForexPublisher.DataProducers.Forex(_mockLogger.Object, null, _mockSpot.Object);
        Assert.That(() => forex.DeserializeToRecord(newTick), Throws.Exception);
    }
}
