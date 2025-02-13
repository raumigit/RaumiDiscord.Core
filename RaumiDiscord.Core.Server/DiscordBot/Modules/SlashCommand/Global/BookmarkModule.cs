using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class BookmarkModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DeltaRaumiDbContext deltaRaumiDb;
        public BookmarkModule(DeltaRaumiDbContext deltaRaumiDb)
        {
            this.deltaRaumiDb = deltaRaumiDb;
        }

        [SlashCommand("hoyocode", "HoYoverseで使えるギフトコードを出力します")]
        public async Task HoYoCode(
            [Summary("Get","有効なコードを出力します。Setを使えばURLを共有できます。")]
            [Choice("Get","get")]
            [Choice("Set","set")]
            string action,
            [Summary("type","URLのタイプを指定")]
            [Choice("URL","url")]
            [Choice("GenshinImpact","GI")]
            [Choice("HonkaiSterrail","HSR")]
            [Choice("ZenlessZoneZero","ZZZ")]
            string urlType,
            string url = null,
            string ttl = null)
        {
            if (action == "set")
            {
                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(ttl))
                {
                    await RespondAsync("URLと有効期限を指定してください。(有効期限：yyyy/mm/dd-hh:mm:sszzz)", ephemeral: true);
                    return;
                }
                if (!DateTimeOffset.TryParseExact(ttl, "yyyy/MM/dd-HH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset expirationTime))
                {
                    await RespondAsync("TTLの形式が正しくありません。yyyy/MM/dd-HH:mm:sszzz 形式で入力してください。", ephemeral: true);
                    return;
                }

                

                // UnixTime 形式に変換
                long unixExpirationTime = expirationTime.ToUnixTimeSeconds();

                var newEntry = new UrlDetaModel
                {
                    Url = url,
                    UrlType = urlType,
                    TTL = expirationTime.UtcDateTime
                };
                deltaRaumiDb.UrlDetaModels.Add(newEntry);
                await deltaRaumiDb.SaveChangesAsync();
                await RespondAsync($"登録完了: {urlType} - {url} (期限: <t:{unixExpirationTime}:R>)");
            }
            else if (action == "get")
            {
                var now = DateTime.UtcNow;
                var results = await deltaRaumiDb.UrlDetaModels
                    .Where(u => u.UrlType == urlType && u.TTL > now)
                    .Select(u => u.Url)
                    .ToListAsync();

                if (results.Count == 0)
                {
                    await RespondAsync("有効なURLが見つかりませんでした。", ephemeral: true);
                    return;
                }

                await RespondAsync(string.Join("\n", results));
            }
            
        }
    }
}
