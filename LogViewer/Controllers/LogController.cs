using LogViewer.Hubs;
using LogViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LogViewer.Controllers
{
    [ApiController]
    [Route( "api/[controller]" )]
    public class LogController : Controller
    {
        private readonly string _logDirectory;
        private readonly PageConfig _pageConfig;
        private readonly IHubContext<LogHub> _hubContext;

        public LogController( IConfiguration configuration, PageConfig pageConfig, IHubContext<LogHub> hubContext )
        {
            _logDirectory = configuration ["LogSettings:LogDirectory"];
            _pageConfig = pageConfig;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post( [FromBody] List<LogEntry> body )
        {
            // ここでデータを処理（例：データベースに保存）

            Console.WriteLine( $"{body.ToString()}" );

            // SignalRを使用してクライアントに更新を通知
            foreach( var item in body )
                await _hubContext.Clients.All.SendAsync( "ReceiveLogEntry", item );

            return Ok( body );
        }
    }
}
