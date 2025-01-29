using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;

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

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Task? task = Task.Run(async () => {
    try
    {
        Console.WriteLine("Deltaraumi_load呼び出し済み");
        Deltaraumi_Discordbot.Deltaraumi_load(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }
});

app.Run();
