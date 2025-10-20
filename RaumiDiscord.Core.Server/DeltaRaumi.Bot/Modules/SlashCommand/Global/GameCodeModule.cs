using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
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
    /// <param name="publish"></param>
    /// <returns></returns>
    [SlashCommand("gamemode", "ゲームで使えるギフトコードを出力します(メンテナンス中)")]
    public async Task GameCode(
        [Summary("action", "実行するアクション")]
    [Choice("Get","Get")]
    [Choice("Set","Set")]
    [Choice("Help","Help")]
    string action,
            [Summary("urlType", "URLのタイプ")] string? urlType = null,
            [Summary("url", "URL又はコード")] string? url = null,
            [Summary("ttl", "有効期限 (例: 2025/9/10-3:34:00+9:00)")] string? ttl = null,
            [Summary("publish", "公開するかどうか")] bool? publish = false)
    {
        try
        {
            switch (action)
            {
                case "Get":
                    await HandleGetAction(urlType);
                    break;
                case "Set":
                    await HandleSetAction(urlType, url, ttl, publish);
                    break;
                case "Help":
                    await HandleHelpAction(urlType);
                    break;
            }
        }
        catch (Exception ex)
        {
            await RespondAsync($"エラーが発生しました: {ex.Message}", ephemeral: true);
        }
    }

    private async Task HandleHelpAction(string? urlType)
    {
        string helptemplate;
        helptemplate = "使い方:\n" +
                   "/GameCode Get type:Url - 自分が登録した有効なURLを取得します。\n" +
                   "/GameCode Get type:Code - 有効なコードを取得します。\n" +
                   "/GameCode Set type:Url url:<URL> ttl:<有効期限> publish:<true/false> - URLを登録します。\n" +
                   "/GameCode Set type:Code url:<コード> ttl:<有効期限> publish:<true/false> - コードを登録します。\n\n" +
                   "注意点:\n" +
                   "・URLはhttps://またはhttp://で始まる必要があります。(type:Urlの場合)\n" +
                   "・コードは英数字のみで構成されている必要があります。(type:Codeの場合)\n" +
                   "・有効期限は締め切りの日時で、yyyy/MM/dd-HH:mm:sszzz形式で入力してください。(例：2025/9/10-3:34:00+9:00)\n" +
                   "・コードの公開設定は、type:Codeの場合は強制的にtrueになります。\n" +
                   "・コードの登録時に有効期限の入力は慎重に行ってください。間違えると原則編集も同じコードの作り直しもできません。\n" +
                   "・オフラインイベントなどで所得できる限定のコードを無断で登録しないでください。発覚した場合、削除および利用停止措置を取ることがあります。\n";
        string helpUrlType;
        helpUrlType = "typeの種類:\n" +
                        "- Url: URLを指定します。\n" +
                        "- Code: ゲームコードを指定します。\n";

        if (string.IsNullOrEmpty(urlType))
        {
            var helpText = "**GameCodeコマンドの使い方**\n\n" +
                          "**Get**: `/GameCode action:Get [urlType:タイプ]`\n" +
                          "有効なコードを取得します。urlTypeを指定すると絞り込めます。\n\n" +
                          "**Set**: `/GameCode action:Set urlType:タイプ url:URL ttl:期限 [publish:公開]`\n" +
                          "新しいコードを登録します。\n\n" +
                          "**Help**: `/GameCode action:Help [urlType:タイプ]`\n" +
                          "ヘルプを表示します。urlTypeを指定すると詳細を確認できます。";

            await RespondAsync(helpText, ephemeral: true);
            return;
        }

        var gameConfig = _gameMetaService.FindGame(urlType);
        if (gameConfig != null)
        {
            await RespondAsync(
                $"このUrlTypeは **{gameConfig.Name}** または **{gameConfig.Shortname}** で指定できます。",
                ephemeral: true);
        }
        else
        {
            var allGames = _gameMetaService.GetAllGames();
            var gameList = string.Join("\n", allGames.Select(g => $"• {g.Name}"));
            await RespondAsync(
                $"現在登録できるUrlTypeは以下のとおりです。\n{gameList}",
                ephemeral: true);
        }
    }

    private async Task HandleSetAction(string? urlType, string? url, string? ttl, bool? publish)
    {
        if (string.IsNullOrEmpty(urlType) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(ttl))
        {
            await RespondAsync("urlType、url、ttlは必須です。", ephemeral: true);
            return;
        }
        var gameConfig = _gameMetaService.FindGame(urlType);
        if (gameConfig == null)
        {
            await RespondAsync($"指定されたUrlType `{urlType}` は存在しません。", ephemeral: true);
            return;
        }

        if (!url.StartsWith("https://") && !url.StartsWith("http://"))
        {
            if (!string.IsNullOrEmpty(gameConfig.BaseUrl) && IsAlphanumeric(url))
            {
                url = gameConfig.BaseUrl + url;
            }
            else
            {
                await RespondAsync("URLはhttps://またはhttp://で始まる必要があります。", ephemeral: true);
                return;
            }
        }

        if (!TryParseDateTimeWithTimezone(ttl, out var parsedTtl))
        {
            await RespondAsync("ttlの形式が正しくありません。例: 2025/9/10-3:34:00+9:00", ephemeral: true);
            return;
        }

        if (parsedTtl <= DateTime.UtcNow)
        {
            await RespondAsync("有効期限は現在時刻より後である必要があります。", ephemeral: true);
            return;
        }

        var existingCode = await _deltaRaumiDb.UrlDataModels
            .FirstOrDefaultAsync(u => u.Url == url);

        if (existingCode != null)
        {
            await RespondAsync("このコードは既に登録されています。", ephemeral: true);
            return;
        }

        var shouldPublish = publish ?? (gameConfig.Name != "Url");

        var newCode = new UrlDataModel
        {
            Url = url,
            UrlType = gameConfig.Name,
            DiscordUser = Context.User.Id.ToString(),
            Ttl = parsedTtl,
            Publish = shouldPublish
        };

        _deltaRaumiDb.UrlDataModels.Add(newCode);
        await _deltaRaumiDb.SaveChangesAsync();

        var unixTime = new DateTimeOffset(parsedTtl).ToUnixTimeSeconds();
        var storeLinks = BuildStoreLinks(gameConfig);

        var response = $"登録完了：{gameConfig.Name} - {url}\n" +
                      $"期限：<t:{unixTime}:R>\n" +
                      $"登録者：<@{Context.User.Id}>";

        if (!string.IsNullOrEmpty(storeLinks))
        {
            response += $"\n{storeLinks}";
        }

        await RespondAsync(response, ephemeral: true);
    }

    private async Task HandleGetAction(string? urlType)
    {
        var now = DateTime.UtcNow;
        var userId = Context.User.Id.ToString();

        IQueryable<UrlDataModel> query = _deltaRaumiDb.UrlDataModels
            .Where(u => u.Ttl > now);

        if (!string.IsNullOrEmpty(urlType))
        {
            var gameConfig = _gameMetaService.FindGame(urlType);
            if (gameConfig == null)
            {
                await RespondAsync($"指定されたUrlType `{urlType}` は存在しません。", ephemeral: true);
                return;
            }

            query = query.Where(u => u.UrlType == gameConfig.Name);
        }

        var codes = await query
            .Where(u => u.Publish || u.DiscordUser == userId)
            .OrderBy(u => u.Ttl)
            .ToListAsync();

        if (!codes.Any())
        {
            await RespondAsync("現在利用可能なコードはありません。", ephemeral: true);
            return;
        }

        var response = string.Join("\n\n", codes.Select(code =>
        {
            var gameConfig = _gameMetaService.FindGame(code.UrlType);
            var unixTime = new DateTimeOffset(code.Ttl).ToUnixTimeSeconds();
            var storeLinks = BuildStoreLinks(gameConfig);

            return $"**{code.UrlType}** - {code.Url}\n" +
                   $"期限：<t:{unixTime}:R>\n" +
                   $"登録者：<@{code.DiscordUser}>" +
                   (string.IsNullOrEmpty(storeLinks) ? "" : $"\n{storeLinks}");
        }));

        await RespondAsync(response, ephemeral: true);
    }

    private string BuildStoreLinks(GameConfig? gameConfig)
    {
        if (gameConfig?.Store == null) return string.Empty;

        var links = new List<string>();
        if (!string.IsNullOrEmpty(gameConfig.Store.Google))
            links.Add($"[Google Play]({gameConfig.Store.Google})");
        if (!string.IsNullOrEmpty(gameConfig.Store.Apple))
            links.Add($"[App Store]({gameConfig.Store.Apple})");

        return links.Any() ? "アプリストア\n" + string.Join(" ", links) : string.Empty;
    }

    private bool IsAlphanumeric(string str)
    {
        return str.All(char.IsLetterOrDigit);
    }

    private bool TryParseDateTimeWithTimezone(string input, out DateTime utcDateTime)
    {
        utcDateTime = DateTime.MinValue;

        try
        {
            var formats = new[]
            {
                    "yyyy/M/d-H:mm:sszzz",
                    "yyyy/M/d-H:mm:ss zzz",
                    "yyyy-M-d-H:mm:sszzz",
                    "yyyy-M-d-H:mm:ss zzz"
                };

            if (DateTimeOffset.TryParseExact(input, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateTimeOffset))
            {
                utcDateTime = dateTimeOffset.UtcDateTime;
                return true;
            }
        }
        catch { }

        return false;
    }
}


    // 保守用に保留中
    //{
        

    //    //実行しないでください

    //    if(action == "help")
    //    {
            

    //    }
    //    else if (action == "set")
    //    {
            


    //        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(ttl))
    //        {
    //            await RespondAsync("URLと有効期限を指定してください。(有効期限：yyyy/MM/dd-HH:mm:sszzz)", ephemeral: true);
    //            return;
    //        }
    //        if (!DateTimeOffset.TryParseExact(ttl, "yyyy/MM/dd-HH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset expirationTime) && expirationTime.UtcDateTime <= DateTime.UtcNow)
    //        {
    //            await RespondAsync("TTLの形式が正しくありません。yyyy/MM/dd-HH:mm:sszzz 形式で入力してください。\n" +
    //                               "または有効ではない時間を入力している可能性があります。", ephemeral: true);
    //            return;
    //        }
    //        if (urlType == "Url")
    //        {
    //            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
    //            {
    //                await RespondAsync("URLが正しくありません。", ephemeral: true);
    //                return;
    //            }
    //        }

    //        if (meta.BaseUrl != null && url.StartsWith(meta.BaseUrl))
    //        {
    //            Console.WriteLine($"URL->{url}");
    //        }

    //        if (meta.BaseUrl != null && Regex.IsMatch(url ?? "", "^[A-Za-z0-9]+$"))
    //        {
    //            url = meta.BaseUrl + url;
    //            publishAttri = true; // コードの場合は強制公開
                //await _loggingService.Log($"URLがCodeだったため修正しました。", "GameCode");
    //        }
    //        // UnixTime 形式に変換
    //        long unixExpirationTime = expirationTime.ToUnixTimeSeconds();

    //        var newEntry = new UrlDataModel
    //        {
    //            Url = url,
    //            UrlType = urlType,
    //            DiscordUser = Context.User.Id.ToString(),
    //            Ttl = expirationTime.UtcDateTime,
    //            Publish = publishAttri
    //        };

    //        var urlRecord = _deltaRaumiDb.UrlDataModels.Where(k => k.Url == url).ToList();
    //        if (urlRecord.Any())
    //        {
    //            await RespondAsync("既に登録されています。", ephemeral: true);
    //            await _loggingService.Log($"登録済みのコード：{urlRecord}", "GameCode");
    //            return;
    //        }
    //        _deltaRaumiDb.UrlDataModels.Add(newEntry);
    //        await _deltaRaumiDb.SaveChangesAsync();
    //        await RespondAsync($"登録完了: {urlType} - {url} (期限: <t:{unixExpirationTime}:R>) 登録者：<@{Context.User.Id.ToString()}>");
    //        await _loggingService.Log($"登録されたコード：{url}", "GameCode");
    //    }
    //    else if (action == "get")
    //    {
    //        var now = DateTime.UtcNow;
    //        List<string> results;

    //        if (meta == null)
    //            await Err();

    //        if (urlType == "Url")
    //        {
    //            results = await _deltaRaumiDb.UrlDataModels
    //                .Where(u => u.UrlType == urlType && u.Ttl > now && u.DiscordUser == Context.User.Id.ToString() && u.Publish == true)
    //                .Select(u => $"{u.Url}")
    //                .ToListAsync();
    //        }
    //        else
    //        {
    //            results = await _deltaRaumiDb.UrlDataModels
    //                .Where(u => u.UrlType == urlType && u.Ttl > now)
    //                .Select(u => $"{u.Url}")
    //                .ToListAsync();
    //        }

    //        if (results.Count == 0)
    //        {
    //            await RespondAsync("有効なURLが見つかりませんでした。", ephemeral: true);
    //            return;
    //        }
    //        await RespondAsync(string.Join("\n", results), ephemeral: true);
    //    }
    //}

    //private static string urlget()
    //{
    //    string s = "Url";
    //    return s;
    //}

    //private async Task Err() 
    //{
    //    await RespondAsync("サーバーに問題が発生しているため続行できません。管理者に問い合わせてください。", ephemeral: true);
    //    await _loggingService.Log($"不正な動作：GameCodeのJsonがNullです。", "GameCode",ImprovedLoggingService.LogLevel.Error);
    //    return;
    //}
//}