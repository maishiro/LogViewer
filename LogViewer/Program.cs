using LogViewer.Hubs;
using LogViewer.Models;
using LogViewer.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilogの設定
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/logviewer.log",
        fileSizeLimitBytes: 5_000_000, // 5MB
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 9, // 最大9個のバックアップ
        shared: true )
    .CreateLogger();

// Serilogをサービスに追加
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
//builder.Services.AddHostedService<LogMonitorService>(); // LogMonitorServiceを登録


// XML設定ファイルの読み込み
var xmlConfigService = new XmlConfigService("config/config.xml");
var xmlConfig = xmlConfigService.LoadConfig();
//
var pageConfig = new PageConfig
{
    PageTitle = xmlConfig.Element("pageTitle")?.Value,
    PageDescription = xmlConfig.Element("pageDescription")?.Value,
    RefreshInterval = int.Parse(xmlConfig.Element("refreshInterval")?.Value ?? "5000"),
    MaxEntries = int.Parse(xmlConfig.Element("maxEntries")?.Value ?? "100")
};
// PageConfigをDIコンテナに登録
builder.Services.AddSingleton( pageConfig );


int port = 5000; // 初期ポート番号

// ConfigurationからUrlsを取得
var urls = builder.Configuration["Urls"];
// プログラム引数からUrlsを取得
var argsUrls = args.FirstOrDefault(arg => arg.StartsWith("--urls="))?.Split('=')[1];
if( !string.IsNullOrEmpty( argsUrls ) )
{
    urls = argsUrls;
    var uri = new Uri( argsUrls );
    if( uri != null )
        port = uri.Port;

}
if( string.IsNullOrEmpty( urls ) )
{
    while( true )
    {
        try
        {
            builder.WebHost.UseUrls( $"http://*:{port}" );
            Console.WriteLine( $"Trying to start server on port {port}" );
            break;
        }
        catch( Exception )
        {
            Console.WriteLine( $"Port {port} is in use, trying next port..." );
            port++;
        }
    }
}
else
{
    builder.WebHost.UseUrls( urls );
    Console.WriteLine( $"Using configured URLs: {urls}" );
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if( !app.Environment.IsDevelopment() )
{
    app.UseExceptionHandler( "/Home/Error" );
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}" );
app.MapHub<LogHub>( "/logHub" );

app.Run();
