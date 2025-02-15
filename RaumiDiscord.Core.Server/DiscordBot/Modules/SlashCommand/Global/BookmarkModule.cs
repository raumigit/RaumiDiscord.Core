using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;
using System.Linq;
using System.Text.RegularExpressions;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class BookmarkModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LoggingService LoggingService;
        private readonly DeltaRaumiDbContext deltaRaumiDb;
        public BookmarkModule(DeltaRaumiDbContext deltaRaumiDb,LoggingService logger)
        {
            this.deltaRaumiDb = deltaRaumiDb;
            this.LoggingService = logger;
        }

        [SlashCommand("hoyocode", "HoYoverseで使えるギフトコードを出力します")]
        public async Task HoYoCode(
            [Summary("Get","有効なコードを出力します。Setを使えばURLを共有できます。")]
            [Choice("Get","get")]
            [Choice("Set","set")]
            string action,
            [Summary("type","URLのタイプを指定")]
            [Choice("URL","URL")]
            [Choice("GenshinImpact","GI")]
            [Choice("HonkaiStarRail","HSR")]
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
                if (!DateTimeOffset.TryParseExact(ttl, "yyyy/MM/dd-HH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset expirationTime) && expirationTime.UtcDateTime <= DateTime.UtcNow)
                {
                    await RespondAsync("TTLの形式が正しくありません。yyyy/MM/dd-HH:mm:sszzz 形式で入力してください。\n" +
                        "または有効ではない時間を入力している可能性があります。", ephemeral: true);
                    return;
                }
                if (urlType == "URL")
                {
                    if (!url.StartsWith("https://") && !url.StartsWith("http://"))
                    {
                        await RespondAsync("URLが正しくありません。", ephemeral: true);
                        return;
                    }
                }
                else if (urlType == "GI" || urlType == "HSR" || urlType == "ZZZ")
                {
                    string baseUrl = urlType switch
                    {
                        "GI" => "https://genshin.hoyoverse.com/ja/gift?code=",
                        "HSR" => "https://hsr.hoyoverse.com/gift?code=",
                        "ZZZ" => "https://zzz.hoyoverse.com/gift?code=",
                        _ => ""
                    };

                    if (url.StartsWith(baseUrl))
                    {
                        Console.WriteLine($"URL->{url}");
                    }

                    if (Regex.IsMatch(url, "^[A-Z0-9]+$"))
                    {
                        url = baseUrl + url;
                    }
                    await LoggingService.LogGeneral($"URLがCodeだったため修正しました。");
                }
                



                // UnixTime 形式に変換
                long unixExpirationTime = expirationTime.ToUnixTimeSeconds();

                var newEntry = new UrlDetaModel
                {
                    Url = url,
                    UrlType = urlType,
                    TTL = expirationTime.UtcDateTime
                };

                var Url_record = deltaRaumiDb.UrlDetaModels.Where(k => k.Url == url).ToList();
                if (Url_record.Any())
                {
                    await RespondAsync("既に登録されています。",ephemeral:true);
                    return;
                }
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
