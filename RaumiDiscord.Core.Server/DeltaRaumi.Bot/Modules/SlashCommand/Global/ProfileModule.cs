using Discord;
using Discord.Interactions;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Modules.SlashCommand.Global;

/// <summary>
/// ProfileModuleは、ユーザーのプロフィールに関する操作を提供します。
/// </summary>
/// 
[Group("profile", "プロフィールに関する操作が行えます。")]
public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DeltaRaumiDbContext _deltaRaumiDb;
    private readonly ImprovedLoggingService _logger;

    /// <summary>
    /// ProfileModuleのコンストラクタ
    /// </summary>
    /// <param name="deltaRaumiDb"></param>
    /// <param name="logger"></param>
    public ProfileModule(DeltaRaumiDbContext deltaRaumiDb, ImprovedLoggingService logger)
    {
        _deltaRaumiDb = deltaRaumiDb;
        _logger = logger;
    }
    /// <summary>
    /// Profileは、ユーザーのプロフィールに関する操作を提供します。
    /// </summary>
    public bool FileUpload = true;

    /// <summary>
    /// /// プロフィールカードを生成します。
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    [SlashCommand("generate", "プロフィールカードを出力します。コメントをオプションで追加できます。")]
    public async Task Generate([Summary("Coment", "自由にコメントをいれることができます。")] string? comment = null)
    {
        // string background = @".\Assets\Image\default_bg.png";
        // string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
        // string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
        string outputPath = @$".\Tests\namecard\{Context.User.Id}.png";
        string discordname = Context.User.Username;
        // string profileImageFlame = @".\Assets\Image\sampleflame.png";

        string avatarUrl = (Context.User as IGuildUser)?.GetGuildAvatarUrl()
                           ?? Context.User.GetAvatarUrl()
                           ?? Context.User.GetDefaultAvatarUrl();

        string avatarPath = $@".\Temp\Discordicon\128\{Context.User.Id}.png";

        int exp; // 仮の経験値

        Directory.CreateDirectory(@".\Temp\Discordicon\128");
        Directory.CreateDirectory(@".\Tests\namecard");

        await DeferAsync();

        //using (HttpClient client = new HttpClient())
        //{
        //    byte[] imageBytes = await client.GetByteArrayAsync(avatarUrl);
        //    await File.WriteAllBytesAsync(avatarPath, imageBytes);
        //}

        try
        {
            var userGuilddata = _deltaRaumiDb.UserGuildData.Where(d => d.UserId == Context.User.Id.ToString() && d.GuildId == Context.Guild.Id.ToString()).FirstOrDefault();
            exp =userGuilddata.GuildExp;


            await ImageGenerator.UserCardGenerater(Context.Guild.Name, Context.User.Id.ToString(), discordname, avatarPath, avatarUrl, comment, exp);

            if (FileUpload)
            {
                await FollowupWithFileAsync(outputPath, text: "生成が完了しました。");
            }
            else
            {
                await FollowupAsync("GeneratedServer...", ephemeral: true);
            }
        }
        catch (Exception ex)
        {

            await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```", ephemeral: true);
        }
    }

    /// <summary>
    /// プロフィールの背景をデフォルトの画像に変更します。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [SlashCommand("default-bg", "背景をデフォルトの画像へ変更します。")]
    public async Task DefaultBg()
    {
        await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
    }

    /// <summary>
    /// プロフィールの背景を指定したURLの画像に変更します。
    /// </summary>
    /// <param name="backgraundUrl"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [SlashCommand("set-bg", "背景をコマンド経由で設定します。")]
    public async Task SetBg([Summary("BackgraundUrl", "Urlで背景画像を設定することができます。")] string backgraundUrl)
    {
        await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
    }

    /// <summary>
    /// プロフィールの詳細な編集がWebから行えます。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [SlashCommand("edit-web", "Webからプロフィールの詳細な編集ができます。")]
    public async Task EditWeb()
    {
        await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
    }
}


/*

[SlashCommand("example", "hotcocoa")]
[Summary("mino")]
public async Task exsample()
{
    await FollowupAsync("実装までお待ち下さい。");
    throw new NotImplementedException();
}


*/
