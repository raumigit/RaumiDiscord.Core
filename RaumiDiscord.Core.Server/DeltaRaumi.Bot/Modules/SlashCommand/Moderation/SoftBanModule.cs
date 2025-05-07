using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Modules.SlashCommand.Moderation
{
    public class SoftBanModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("softban", "Kicking a user deleting the last x days of messages.")]
        public async Task SoftBanAsync(
        [Summary("member", "ソフトBANを行うメンバー."), DoHierarchyCheck]
        SocketGuildUser member,
        [Summary("reason", "理由")]
        string reason,
        [Summary("days_to_delete", "削除する日数"),
            Choice("1", 1),
            Choice("3", 3),
            Choice("7", 7)
        ]
        int daysToDelete = 7)
        {
            var e = (
                $"「{Format.Bold(Context.Guild.Name)}から {Format.Bold(reason)}によりソフトバンされました。」" +
                $"\nソフトバンとは、単にあなたがキックされ、一定量の以前のメッセージが大量に削除されることを意味します。" +
                $"まだ再参加できます。");

            //if (!await member.TrySendMessageAsync(embed: e.Apply(GetData()).Build()))
            //    Warn(LogSource.Module, $"{member} にメッセージを送信しようとしたときに 403 が発生しました!");

            try
            {
                await member.BanAsync(daysToDelete, reason);
                await Context.Guild.RemoveBanAsync(member.Id);

                //return Ok($"Successfully softbanned **{member}**.", () =>
                //    ModService.OnModActionCompleteAsync(ModActionEventArgs
                //        .FromModule(this)
                //        .WithActionType(ModActionType.Softban)
                //        .WithTarget(member)
                //        .WithReason(reason)));
            }
            catch
            {
                await ReplyAsync("そのメンバーをソフトバンする際にエラーが発生しました。権限不足かそのメンバーはロールが自身より上位にいますか？");
            }
        }
        
    }
}
