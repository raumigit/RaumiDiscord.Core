using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    internal class DeltaRaumiHandler
    {
       
        public DeltaRaumiHandler(DiscordSocketClient client, DeltaRaumiEventHandler events) 
        {
            _client = client;
            _event = events;
            //_provider = provider;
        }
        /// <summary>
        ///     Gets the client.
        /// </summary>
        private DiscordSocketClient _client { get; }

        /// <summary>
        ///     Gets the event.
        /// </summary>
        private DeltaRaumiEventHandler _event { get; }

        /// <summary>
        ///     Gets the provider.
        /// </summary>
        //private IServiceProvider _provider { get; }

        /// <summary>
        ///     Initializes and logs the bot in.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task InitializeAsync()
        {

            // These are our EventSetup, each time one of these is triggered it runs the corresponding method. Ie, the bot receives a PartnerMessage we run Event.MessageReceivedAsync
            //_client.Log += _event.LogAsync;
            _client.JoinedGuild += _event.JoinedGuildAsync;
            _client.LeftGuild += _event.LeftGuildAsync;
            
            //_client.ReactionAdded += _event.ReactionAddedAsync;　//<-使わない
            _client.MessageReceived += _event.MessageReceivedAsync;

            //_client.UserJoined += user => Events.UserJoinedAsync(_provider.GetRequiredService<DatabaseHandler>().Execute<GuildModel>(DatabaseHandler.Operation.LOAD, null, user.Guild.Id), user);
            //_client.UserLeft += user => Events.UserLeftAsync(_provider.GetRequiredService<DatabaseHandler>().Execute<GuildModel>(DatabaseHandler.Operation.LOAD, null, user.Guild.Id), user);

            //↑2つは独自の処理を加える必要がありそう
        }
    }
}