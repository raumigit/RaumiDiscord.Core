using Discord;
using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    public class DeltaRaumiHandler(
        DiscordSocketClient client,
        DeltaRaumiEventHandler events,
        IServiceProvider provider)
    {
        

        /// <summary>
        /// Gets the client.
        /// </summary>
        private DiscordSocketClient Client { get; } = client;

        /// <summary>
        /// Gets the event.
        /// </summary>
        private DeltaRaumiEventHandler Event { get; } = events;

        /// <summary>
        /// Gets the provider.
        /// </summary>
        private IServiceProvider Provider { get; } = provider;

        /// <summary>
        /// Initializes and logs the bot in.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task InitializeAsync()
        {
            // These are our EventSetup, each time one of these is triggered it runs the corresponding method. Ie, the bot receives a PartnerMessage we run Event.MessageReceivedAsync
            //_client.Log += _event.LogAsync;
            Client.JoinedGuild += Event.JoinedGuildAsync;
            Client.LeftGuild += Event.LeftGuildAsync;

            //_client.ReactionAdded += _event.ReactionAddedAsync;　//<-使わない
            Client.MessageReceived += Event.MessageReceivedAsync;
            Client.MessageUpdated += Event.MessageUpdated;
            Client.MessageDeleted += Event.MessageDeleted;

            return Task.CompletedTask;

            //_client.UserJoined += user => Events.UserJoinedAsync(_provider.GetRequiredService<DatabaseHandler>().Execute<GuildModel>(DatabaseHandler.Operation.LOAD, null, user.Guild.Id), user);
            //_client.UserLeft += user => Events.UserLeftAsync(_provider.GetRequiredService<DatabaseHandler>().Execute<GuildModel>(DatabaseHandler.Operation.LOAD, null, user.Guild.Id), user);

            //↑2つは独自の処理を加える必要がありそう
        }
    }
}