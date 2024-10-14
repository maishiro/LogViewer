using LogViewer.Models;
using Microsoft.AspNetCore.SignalR;

namespace LogViewer.Hubs
{
    public class LogHub : Hub
    {
        public async Task SendLogEntry( LogEntry entry )
        {
            await Clients.All.SendAsync( "ReceiveLogEntry", entry );
        }
    }
}