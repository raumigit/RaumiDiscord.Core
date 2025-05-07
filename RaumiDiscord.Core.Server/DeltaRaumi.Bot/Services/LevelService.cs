using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// LevelServiceは、ユーザーのレベルを管理するためのサービスです。
    /// </summary>
    public class LevelService
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DiscordSocketClient _client;
        /// <summary>
        /// LevelServiceのコンストラクター
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logging"></param>
        public LevelService(DiscordSocketClient client, ImprovedLoggingService logging)
        {
            _client = client;
            _logger = logging;

            // イベントハンドラを登録
        }
        /// <summary>
        /// LevelsProsessAsyncは、メッセージを処理し、レベルアップの処理を行います。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task LevelsProsessAsync(SocketMessage message)
        {
            var guildChannel = message.Channel as ITextChannel;
            var guild = guildChannel.Guild as SocketGuild;
            var guildUser = message.Author as SocketGuildUser;

            var embed = new EmbedBuilder();

            if (!(message is SocketUserMessage Message) || Message.Channel is IDMChannel)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
