using DeskDashboard.Domain;

namespace DeskDashboard.Services;

public interface ISpotService
{
    void Start();
    void Stop();
    void Pause();
    void Resume();
    bool AddSubscription(Spot spot);
}
