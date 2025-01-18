using Discord.WebSocket;
using Discord;
using Nett;
using Newtonsoft.Json.Linq;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DiscordBot.Data;
using Discord.Net;
using RaumiDiscord.Core.Server.DataContext;
using Discord.Rest;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    internal class Deltaraumi_Discordbot
    {
        //public IReadOnlyCollection<SocketGuildUser> ConnectedUsers { get; private set; }
        //VCユーザーを取るためのコマンド　検証と実装はこれから
        public List<ulong> guildIDs { get; private set; }

        public static ulong vc_chid { get; set; }
        public static string vc_region { get; set; }

        //public static ulong GuildId { get; private set; }
        //おそらく出番なし
        
        public static DiscordSocketClient _client;
        private IServiceProvider _services;
        public static Config _Config;
        public static SqlMode AppSqlMode { get; set; }
        public enum SqlMode { Sqlite, MariaDb }
        private DiscordCoordinationService DiscordCoordinationService;

        public static void Deltaraumi_load(string[] args)
        {
            Console.WriteLine($"Directories.Config = {Directories.Config}");
            Console.WriteLine($"Directories.Appdata = {Directories.AppData}");
            Console.WriteLine($"Directories.ProgramData = {Directories.ProgramData}");
            Console.WriteLine();
            try
            {
                new Deltaraumi_Discordbot().MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }

        }

        private async Task MainAsync(string[] args)
        {
            try
            {
                await Log(new LogMessage(LogSeverity.Info, "Startup", "DeltaRaumiを初期化中"));

                

                await Log(new LogMessage(LogSeverity.Info, "Startup", "サービスプロバイダを設定中..."));

                _Config = new Config().GetConfigFromFile();

                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    ConnectionTimeout = 8000,
                    HandlerTimeout = 3000,
                    MessageCacheSize = 64,
                    LogLevel = LogSeverity.Verbose,
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                });
                _services = BuildServices();

                var dbContext = _services.GetRequiredService<DeltaRaumiDbContext>();
                this.DiscordCoordinationService = _services.GetRequiredService<DiscordCoordinationService>();

                //apply new database migrations on startup
                var migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Count() > 0)
                {
                    Console.WriteLine("Applying database migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("Done.");
                }

                await Log(new LogMessage(LogSeverity.Info, "Startup", "全て初期化が完了"));

                await Log(new LogMessage(LogSeverity.Info, "Startup", "ログイン→"));
                try
                {
                    string _token = _Config.TokenData.Token;

                    await _client.LoginAsync(TokenType.Bot, _token);
                    //await Log(new LogMessage(LogSeverity.Debug, "Startup", $"Tokun:{_token}"));

                    await _client.StartAsync();
                }
                catch (HttpException e)
                {
                    await Log(new LogMessage(LogSeverity.Critical, "Startup", "ログイン失敗"));
                    await Log(new LogMessage(LogSeverity.Critical, "Startup", $"{e}"));
                    Environment.Exit(1);
                }

                _client.MessageReceived += MessageReceivedAsync;
                _client.MessageUpdated += MessageUpdated;
                //_client.MessageDeleted += MessegeDeleted;
                //まもなく分離　分離後はサービスプロバイダに登録予定

                guildIDs = _client.Guilds.Select(guild => guild.Id).ToList();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(new LogMessage(LogSeverity.Info, "Startup", "DeltaRaumi接続中").ToString());
                Console.ResetColor();

                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("===============================");
                Console.WriteLine(e.StackTrace);

                Environment.Exit(1);
            }
        }
        //今こそ動作はしていないが戻す予定/コードだけ残す
        private static async Task InteractionCreatedAsync(SocketInteraction interaction)
        {
            // 安全キャストは、キャストされるものが null になるのを防ぐ最善の方法です。
            // このチェックに合格しない場合は、その型にキャストできません。
            if (interaction is SocketMessageComponent component)
            {
                // 上記のボタン(MessageReceivedAsync)で作成された ID を確認します。
                if (component.Data.CustomId == "pingbtn")
                    await interaction.RespondAsync("やぁ、ラウミことデルタラウミだよ。");

                else
                    Console.WriteLine("ハンドラーのない ID が飛んできたぞ…？");
            }
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Console.WriteLine($"*ReceivedServer:");
            Console.WriteLine($"|ReceivedChannel:{message.Channel}");
            Console.WriteLine($"|ReceivedUser:{message.Author}");
            Console.WriteLine($"|MessageReceived:{message.Content}");
            Console.WriteLine($"|CleanContent:{message.CleanContent}");
            Console.WriteLine($"|EmbedelMessage:{message.Embeds}");
            //ボットは自分自身に応答してはなりません。
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
            {
                Console.WriteLine(">PING");
                // ドロップダウンとボタンを作成できる新しいコンポーネント ビルダーを作成します。
                var cb = new ComponentBuilder().WithButton("クリック", "pingbtn", ButtonStyle.Primary);

                // ボタンを含む、コンテンツ 'pong' を含むメッセージを送信します。
                // このボタンは、呼び出しに渡される前に .Build() を呼び出してビルドする必要があります。
                await message.Channel.SendMessageAsync("pong!", components: cb.Build());
            }

            try
            {
                string contentbase = "@Raumi#1195 *";
                switch (message.CleanContent)
                {
                    case "@Raumi#1195":
                        await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/20241004114923.png", "ん？");
                        break;

                    case "@Raumi#1195 VCADD":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されました。");
                        break;

                    case "@Raumi#1195 Discordリージョン：適当" or "@Raumi#1195 Discordリージョン：" or "@Raumi#1195 reset":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されています。代わりに/vc-regionを利用してください。");
                        break;

                    case "@Raumi#1195 Discordリージョン：HK" or "@Raumi#1195 Discordリージョン：香港" or "@Raumi#1195 VCHK":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されています。代わりに/vc-regionを利用してください。");
                        break;

                    case "@Raumi#1195 Discordリージョン：JP" or "@Raumi#1195 Discordリージョン：日本" or "@Raumi#1195 VCJP":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されています。代わりに/vc-regionを利用してください。");
                        break;

                    case "@Raumi#1195 Discordリージョン：BR" or "@Raumi#1195 Discordリージョン：ブラジル" or "@Raumi#1195 VCBR":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されています。代わりに/vc-regionを利用してください。)");
                        break;

                    case "@Raumi#1195 Discordリージョン：SG" or "@Raumi#1195 Discordリージョン：シンガポール" or "@Raumi#1195 VCSG":
                        await message.Channel.SendMessageAsync("＊このコマンドは廃止されています。代わりに/vc-regionを利用してください。");
                        break;

                    case "@Raumi#1195 画像１":
                        //await message.Channel.SendMessageAsync("*TBA");
                        await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/IMG_2265.JPG", "ん？");
                        break;

                    case "@Raumi#1195 画像２":
                        //await message.Channel.SendMessageAsync("*TBA");
                        await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/20241004114915.png", "ん？");
                        break;

                    case "@Raumi#1195 誰がいる？" or "@Raumi#1195 !vcls":
                        await message.Channel.SendMessageAsync("そのうち実装されます(スラッシュコマンドと共に)");
                        Console.WriteLine($"未実装");
                        break;

                    case "@Raumi#1195 GlobalCommandUpdate" :
                        if (message.Author.Id == 558636367106539521)
                        {
                            await SlashCommandInterationService.GlobalCommandUpdate();
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("うん？\nこの指示を受け付けてるのはあなたではないはず…");
                        }
                        break;

                    case string match when System.Text.RegularExpressions.Regex.IsMatch(message.CleanContent, contentbase):

                        await message.Channel.SendMessageAsync("該当するメッセージコマンドはないっぽい…");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("メッセージ送信エラー　(E-M4001)");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        }

        private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // メッセージがキャッシュになかった場合、ダウンロードすると `after` のコピーが取得されます。
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message.Channel}\n{message.Author}:```diff\n- {message}\n! {after}\n```");
        }
        public static Task Log(LogMessage msg) => Task.Run(() => Console.WriteLine(msg.ToString()));
        private IServiceProvider BuildServices()
        => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton<CommandService>()
            .AddSingleton<LoggingService>()
            .AddDbContext<DeltaRaumiDbContext>()
            .AddSingleton<SlashCommandInterationService>()
            .AddSingleton<WelcomeMessageService>()
            .AddSingleton<ComponentInteractionService>()
            .AddSingleton<DiscordCoordinationService>()
            .AddSingleton<VoicertcregionService>()
            .BuildServiceProvider();
    }
}
