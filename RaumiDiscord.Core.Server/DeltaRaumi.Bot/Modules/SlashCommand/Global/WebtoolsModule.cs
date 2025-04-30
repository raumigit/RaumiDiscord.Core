using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class WebtoolsModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("webtools", "Webダッシュボードへ案内されます。(未実装)")]
        public async Task WebDashbordLink()
        {
            await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            throw new NotImplementedException();
        }
    }
}
