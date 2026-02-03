using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;

namespace RaumiDiscord.Core.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // テスト用InMemoryデータベースを追加
            // Program.csで Testing 環境の場合は DbContext が登録されないため、ここで追加
            services.AddDbContext<DeltaRaumiDbContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
            });
        });
    }
}