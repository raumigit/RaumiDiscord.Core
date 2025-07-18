﻿using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    class DiscordCoordinationService
    {
        private readonly DiscordSocketClient Client;
        private readonly SlashCommandInterationService SlashCommandService;
        private readonly ComponentInteractionService ComponentInteractionService;
        private readonly ImprovedLoggingService LoggingService;
        private readonly WelcomeMessageService WelcomeMessageService;
        public Configuration config { get; set; }
        public DiscordCoordinationService(DiscordSocketClient client, SlashCommandInterationService slashCommandService, ComponentInteractionService componentInteractionService, ImprovedLoggingService loggingService/*, WelcomeMessageService welcomeMessageService*/)
        {
            Client = client;
            SlashCommandService = slashCommandService;
            ComponentInteractionService = componentInteractionService;
            LoggingService = loggingService;
            //WelcomeMessageService = welcomeMessageService;

            Client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            config = new Configuration().GetConfig();
            string game = config.Setting?.CustomStatusGame ?? "null";
            Console.WriteLine(game ??= "null!");
            await Client.SetGameAsync(game);
            await LoggingService.Log("Startup Complete", "Startup");
            await LoggingService.Log($"Logged in as {Client.CurrentUser.Username}", "Startup");
        }
    }
}