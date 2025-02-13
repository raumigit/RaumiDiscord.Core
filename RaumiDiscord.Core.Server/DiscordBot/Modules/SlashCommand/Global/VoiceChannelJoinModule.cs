using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class VoiceChannelJoinModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("join", "BOTをボイスチャンネルに呼び出す")]
        public async Task BotJoin()
        {
            await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            throw new NotImplementedException();
        }
    }
}
