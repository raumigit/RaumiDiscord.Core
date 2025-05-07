using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    public class LevelService
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DiscordSocketClient _client;
        public LevelService(DiscordSocketClient client, ImprovedLoggingService logging)
        {
            _client = client;
            _logger = logging;

            // イベントハンドラを登録
        }
        public static async Task LevelsProsessAsync(SocketMessage message)
        {
            var guildChannel = message.Channel as ITextChannel;
            var guild = guildChannel.Guild as SocketGuild;
            var guildUser = message.Author as SocketGuildUser;

            var embed = new EmbedBuilder();

            if (!(message is SocketUserMessage Message) || Message.Channel is IDMChannel)
            {
                return;
            }

        }
    }
}
