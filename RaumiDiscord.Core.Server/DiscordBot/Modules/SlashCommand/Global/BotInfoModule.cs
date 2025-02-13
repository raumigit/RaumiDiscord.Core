using Discord.Commands;
using Discord.Interactions;
using System.ComponentModel;
using System.Reflection.Metadata;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class BotInfoModule : InteractionModuleBase<SocketInteractionContext>
    {

        public InteractionService Commands { get; set; }

        private InteractionHandler handler;

        public BotInfoModule()
        {

        }


        [SlashCommand("ping", "pingをします")]
        public async Task PingCommand()
        {
            await RespondAsync($"DiscordGateway:{Context.Client.Latency}ms");
        }
    }
}
