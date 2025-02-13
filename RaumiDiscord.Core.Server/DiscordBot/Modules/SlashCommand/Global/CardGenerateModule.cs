using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class CardGenerateModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("名刺", "名刺を出力します。この機能で出力したものはあくまで交友として使ってください。")]
        public async Task CardGenerate()
        {
            await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            throw new NotImplementedException();
        }
    }
}
