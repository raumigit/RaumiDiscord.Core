using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    /// <summary>
    /// DeltaRaumiEventHandlerは、Discordのイベントを処理するためのクラスです。
    /// </summary>
    public class DeltaRaumiEventHandler
    {
        
        /// <summary>
        /// DeltaRaumiEventHandlerは、Discordのイベントを処理するためのクラスです。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="statService"></param>
        /// <param name="loggerService"></param>
        /// <param name="messageService"></param>
        /// <param name="cacheMessage"></param>
        /// <param name="guild"></param>
        /// <param name="welcome"></param>
        public DeltaRaumiEventHandler(DiscordSocketClient client, StatService statService, ImprovedLoggingService loggerService, MessageService messageService, Cacheable<IUserMessage, ulong> cacheMessage, SocketGuild guild, WelcomeMessageService welcome)
        
        {
            // SocketGuild guild;

            Client = client;
            StatService = statService;
            this._loggerService = loggerService;
            MessageService = messageService;
            CacheMessage = cacheMessage;
            Welcome = welcome;
            Guild = guild;
            //_cacheMessage = cacheMessage;
            //_guild = guild;
            //_welcome= welcome;
        }


        /// <summary>
        /// Gets the client.
        /// </summary>
        public DiscordSocketClient Client { get; }

        private MessageService MessageService { get; }

        private Cacheable<IUserMessage, ulong> CacheMessage { get; }

        private SocketGuild Guild { get; }

        private StatService StatService { get; }

        private readonly ImprovedLoggingService _loggerService;

        private WelcomeMessageService Welcome { get; }


        internal Task LogAsync(LogMessage message)
        {
            return Task.Run(() => _loggerService.Log(message.Message, message.Source, DiscordLoggingAdapter.ConvertDiscordLogLevel(message.Severity)));
        }

        internal Task JoinedGuildAsync(SocketGuild guild)
        {
            // 実装が必要な場合はここに処理を追加
            _loggerService.Log($"Guild joined: {guild.Name}", "JoinedGuildAsync");
            return Task.CompletedTask;
        }

        internal Task LeftGuildAsync(SocketGuild guild)
        {
            // 実装が必要な場合はここに処理を追加
            _loggerService.Log($"Left guild: {guild.Name}", "LeftGuildAsync");
            return Task.CompletedTask;
        }

        internal async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            await MessageService.GetMessageReceivedAsync(socketMessage);
            await MessageService.LevelsHandler(socketMessage);
            await StatService.UserStatDetection(socketMessage);
        }

        internal async Task MessageUpdated(Cacheable<IMessage, ulong> ucacheable, SocketMessage message, ISocketMessageChannel socketMessageChannel)
        {
            await MessageService.GetMessageUpdatedAsync(ucacheable, message, socketMessageChannel);
        }

        internal async Task MessageDeleted(Cacheable<IMessage, ulong> dcacheable, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            await MessageService.GetMessageDeletedAsync(dcacheable, cachedChannel);
        }

        internal Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // 実装が必要な場合はここに処理を追加
            return Task.CompletedTask;
        }

        internal async Task<Task> ReactionRemovedAsync(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // 実装が必要な場合はここに処理を追加
            return Task.CompletedTask;
        }

        internal async Task UserJoinedAsync(SocketGuildUser user)
        {
            // 実装が必要な場合はここに処理を追加
            await _loggerService.Log($"User joined: {user.Username}", "UserJoinedAsync");

            await Welcome.WelcomeCardGenerator(user);

            //return Task.CompletedTask;
        }

        internal Task UserLeftAsync(SocketGuildUser user)
        {
            // 実装が必要な場合はここに処理を追加
            _loggerService.Log($"User left: {user.Username}", "UserLeftAsync");
            return Task.CompletedTask;
        }
    }
}
