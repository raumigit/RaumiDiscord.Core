using RaumiDiscord.Core.Server.DiscordBot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

Task task = Task.Run(async () => {
    try
    {
        Console.WriteLine("Deltaraumi_loadŒÄ‚Ño‚µÏ‚İ");
        Deltaraumi_Discordbot.Deltaraumi_load(args);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }
});


app.Run();
