using Discord.Interactions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using System.Runtime.InteropServices;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Modules.SlashCommand.Global
{
    /// <summary>
    /// ユーザーに関する操作を行うモジュールです。
    /// </summary>

    [Group("user", "ユーザーに関する操作が行えます。")]
    public class UserServiceModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private readonly ImprovedLoggingService _logger;
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


        //public class user 
        //{
        /// <summary>
        /// ユーザーのメインアカウントとサブアカウントをリンクするための認証キーを生成します。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [SlashCommand("link", "親アカウント用の認証キーを生成します")]
        public async Task LinkToken([Summary("code", "認証コード")] string code = null)
        {
            var userId = Context.User.Id;

            // サブアカウントがコードを使ってリンクする場合
            if (!string.IsNullOrEmpty(code))
            {
                if (!Guid.TryParse(code, out var tokenGuid))
                {
                    await RespondAsync("無効なコード形式です。", ephemeral: true);
                    return;
                }

                var token = _deltaRaumiDb.Components.FirstOrDefault(a => a.LinkCode == tokenGuid.ToString());

                if (token == null)
                {
                    await RespondAsync("このコードは使用できません。", ephemeral: true);
                    return;
                }

                if (token.TimeToLive < DateTime.UtcNow)
                {
                    await RespondAsync("このコードは期限切れです。", ephemeral: true);
                    return;
                }

                if (token.OwnerId == userId.ToString())
                {
                    await RespondAsync("自分自身をリンクすることはできません。", ephemeral: true);
                    return;
                }

                var alreadyLinked = _deltaRaumiDb.LinkedAccount.Any(x => x.SubUserId == userId.ToString());
                if (alreadyLinked)
                {
                    await RespondAsync("このアカウントはすでにリンクされています。", ephemeral: true);
                    return;
                }

                var linkedCount = _deltaRaumiDb.LinkedAccount.Count(x => x.ParentUserId == userId.ToString());
                if (linkedCount >= 3)
                {
                    await RespondAsync("この親アカウントはすでに3つのサブアカウントにリンクされています。", ephemeral: true);
                    return;
                }

                _deltaRaumiDb.LinkedAccount.Add(new LinkedAccountModel
                {
                    Id = Guid.NewGuid(),
                    ParentUserId = token.OwnerId,
                    SubUserId = userId.ToString(),
                });

                _deltaRaumiDb.Components.Remove(token);

                await _deltaRaumiDb.SaveChangesAsync();

                await RespondAsync("アカウントのリンクに成功しました。", ephemeral: true);
            }
            // 親アカウントが認証コードを生成する場合
            else
            {
                // サブアカウントがコード生成しようとした場合は拒否
                var isSub = _deltaRaumiDb.LinkedAccount.Any(x => x.SubUserId == userId.ToString());
                if (isSub)
                {
                    await RespondAsync("サブアカウントは認証コードを生成できません。", ephemeral: true);
                    return;
                }

                DiscordComponentModel model = new DiscordComponentModel();

                try
                {
                    // 既存の有効なトークンがあれば再利用
                    var existing = _deltaRaumiDb.Components
                        .FirstOrDefault(l => l.OwnerId == userId.ToString());
                    if (existing != null)
                    {
                        if (existing.TimeToLive < DateTime.UtcNow)
                        {
                            existing.TimeToLive = DateTime.UtcNow.AddMinutes(15);
                            _deltaRaumiDb.Components.Update(existing);
                            await _deltaRaumiDb.SaveChangesAsync();
                        }
                    }
                    
                    var token = existing;
                    if (token == null)
                    {
                        model.CustomId = Guid.NewGuid();
                        model.CreateTime = DateTime.Now;
                        model.OwnerId = userId.ToString();
                        model.DeltaRaumiComponentType = "LinkToken";
                        model.LinkCode = Guid.NewGuid().ToString();
                        model.TimeToLive = DateTime.UtcNow.AddMinutes(15);
                    }

                    if (existing == null)
                    {
                        _deltaRaumiDb.Components.Add(model);
                        await _deltaRaumiDb.SaveChangesAsync();
                    }
                    var linkcode = model.LinkCode ?? token.LinkCode ?? "undefined"; //実行の仕組み上tokenを先に持ってくるとnullになり失敗する。
                    await RespondAsync($"このコードをサブアカウントで使用してください:\n`/user link code:{linkcode}`", ephemeral: true);
                }
                catch (Exception e)
                {
                    await RespondAsync($"{e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}

