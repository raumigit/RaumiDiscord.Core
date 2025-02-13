using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class WebtoolsModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("webtools", "Webダッシュボードへ案内されます。(未実装)")]
        public async Task WebDashbordLink()
        {
            throw new NotImplementedException();
        }
    }
}
