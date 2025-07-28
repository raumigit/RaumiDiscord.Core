using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;
using RaumiDiscord.Core.Server.DeltaRaumi.Common;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DiscordBot.Services;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    internal class Deltaraumi_Discordbot
    {
        //public IReadOnlyCollection<SocketGuildUser> ConnectedUsers { get; private set; }
        //VCユーザーを取るためのコマンド　検証と実装はこれから
        public List<ulong>? guildIDs { get; private set; }

        public static ulong vc_chid { get; set; }
        public static string? vc_region { get; set; }

        //public static ulong GuildId { get; private set; }
        //おそらく出番なし

        public static DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private IServiceProvider? _services;
        private static IConfiguration _configuration;
        public static Configuration? _Config { get; set; }

        public static ImprovedLoggingService _logger;
        public static SqlMode AppSqlMode { get; set; }
        public enum SqlMode { Sqlite, MariaDb }
        private DiscordCoordinationService? DiscordCoordinationService;

        public Deltaraumi_Discordbot()
        {
            _logger = new ImprovedLoggingService();

        }

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
                await _logger.Log("DeltaRaumiを初期化中", "Startup");

                _Config = new Configuration().GetConfigFromFile();

                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    ConnectionTimeout = 8000,
                    HandlerTimeout = 3000,
                    MessageCacheSize = 1000,
                    LogLevel = LogSeverity.Verbose,
                    LogGatewayIntentWarnings = true,
                    //AlwaysDownloadUsers = true

                });
                await InitializeAsync();

                _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
                _services = BuildServices();

                await _logger.Log("サービスプロバイダを設定中...", "Startup");

                var dbContext = _services.GetRequiredService<DeltaRaumiDbContext>();
                this.DiscordCoordinationService = _services.GetRequiredService<DiscordCoordinationService>();
                _client.Log += LogAsync;

                guildIDs = _client.Guilds.Select(guild => guild.Id).ToList();
                await _services.GetRequiredService<InteractionHandler>().InitializeAsync();


                await _services.GetRequiredService<DeltaRaumiHandler>().InitializeAsync();

                //起動時に新しいデータベース移行を適用する
                var migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Count() > 0)
                {
                    Console.WriteLine("Applying database migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("Done.");
                }


                await _logger.Log("初期化が完了", "Startup");

                await _logger.Log("ログイン→", "Startup");
                try
                {
                    string _token = _Config.TokenData.Token;

                    await _client.LoginAsync(TokenType.Bot, _token);
                    //await Log(new LogMessage(LogSeverity.Debug, "Startup", $"Tokun:{_token}"));

                    await _client.StartAsync();
                }
                catch (HttpException e)
                {
                    await _logger.Log("ログイン失敗", "Startup", ImprovedLoggingService.LogLevel.Error);
                    await _logger.Log($"{e}", "Startup", ImprovedLoggingService.LogLevel.Error);
                    Environment.Exit(1);
                }


                //_client.MessageReceived += MessageReceivedAsync;
                //_client.MessageUpdated += MessageUpdated;
                //_client.MessageDeleted += MessegeDeleted;
                //まもなく分離　分離後はサービスプロバイダに登録予定


                Console.ForegroundColor = ConsoleColor.Magenta;
                await _logger.Log("DeltaRaumi接続中", "Startup", ImprovedLoggingService.LogLevel.Notice);
                Console.ResetColor();

                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                await _logger.Log($"{e.Message}\n{e.StackTrace}", "Startup", ImprovedLoggingService.LogLevel.Fatal);

                Environment.Exit(1);
            }
        }

        private Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // メッセージがキャッシュになかった場合、ダウンロードすると `after` のコピーが取得されます。
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message.Channel}|{message.Author}\n{message.Author}:```diff\n- {message}\n! {after}\n```");
        }

        //private readonly InteractionHandler;
        public static Task LogAsync(LogMessage log)
        {
            // LogMessageをImprovedLoggingServiceに変換して使用
            ImprovedLoggingService.LogLevel level = DiscordLoggingAdapter.ConvertDiscordLogLevel(log.Severity);
            _logger.Log($"{log.Message}", $"{log.Source}", level);

            return Task.CompletedTask;
        }

        private IServiceProvider BuildServices() => new ServiceCollection()
            // データベースコンテキスト - Scopedが最適（リクエストごとに新しいインスタンス）
            .AddDbContext<DeltaRaumiDbContext>()

            // 設定やクライアントなどの基本的なシングルトン
            .AddSingleton(_client)
            .AddSingleton(_configuration)
            .AddSingleton<ImprovedLoggingService>()
            .AddSingleton<DiscordLoggingAdapter>()

            // Discord関連のコアサービス
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<ComponentInteractionService>()
            .AddSingleton<InteractionHandler>()
            .AddSingleton<SlashCommandInterationService>()

            // イベントハンドラー
            .AddSingleton<DeltaRaumiHandler>()
            .AddSingleton<DeltaRaumiEventHandler>()

            // 状態を維持する必要があるコア機能サービス
            .AddSingleton<DiscordCoordinationService>()
            .AddSingleton<MessageService>()
            .AddSingleton<ScheduledTask>()
            .AddSingleton<StatService>()
            .AddSingleton<VoicertcregionService>()
            .AddSingleton<WelcomeMessageService>()

            // リクエストごとに新しいインスタンスが必要なサービス
            .AddScoped<DataEnsure>()
            .AddScoped<LevelService>()

            // Guild IDsのコレクション
            .AddSingleton(provider => _client.Guilds.Select(guild => guild.Id).ToList())

            .BuildServiceProvider();
    }
}