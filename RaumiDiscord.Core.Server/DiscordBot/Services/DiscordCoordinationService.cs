using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DiscordBot.Data;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    class DiscordCoordinationService
    {
        private readonly DiscordSocketClient Client;
        private readonly SlashCommandInterationService SlashCommandService;
        private readonly ComponentInteractionService ComponentInteractionService;
        private readonly LoggingService LoggingService;
        private readonly WelcomeMessageService WelcomeMessageService;
        public Configuration config { get; set; }
        public DiscordCoordinationService(DiscordSocketClient client, SlashCommandInterationService slashCommandService, ComponentInteractionService componentInteractionService, LoggingService loggingService, WelcomeMessageService welcomeMessageService)
        {
            Client = client;
            SlashCommandService = slashCommandService;
            ComponentInteractionService = componentInteractionService;
            LoggingService = loggingService;
            WelcomeMessageService = welcomeMessageService;

            Client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            config = new Configuration().GetConfig();
            string game = config.Setting?.CustomStatusGame ?? "null";
            Console.WriteLine(game??="null!");
            await Client.SetGameAsync(game);
            await LoggingService.LogGeneral("Startup Complete");
            await LoggingService.LogGeneral($"Logged in as {Client.CurrentUser.Username}");
        }
    }
}