using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DiscordBot.Services;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    public class DeltaRaumiEventHandler
    {
        /// <summary>
        /// true = check and update all missing servers on start.
        /// </summary>
        private bool guildCheck = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaRaumiEventHandler"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param>
        public DeltaRaumiEventHandler(DiscordSocketClient client,StatService stat, ImprovedLoggingService logger,MessageService message)
        {
            SocketGuild guild; 

            _client = client;
            _stat= stat;
            _logger= logger;
            _message= message;
            //_guild = guild;
            //_welcome= welcome;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        public DiscordSocketClient _client { get; }

        private MessageService _message { get; }

        private SocketGuild _guild{ get; }

        private StatService _stat{ get; }

        private ImprovedLoggingService _logger;

        private WelcomeMessageService _welcome{ get; }


        internal Task LogAsync(LogMessage message)
        {
            return Task.Run(() => _logger.Log(message.Message, message.Source, DiscordLoggingAdapter.ConvertDiscordLogLevel(message.Severity)));
        }

        internal Task JoinedGuildAsync(SocketGuild guild)
        {
            throw new NotImplementedException();
        }
        
        internal async Task LeftGuildAsync(SocketGuild guild)
        {
            throw new NotImplementedException();
        }

        internal async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            await _message.GetMessageReceivedAsync(socketMessage);
            await _message.LevelsHandler(socketMessage);
            //throw new NotImplementedException();
        }
    }
}
