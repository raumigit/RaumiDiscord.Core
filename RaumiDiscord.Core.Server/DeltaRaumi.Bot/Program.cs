using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;
using System.Runtime.InteropServices;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//サービスにDataContextを登録する
builder.Services.AddDbContext<DeltaRaumiDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:6440");
}
// レートリミッターを追加
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        // IPアドレスを使って制限（null対策含む）
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100, // 1分あたり100回
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6, // セグメント数
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // 待機なし、即429
            });
    });

    options.RejectionStatusCode = 429; // 明示的に指定（デフォルトも429）
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //options.RoutePrefix = string.Empty;
});

app.MapControllers();
if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.Error.WriteLine("このシステムのターゲットはWindowsのため予期されない動作を停止します。");
    // crashReport :
    Environment.Exit(-1);
    return;
}

app.MapFallbackToFile("/index.html");

Task? task = Task.Run(() =>
{
    try
    {
        Console.WriteLine("Deltaraumi_load呼び出し済み");
        Deltaraumi_Discordbot.Deltaraumi_load(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }

    return Task.CompletedTask;
});


app.Run();
