using ForexPublisher.Domain;

namespace ForexPublisher.Services;

public interface ISpotService
{
    void Start();
    void Stop();
    void Pause();
    void Resume();
    bool AddSubscription(Spot spot);
}
