using Microsoft.AspNetCore.SignalR;

namespace DeskDashboard.Hubs
{
    /// <summary>
    /// SignalR implementation for the Forex Hub that will push data to the client.
    /// </summary>
    public class ForexHub : Hub
    {
        private readonly ILogger<ForexHub> _logger;
        public ForexHub(ILogger<ForexHub> logger)
        {
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected with Id : {Context.ConnectionId}");
            //We do not need to store the Id of the connection as SignalR does it for us.
            await Clients.Caller.SendAsync("ForexConnected", $"You are now connected with Id : {Context.ConnectionId}");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client connected with Id : {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
