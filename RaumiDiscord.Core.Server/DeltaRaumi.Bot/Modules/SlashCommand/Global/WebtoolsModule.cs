using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    /// <summary>
    /// WebtoolsModuleは、Webダッシュボードに関するコマンドを提供します。
    /// </summary>
    public class WebtoolsModule : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// Webダッシュボードへ案内されます。(未実装)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [SlashCommand("webtools", "Webダッシュボードへ案内されます。(未実装)")]
        public async Task WebDashbordLink()
        {
            await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            throw new NotImplementedException();
        }
    }
}
