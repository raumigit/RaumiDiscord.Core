using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    public class DiscordCoordinationService
    {
        private readonly DiscordSocketClient Client;
        private readonly SlashCommandInterationService SlashCommandService;
        private readonly ComponentInteractionService ComponentInteractionService;
        private readonly LoggingService LoggingService;
        private readonly WelcomeMessageService WelcomeMessageService;

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
            await Client.SetGameAsync("TEST");

            await LoggingService.LogGeneral("Startup Complete");
            await LoggingService.LogGeneral($"Logged in as {Client.CurrentUser.Username}");

        }
    }
}