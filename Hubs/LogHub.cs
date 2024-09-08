using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LogViewer.Hubs
{
    public class LogHub : Hub
    {
        public async Task SendLogEntry( string timestamp, string message )
        {
            await Clients.All.SendAsync( "ReceiveLogEntry", timestamp, message );
        }
    }
}