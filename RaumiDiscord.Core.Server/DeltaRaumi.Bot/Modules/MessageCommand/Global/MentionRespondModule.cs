using Discord;
using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.MessageCommand
{
    public class MentionRespondModule
    {
        [MessageCommand("@Raumi#1195")]
        public async Task MentiononlyCommand(IMessage message)
        {
            string contentbase = "@Raumi#1195 *";
            switch (message.CleanContent)
            {
                case "@Raumi#1195":
                    await message.Channel.SendMessageAsync("なに...？");
                    break;

                case string match when System.Text.RegularExpressions.Regex.IsMatch(message.CleanContent, contentbase):

                    await message.Channel.SendMessageAsync("該当するメッセージコマンドはないっぽい…");
                    break;
                default:
                    break;
            }
        }
    }
}
