using Discord;
using Discord.WebSocket;
using NuGet.ProjectModel;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using System.Threading.Channels;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    /// <summary>
    ///     DeltaRaumiEventHandlerは、Discordのイベントを処理するためのクラスです。
    /// </summary>
    public class DeltaRaumiEventHandler
    {
        /// <summary>
        /// DeltaRaumiEventHandlerは、Discordのイベントを処理するためのクラスです。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stat"></param>
        /// <param name="logger"></param>
        /// <param name="messageService"></param>
        public DeltaRaumiEventHandler(DiscordSocketClient client,StatService statService, ImprovedLoggingService loggerService, MessageService messageService)
        {
            SocketGuild guild; 

            _client = client;
            _statService = statService;
            _loggerService = loggerService;
            _messageService= messageService;
            //_cacheMessage = cacheMessage;
            //_guild = guild;
            //_welcome= welcome;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public DiscordSocketClient _client { get; }

        private MessageService _messageService { get; }

        private Cacheable<IUserMessage, ulong> _cacheMessage { get; }

        private SocketGuild _guild { get; }

        private StatService _statService { get; }

        private ImprovedLoggingService _loggerService;

        private WelcomeMessageService _welcome { get; }


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
            await _messageService.GetMessageReceivedAsync(socketMessage);
            await _messageService.LevelsHandler(socketMessage);
            await _statService.UserStatDetection(socketMessage);
        }

        internal async Task MessageUpdated(Cacheable<IMessage, ulong> ucacheable, SocketMessage message, ISocketMessageChannel socketMessageChannel)
        {
            await _messageService.GetMessageUpdatedAsync(ucacheable, message, socketMessageChannel);
        }

        internal async Task MessageDeleted(Cacheable<IMessage, ulong> dcacheable, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            await _messageService.GetMessageDeletedAsync(dcacheable, cachedChannel);
        }

        internal Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // 実装が必要な場合はここに処理を追加
            return Task.CompletedTask;
        }

        internal Task ReactionRemovedAsync(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // 実装が必要な場合はここに処理を追加
            return Task.CompletedTask;
        }

        internal Task UserJoinedAsync(SocketGuildUser user)
        {
            // 実装が必要な場合はここに処理を追加
            _loggerService.Log($"User joined: {user.Username}", "UserJoinedAsync");
            return Task.CompletedTask;
        }

        internal Task UserLeftAsync(SocketGuildUser user)
        {
            // 実装が必要な場合はここに処理を追加
            _loggerService.Log($"User left: {user.Username}", "UserLeftAsync");
            return Task.CompletedTask;
        }
    }
}
