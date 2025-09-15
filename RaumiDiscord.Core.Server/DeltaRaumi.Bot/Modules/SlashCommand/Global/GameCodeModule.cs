using System.Text.RegularExpressions;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;



namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Modules.SlashCommand.Global;

/// <summary>
/// BookmarkModuleは、URLの登録と取得を行うモジュールです。
/// </summary>
public class GameCodeModule : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// BookmarkModuleは、URLの登録と取得を行うモジュールです。
    /// </summary>
    private readonly ImprovedLoggingService _loggingService;
    private readonly DeltaRaumiDbContext _deltaRaumiDb;
    private GameMetaService _gameMetaService;

    /// <summary>
    /// BookmarkModuleのコンストラクタ
    /// </summary>
    /// <param name="deltaRaumiDb"></param>
    /// <param name="logger"></param>
    /// <param name="gameMetaService"></param>
    public GameCodeModule(DeltaRaumiDbContext deltaRaumiDb, ImprovedLoggingService logger, GameMetaService gameMetaService)
    {
        _deltaRaumiDb = deltaRaumiDb;
        _loggingService = logger;
        _gameMetaService = gameMetaService;
    }

    /// <summary>
    /// HoYoverseのギフトコードを登録・取得するコマンド
    /// </summary>
    /// <param name="action"></param>
    /// <param name="urlType"></param>
    /// <param name="url"></param>
    /// <param name="ttl"></param>
    /// <param name="publishAttri"></param>
    /// <returns></returns>
    [SlashCommand("GameCode", "ゲームで使えるギフトコードを出力します")]
    public async Task GameCode(
        [Summary("action","Get:有効なコードを出力します。Set:URLを共有できます。")]
        [Choice("Get","get")]
        [Choice("Set","set")]
        string action,
        [Summary("type","URLまたはゲームのタイプを指定")]
        int urlType,
        string? url = null,
        [Summary("ttl","有効期限を設定します。yyyy/MM/dd-HH:mm:sszzz形式で入力してください。")]
        string? ttl = null,
        [Summary("publish","コードの公開を指定します。")]
        [Choice("false",0)]
        [Choice("true",1)]
        bool publishAttri=false)
    {
        var meta = _gameMetaService.GetGame(urlType);


        if (action == "set")
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(ttl))
            {
                await RespondAsync("URLと有効期限を指定してください。(有効期限：yyyy/MM/dd-HH:mm:sszzz)", ephemeral: true);
                return;
            }
            if (!DateTimeOffset.TryParseExact(ttl, "yyyy/MM/dd-HH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset expirationTime) && expirationTime.UtcDateTime <= DateTime.UtcNow)
            {
                await RespondAsync("TTLの形式が正しくありません。yyyy/MM/dd-HH:mm:sszzz 形式で入力してください。\n" +
                                   "または有効ではない時間を入力している可能性があります。", ephemeral: true);
                return;
            }
            if (urlType == 0)
            {
                if (!url.StartsWith("https://") && !url.StartsWith("http://"))
                {
                    await RespondAsync("URLが正しくありません。", ephemeral: true);
                    return;
                }
            }

            if (meta.BaseUrl != null && url.StartsWith(meta.BaseUrl))
            {
                Console.WriteLine($"URL->{url}");
            }

            if (meta.BaseUrl != null && Regex.IsMatch(url ?? "", "^[A-Za-z0-9]+$"))
            {
                url = meta.BaseUrl + url;
                publishAttri = true; // コードの場合は強制公開
                await _loggingService.Log($"URLがCodeだったため修正しました。", "GameCode");
            }
            // UnixTime 形式に変換
            long unixExpirationTime = expirationTime.ToUnixTimeSeconds();

            var newEntry = new UrlDataModel
            {
                Url = url,
                UrlType = urlType,
                DiscordUser = Context.User.Id.ToString(),
                Ttl = expirationTime.UtcDateTime,
                Publish = publishAttri
            };

            var urlRecord = _deltaRaumiDb.UrlDataModels.Where(k => k.Url == url).ToList();
            if (urlRecord.Any())
            {
                await RespondAsync("既に登録されています。", ephemeral: true);
                await _loggingService.Log($"登録済みのコード：{urlRecord}", "GameCode");
                return;
            }
            _deltaRaumiDb.UrlDataModels.Add(newEntry);
            await _deltaRaumiDb.SaveChangesAsync();
            await RespondAsync($"登録完了: {urlType} - {url} (期限: <t:{unixExpirationTime}:R>) 登録者：<@{Context.User.Id.ToString()}>");
            await _loggingService.Log($"登録されたコード：{url}", "GameCode");
        }
        else if (action == "get")
        {
            var now = DateTime.UtcNow;
            List<string> results;

            if (urlType == 0)
            {
                results = await _deltaRaumiDb.UrlDataModels
                    .Where(u => u.UrlType == urlType && u.Ttl > now && u.DiscordUser == Context.User.Id.ToString() && u.Publish == true)
                    .Select(u => $"{u.Url}")
                    .ToListAsync();
            }
            else
            {
                results = await _deltaRaumiDb.UrlDataModels
                    .Where(u => u.UrlType == urlType && u.Ttl > now)
                    .Select(u => $"{u.Url}")
                    .ToListAsync();
            }

            if (results.Count == 0)
            {
                await RespondAsync("有効なURLが見つかりませんでした。", ephemeral: true);
                return;
            }
            await RespondAsync(string.Join("\n", results), ephemeral: true);
        }
    }
}