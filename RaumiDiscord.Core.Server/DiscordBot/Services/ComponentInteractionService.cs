using Discord;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;
using RaumiDiscord.Core.Server.DiscordBot.Data;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    class ComponentInteractionService
    {
#nullable disable
        private readonly DiscordSocketClient Client;
        private readonly DeltaRaumiDbContext DeltaRaumiDbContext;
        private readonly LoggingService LoggingService;

        private Discord.Color RaumiMainColor = new Discord.Color(0x7bb3ee);
        private Discord.Color RaumiSubColor = new Discord.Color(0xf02443);

        public Configuration configuration { get; set; }

        public ComponentInteractionService(DiscordSocketClient client, DeltaRaumiDbContext deltaRaumiDbContext, LoggingService loggingService)
        {
            Client = client;
            DeltaRaumiDbContext = deltaRaumiDbContext;
            LoggingService = loggingService;

            client.SelectMenuExecuted += Client_SelectMenuExecuted;
            client.ButtonExecuted += Client_ButtonExecuted;
            //client.SlashCommandExecuted += Client_SlashCommandExecuted;
        }

        private async Task Client_ButtonExecuted(SocketMessageComponent component)
        {
            await component.DeferAsync();

            EmbedBuilder builder= new EmbedBuilder();
            switch (component.Data.CustomId)
            {
                case "DoPat":
                    builder.WithAuthor(component.User);
                    builder.WithDescription("あなたはこの狐を撫でてみることにした\nまんざらでもなさそうだ");
                    builder.WithImageUrl("");
                    builder.WithColor(RaumiMainColor);
                    await component.FollowupAsync(embed: builder.Build(),ephemeral: true) ;
                    break;
                case "DontPat":
                    builder.WithAuthor(component.User);
                    builder.WithDescription("あなたはこの狐に触ることを拒んだ\nご機嫌ななめだった");
                    builder.WithImageUrl("");
                    builder.WithColor(Color.DarkRed);
                    await component.FollowupAsync(embed: builder.Build(), ephemeral: true) ;
                    break;
                default:
                    await LoggingService.LogGeneral("通常では到達できないエラー(E-5900)",LoggingService.LogGeneralSeverity.Error);
                    break;
            }
        }

        private async Task Client_SelectMenuExecuted(SocketMessageComponent component)
        {
            //await component.DeferAsync();

            DiscordComponentModel model = DeltaRaumiDbContext.Components.Find(Guid.Parse(component.Data.CustomId));

            EmbedBuilder builder = new EmbedBuilder();

            configuration = new Configuration().GetConfig();

            switch (model.DeltaRaumiComponentType)
            {
                case "FAQ-Menu":
                    if (component.User.Id != model.OwnerId)
                    {
                        await component.RespondAsync("**許可されていない動作**：このメニューは他ユーザーによって制御されています。", ephemeral: true);
                        return;
                    }

                    switch (component.Data.Values.FirstOrDefault())
                    {
                        case "about":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("DeltaRaumiとは？");
                            builder.WithDescription("Raumiが制作した主に個人用途のBOTです。2024年11月に計画され、2025年1月に見切り発車されているアルファ段階のBOTのため常時稼働はしていません。");
                            builder.AddField("Github", "実行中のコードは[ここにあります](https://github.com/raumigit/RaumiDiscord.Core)");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("https://raumisrv.com/");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("ヘルプを参照中");
                            break;

                        case "nowsettings":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("現在の設定値");
                            builder.WithDescription(
                                $"タイムゾーン：{TimeZoneInfo.Local}\n" +
                                "使用言語：JA-JP\n"+
                                "VC状態：--\n"+
                                "使用DB：SQlite3\n" 
                                );
                            
                            builder.WithColor(RaumiMainColor);
                            //builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("ヘルプを参照中");
                            break;

                        case "serverstat":
                            DateTime localuptime = configuration.Setting.UpTime;
                            DateTime utcUptime = localuptime.ToUniversalTime();
                            long unixTime = new DateTimeOffset(utcUptime).ToUnixTimeSeconds(); 
                            //string unixTimestr = unixTime.ToString();
                            builder.WithAuthor(component.User);
                            builder.WithTitle("サーバーの状態");
                            builder.WithDescription(
                                "バージョン：0.1.1.6 (2025/02/21-16:42)\n " +
                                "外部連携：null\n" +
                                "読み上げエンジン：null\n" +
                                "WebGUI：null" +
                                "API：Online\n" +
                                "ロギング中：no\n" +
                                "Stat機能：null\n" +
                                "使用DB：SQlite3\n" +
                                $"致命的なエラー：{configuration.Setting.SystemFatal.ToString()}\n"
                                );
                            builder.AddField("UpTime",$"<t:{unixTime.ToString()}:R>");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("DeltaRaumi");
                            break;

                        case "website":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("DeltaRaumiのホームページはある?");
                            builder.WithDescription("現在はありませんが、いつかはどこかを使って乗せる予定です。");
                            builder.AddField("Github", "実行中のコードは[ここにあります](https://github.com/raumigit/RaumiDiscord.Core)");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("https://github.com/raumigit/RaumiDiscord.Core");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("DeltaRaumi");
                            break;

                        case "donate":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("Patreonとかしてるの？");
                            builder.WithDescription("やってもいいけど面倒くさいから今のところ無いと思っておいてください。\n  Discord内の決済機能が開放されたら実装の目処は付ける予定");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("DeltaRaumi");
                            break;

                        case "bookmark":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("ブックマークって何？");
                            builder.WithDescription("スラッシュコマンドに一つで特定のタイプのURLを出してくれます。\n ＊現在登録はできません。");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("ヘルプを参照中");
                            break;

                        case "wayoperate-24":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("なぜ24時間上がってないの？");
                            builder.WithDescription("Releaseバージョンではないため開発と同時に適当にセルフホスティングされているためです。v1.0までは頻繁に停止します。");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("ヘルプを参照中");
                            break;

                        case "enhancement":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("新しい機能を作る予定は？");
                            
                            builder.WithDescription("現在開発中に機能は以下のとおりです。");
                            builder.AddField("ウェブ側", "- WebGui");
                            builder.AddField("サーバー側", 
                                "- 名刺(カード)生成機能\n" +
                                "- stat機能\n" +
                                "- 誕生日機能\n" +
                                "- 読み上げ機能\n" +
                                "- ウェルカムメッセージ\n");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("DeltaRaumi");
                            break;

                        case "updatenow":
                            builder.WithAuthor(component.User);
                            builder.WithTitle("最新で行われた変更は？");
                            builder.WithDescription("スラッシュコマンドにリージョンの指定が追加されました。");
                            builder.WithColor(RaumiMainColor);
                            builder.WithUrl("");
                            builder.WithCurrentTimestamp();
                            builder.WithFooter("DeltaRaumi");
                            break;
                    }

                    await component.Message.ModifyAsync(x =>
                    {
                        x.Components = null;
                        x.Content = "";
                        x.Embed = builder.Build();
                    });
                    break;
            }
            DeltaRaumiDbContext.Components.Remove(model);
            await DeltaRaumiDbContext.SaveChangesAsync();
        }
    }
}