using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class VoiceChannelLeaveModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("leave", "BOTをVCから切断します")]
        public async Task BotLeave()
        {
            await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            throw new NotImplementedException();
        }
    }
}
