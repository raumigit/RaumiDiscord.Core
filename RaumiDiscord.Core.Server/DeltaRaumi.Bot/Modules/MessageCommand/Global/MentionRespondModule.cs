using Discord;
using Discord.Interactions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.MessageCommand
{
    /// <summary>
    /// MentionRespondModuleは、メンションに対する応答を行うモジュールです。
    /// </summary>
    public class MentionRespondModule
    {
        /// <summary>
        /// メンションコマンドに応じて、特定のメッセージを返します。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
