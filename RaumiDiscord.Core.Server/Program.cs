using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//�T�[�r�X��DataContext��o�^����
builder.Services.AddDbContext<DeltaRaumiDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

//app.Urls.Add("http://localhost:6444");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Task? task = Task.Run(() =>
{
    try
    {
        Console.WriteLine("Deltaraumi_load�Ăяo���ς�");
        Deltaraumi_Discordbot.Deltaraumi_load(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }

    return Task.CompletedTask;
});

app.Run();
