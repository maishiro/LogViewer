using Microsoft.AspNetCore.SignalR;
using LogViewer.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LogViewer.Services
{
    public class LogMonitorService : BackgroundService
    {
        private readonly string _logDirectory = @"C:\Logs"; // ログディレクトリのパス
        private readonly IHubContext<LogHub> _hubContext;
        private static Dictionary<string, long> filePositions = new Dictionary<string, long>();

        public LogMonitorService( IHubContext<LogHub> hubContext, IConfiguration configuration )
        {
            _hubContext = hubContext;
            _logDirectory = configuration ["LogSettings:LogDirectory"];
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            Log.Information( "LogMonitorService started." );

            var watcher = new FileSystemWatcher(_logDirectory, "*.log")
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            watcher.Created += ( s, e ) => ProcessLogFile( e.FullPath );
            watcher.Changed += ( s, e ) => ProcessLogFile( e.FullPath );
            watcher.Renamed += ( s, e ) =>
            {
                filePositions [e.FullPath] = 0;
                ProcessLogFile( e.FullPath );
            };

            watcher.EnableRaisingEvents = true;

            while( !stoppingToken.IsCancellationRequested )
            {
                foreach( var filePath in Directory.GetFiles( _logDirectory, "*.log" ) )
                {
                    ProcessLogFile( filePath );
                }
                await Task.Delay( 1000, stoppingToken ); // 1秒待機してから再度チェック
            }

            Log.Information( "LogMonitorService stopping." );
        }

        private void ProcessLogFile( string filePath )
        {
            Log.Information( "Processing log file: {FilePath}", filePath );

            if( !filePositions.ContainsKey( filePath ) )
            {
                filePositions [filePath] = 0;
            }

            using( var fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
            {
                fs.Seek( filePositions [filePath], SeekOrigin.Begin );
                using( var reader = new StreamReader( fs, Encoding.UTF8 ) )
                {
                    string line;
                    while( ( line = reader.ReadLine() ) != null )
                    {
                        var parts = line.Split(' ', 2);
                        if( parts.Length == 2 && DateTime.TryParse( parts [0], out DateTime timestamp ) )
                        {
                            _hubContext.Clients.All.SendAsync( "ReceiveLogEntry", parts [0], parts [1] );
                        }
                    }
                    filePositions [filePath] = fs.Position;
                }
            }
        }
    }
}