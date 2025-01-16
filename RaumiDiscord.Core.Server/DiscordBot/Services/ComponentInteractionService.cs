using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    class ComponentInteractionService
    {
        private readonly DiscordSocketClient Client;
        private readonly DeltaRaumiDbContext DeltaRaumiDbContext;
        private readonly LoggingService LoggingService;

        private Discord.Color RaumiMainColor = new Discord.Color(0x7bb3ee);
        private Discord.Color RaumiSubColor = new Discord.Color(0xf02443);

        public ComponentInteractionService(DiscordSocketClient client, DeltaRaumiDbContext deltaRaumiDbContext, LoggingService loggingService)
        {
            Client = client;
            DeltaRaumiDbContext = deltaRaumiDbContext;
            LoggingService = loggingService;

            client.SelectMenuExecuted += Client_SelectMenuExecuted;
            client.ButtonExecuted += Client_ButtonExecuted;
            //client.SlashCommandExecuted += Client_SlashCommandExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent component)
        {
            await component.DeferAsync();

            EmbedBuilder builder= new EmbedBuilder();
            switch (component.Data.CustomId)
            {
                case "DoPat":
                    builder.WithAuthor(component.User);
                    builder.WithDescription("あなたはこの狐を撫でてみることにした\nまんざらでもなさそうだ");
                    builder.WithImageUrl("");
                    builder.WithColor(RaumiMainColor);
                    await component.FollowupAsync(embed: builder.Build(),ephemeral: true) ;
                    break;
                case "DontPat":
                    builder.WithAuthor(component.User);
                    builder.WithDescription("あなたはこの狐に触ることを拒んだ\nご機嫌ななめだった");
                    builder.WithImageUrl("");
                    builder.WithColor(Color.DarkRed);
                    await component.FollowupAsync(embed: builder.Build(), ephemeral: true) ;
                    break;
                default:
                    await LoggingService.LogGeneral("通常では到達できないエラー(E-5900)",LoggingService.LogGeneralSeverity.Error);
                    break;
            }
        }

        private async Task Client_SelectMenuExecuted(SocketMessageComponent component)
        {
            throw new NotImplementedException();
        }
    }
}