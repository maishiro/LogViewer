using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LogViewer.Models;
using LogViewer.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LogViewer.Controllers;

public class HomeController : Controller
{
    private readonly string _logDirectory;
    private readonly PageConfig _pageConfig;
    private readonly ILogger<HomeController> _logger;
    private readonly IHubContext<LogHub> _hubContext;

    public HomeController( IConfiguration configuration, PageConfig pageConfig, ILogger<HomeController> logger, IHubContext<LogHub> hubContext )
    {
        _hubContext = hubContext;

	    _logDirectory = configuration ["LogSettings:LogDirectory"];
	    _pageConfig = pageConfig;
   	
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
