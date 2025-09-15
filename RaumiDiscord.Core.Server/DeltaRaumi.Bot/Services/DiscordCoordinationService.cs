using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    class DiscordCoordinationService
    {
        private readonly DiscordSocketClient _client;
        private readonly SlashCommandInterationService _slashCommandService;
        private readonly ComponentInteractionService _componentInteractionService;
        private readonly ImprovedLoggingService _loggingService;
        private readonly WelcomeMessageService _welcomeMessageService;
        public Configuration Config { get; set; }
        public DiscordCoordinationService(DiscordSocketClient client, SlashCommandInterationService slashCommandService, ComponentInteractionService componentInteractionService, ImprovedLoggingService loggingService, WelcomeMessageService welcomeMessageService, Configuration config /*, WelcomeMessageService welcomeMessageService*/)
        {
            _client = client;
            _slashCommandService = slashCommandService;
            _componentInteractionService = componentInteractionService;
            _loggingService = loggingService;
            _welcomeMessageService = welcomeMessageService;
            Config = config;
            //WelcomeMessageService = welcomeMessageService;

            _client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            Config = new Configuration().GetConfig();
            string game = Config.Setting?.CustomStatusGame ?? "null";
            Console.WriteLine(game);
            await _client.SetGameAsync(game);
            await _loggingService.Log("Startup Complete", "Startup");
            await _loggingService.Log($"Logged in as {_client.CurrentUser.Username}", "Startup");
        }
    }
}