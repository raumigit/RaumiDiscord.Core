using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class BookmarkModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("hoyocode", "HoYoverseで使えるギフトコードを出力します")]
        public async Task HoYoCode()
        {
            throw new NotImplementedException();
        }
    }
}
