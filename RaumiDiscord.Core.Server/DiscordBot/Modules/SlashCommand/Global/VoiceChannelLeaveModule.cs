using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class VoiceChannelLeaveModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("leave", "BOTをVCから切断します")]
        public async Task BotLeave()
        {
            throw new NotImplementedException();
        }
    }
}
