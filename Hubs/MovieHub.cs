using Microsoft.AspNetCore.SignalR;

namespace MovieApi.Hubs
{
    public class MovieHub : Hub
    {
        public void SendData(IHubContext<MovieHub> context)
        {
            context.Clients.All.SendAsync("GetData");
        }
    }
}