using Microsoft.AspNetCore.Mvc;
using LogViewer.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;

namespace LogViewer.Controllers
{
    public class LogController : Controller
    {
        private readonly string _logDirectory;
        private readonly PageConfig _pageConfig;

        public LogController( IConfiguration configuration, PageConfig pageConfig )
        {
            _logDirectory = configuration ["LogSettings:LogDirectory"];
            _pageConfig = pageConfig;
        }

        public IActionResult Index()
        {
            var logEntries = new List<LogEntry>();

            foreach( var filePath in Directory.GetFiles( _logDirectory, "*.log" ) )
            {
                using( var fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                using( var reader = new StreamReader( fs, Encoding.UTF8 ) )
                {
                    string line;
                    while( ( line = reader.ReadLine() ) != null && logEntries.Count < _pageConfig.MaxEntries )
                    {
                        var parts = line.Split(' ', 2);
                        if( parts.Length == 2 && DateTime.TryParse( parts [0], out DateTime timestamp ) )
                        {
                            logEntries.Add( new LogEntry { Timestamp = timestamp, Message = parts [1] } );
                        }
                    }
                }
            }

            ViewBag.PageConfig = _pageConfig;
            return View( logEntries );
        }
    }
}
