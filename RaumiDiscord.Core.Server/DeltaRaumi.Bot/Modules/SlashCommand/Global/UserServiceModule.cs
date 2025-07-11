using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Modules.SlashCommand.Global
{
    /// <summary>
    /// ユーザーに関する操作を行うモジュールです。
    /// </summary>
    public class UserServiceModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private ImprovedLoggingService _logger;
        /// <summary>
        /// ユーザーサービスモジュールのコンストラクター
        /// </summary>
        /// <param name="deltaRaumiDb"></param>
        /// <param name="logger"></param>
        public UserServiceModule(DeltaRaumiDbContext deltaRaumiDb, ImprovedLoggingService logger)
        {
            this._deltaRaumiDb = deltaRaumiDb;
            this._logger = logger;
        }

        [Group("user", "ユーザーに関する操作が行えます。")]
        public class user 
        {
            /// <summary>
            /// ユーザーのメインアカウントとサブアカウントをリンクするための認証キーを生成します。
            /// </summary>
            /// <param name="code"></param>
            /// <returns></returns>
            [SlashCommand("link", "親アカウント用の認証キーを生成します")]
            public async Task LinkToken([Summary("code", "認証コード")] string code)
            {
                /*
                var userId = Context.User.Id;
                // サブアカウントとして登録されているか確認
                if (_deltaRaumiDb.AccountLinkes.Any(x => x.SubUserId == userId))
                {
                    await RespondAsync("あなたは既に別のアカウントにリンクされています。認証キーは生成できません。", ephemeral: true);
                    return;
                }
                */
            }
        }
    }
}
