using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Common;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Configuration;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;
using RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using System.Diagnostics;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot
{
    internal class Deltaraumi_Discordbot
    {
        private DiscordSocketClient _client;
        private ImprovedLoggingService _logger;
        private IServiceProvider _services;
        private BotConfiguration _config;
        private CancellationTokenSource _cancellationTokenSource;
        private DiscordCoordinationService DiscordCoordinationService;

        private long _startTimestamp;
        private bool _serverShutdownRequested;

        private List<ulong>? GuildIDs { get; set; }


        // 後方互換性のための静的プロパティ（段階的に削除予定）


        private static Deltaraumi_Discordbot? _instance;
        private StartupOptions options;

        private Deltaraumi_Discordbot()
        {
            _instance = this;
            _logger = new ImprovedLoggingService();
            _cancellationTokenSource = new CancellationTokenSource();
            
        }

        private async Task OnReady()
        {
            await _logger.Log("Discord client ready", "Startup", ImprovedLoggingService.LogLevel.Info);
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
                Console.WriteLine($"Fatal error occurred: {e.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
                Environment.ExitCode = 1;
                throw;
            }
        }

        private async Task MainAsync(string[] args)
        {
            //static Task ErrorParsingArguments(IEnumerable<Microsoft.Extensions.Configuration.CommandLine.Error> errors)
            //{
            //    Console.WriteLine("Command line parsing failed:");
            //    foreach (var error in errors)
            //    {
            //        Console.WriteLine($"{error}");
            //    }
            //    Environment.ExitCode = 1;
            //    return Task.CompletedTask;
            //}

            // Parse the command line arguments and either start the app or exit indicating error
            StartApp();
        }

        public void StartApp()
        {
            _startTimestamp = Stopwatch.GetTimestamp();

            // 簡素化されたアプリケーション設定の作成
            var appPaths = CreateSimplifiedApplicationPaths(options);
            var startupConfig = CreateAppConfiguration(options, appPaths);

            do
            {
                try
                {
                    _serverShutdownRequested = false;
                    StartServer(appPaths, options, startupConfig);
                }
                catch (Exception ex)
                {
                    using var _ = _logger?.Log($"Server start failed: {ex.Message}", "Startup", ImprovedLoggingService.LogLevel.Error) ?? Task.CompletedTask;

                    // 重大なエラーの場合は再起動を試みない
                    if (ex is InvalidOperationException || ex is ArgumentException)
                    {
                        break;
                    }

                    // 再起動前に少し待機
                    Task.Delay(5000, _cancellationTokenSource?.Token ?? CancellationToken.None);
                }
            } while (_serverShutdownRequested && !(_cancellationTokenSource?.Token.IsCancellationRequested ?? true));
        }

        public async Task StartServer(ServerApplicationPaths appPaths, StartupOptions options, IConfiguration startupConfig)
        {
            try
            {
                await _logger.Log("DeltaRaumiを初期化中", "Startup");

                //Discord クライアントの設定
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    ConnectionTimeout = 8000,
                    HandlerTimeout = 3000,
                    MessageCacheSize = 1000,
                    LogLevel = LogSeverity.Verbose,
                    LogGatewayIntentWarnings = true,
                });
                //試験的変更：serilogに統一するため独自のログイベントを無効化
                _client.Log += LogAsync;
                _client.Ready += OnReady;

                await _logger.Log("サービスプロバイダを設定中...", "Startup");

                //_services = BuildServices();
                // 新しい設定システムを使用してサービスを構成
                ConfigureServices(appPaths, startupConfig);

                _config = _services.GetRequiredService<BotConfiguration>();

                // サービスの取得
                DiscordCoordinationService = _services.GetRequiredService<DiscordCoordinationService>();

                await _logger.Log("イベントハンドラーを初期化中...", "Startup");
                await _services.GetRequiredService<InteractionHandler>().InitializeAsync();
                await _services.GetRequiredService<DeltaRaumiHandler>().InitializeAsync();

                // DbContextの初期化とマイグレーション
                await InitializeDatabaseAsync();

                await _logger.Log("初期化が完了", "Startup");

                Console.ForegroundColor = ConsoleColor.Magenta;
                await _logger.Log("DeltaRaumi接続中", "Startup", ImprovedLoggingService.LogLevel.Notice);
                Console.ResetColor();
                
                // Discord への接続
                ConnectToDiscord();

                // 無限待機の代わりにキャンセレーショントークンを使用

                await Task.Delay(Timeout.Infinite, _cancellationTokenSource!.Token);
                // ギルドIDを取得
                //GuildIDs = _client.Guilds.Select(guild => guild.Id).ToList();//null
            }
            catch (OperationCanceledException)
            {
                await _logger!.Log("アプリケーションのシャットダウンが要求されました", "Startup", ImprovedLoggingService.LogLevel.Info);
            }
            catch (Exception e)
            {
                await _logger!.Log($"Server startup error: {e.Message}\n{e.StackTrace}", "Startup", ImprovedLoggingService.LogLevel.Fatal);
                throw; // 再スローして上位で適切に処理
            }
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices(ServerApplicationPaths appPaths, IConfiguration startupConfig)
        {
            var services = new ServiceCollection();

            

            // 既存のクライアントインスタンスがある場合は置き換え
            if (_client != null)
            {
                services.RemoveAll<DiscordSocketClient>();
                services.AddSingleton(_client);
            }

            // 既存のロガーインスタンスがある場合は置き換え
            if (_logger != null)
            {
                services.RemoveAll<ImprovedLoggingService>();
                services.AddSingleton(_logger);
            }

            // 新しいDIシステムを使用してすべてのサービスを登録
            services.AddDeltaRaumiServices(Directories.Config, "Data Source=deltaraumi.db",_client,_logger);

            _services = services.BuildServiceProvider();

            _logger.Log("サービスプロバイダーの設定が正常に完了しました", "Startup", ImprovedLoggingService.LogLevel.Info);

            return _services;
        }
        
        private async Task ConnectToDiscord()
        {
            try
            {
                await _logger.Log("Discord へ接続中", "Startup");

                if (string.IsNullOrWhiteSpace(_config.TokenData?.Token))
                {
                    throw new InvalidOperationException("Discordボットのトークンが設定されていません。設定ファイルでトークンを設定してください。");
                }

                await _client.LoginAsync(TokenType.Bot, _config.TokenData.Token);
                await _client.StartAsync();

                await _logger.Log("Discord への接続が完了", "Startup", ImprovedLoggingService.LogLevel.Info);
                
            }
            catch (HttpException e)
            {
                await _logger.Log("Discord への接続に失敗", "Startup", ImprovedLoggingService.LogLevel.Error);
                await _logger.Log($"HTTP Error: {e.HttpCode} - {e.Reason}", "Startup", ImprovedLoggingService.LogLevel.Error);
                throw new InvalidOperationException("Discordへの接続に失敗しました。ボットトークンとネットワーク接続を確認してください。", e);
            }
            catch (Exception e)
            {
                await _logger.Log($"Discord接続中に予期せぬエラーが発生しました: {e.Message}", "Startup", ImprovedLoggingService.LogLevel.Error);
                throw;
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                await _logger!.Log("データベースを初期化中...", "Database");

                using var scope = _services!.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DeltaRaumiDbContext>();

                // 起動時に新しいデータベース移行を適用する
                var migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Any())
                {
                    await _logger.Log($"Applying {migrations.Count()} database migrations...", "Database");
                    await dbContext.Database.MigrateAsync();
                    await _logger.Log("Database migrations completed successfully", "Database", ImprovedLoggingService.LogLevel.Info);
                }
                else
                {
                    await _logger.Log("データベースは最新の状態です", "Database");
                }
            }
            catch (Exception ex)
            {
                await _logger!.Log($"データベース初期化エラー: {ex.Message}", "Database", ImprovedLoggingService.LogLevel.Error);
                throw new InvalidOperationException("Failed to initialize database", ex);
            }
        }

        private static Task LogAsync(LogMessage log)
        {
            if (_instance?._logger != null)
            {
                ImprovedLoggingService.LogLevel level = DiscordLoggingAdapter.ConvertDiscordLogLevel(log.Severity);
                _instance._logger.Log($"{log.Message}", $"{log.Source}", level);
            }
            return Task.CompletedTask;
        }

        private ServerApplicationPaths CreateSimplifiedApplicationPaths(StartupOptions options)
        {
            // StartupOptionsが空のクラスの場合の暫定実装
            return new ServerApplicationPaths
            {
                // 必要に応じてオプションから値を設定
                // 現在は空のクラスなので、デフォルト値を使用
            };
        }

        private IConfiguration CreateAppConfiguration(StartupOptions options, ServerApplicationPaths appPaths)
        {
            try
            {
                // 設定管理クラスを使用してIConfigurationを作成
                var configManager = new Common.Configuration.ConfigurationManager(Directories.Config);
                var botConfig = configManager.GetConfiguration();

                // 設定の検証
                if (!configManager.ValidateConfiguration())
                {
                    throw new InvalidOperationException("Configuration validation failed");
                }

                // BotConfigurationをIConfigurationにアダプト
                return new BotConfigurationAdapter(botConfig);
            }
            catch (Exception ex)
            {
                _logger?.Log($"Configuration creation failed: {ex.Message}", "Startup", ImprovedLoggingService.LogLevel.Error);
                throw new InvalidOperationException("Failed to create application configuration", ex);
            }
        }

        /// <summary>
        /// アプリケーションの正常なシャットダウンを開始
        /// </summary>
        public async Task RequestShutdownAsync()
        {
            await (_logger?.Log("Shutdown requested", "Shutdown") ?? Task.CompletedTask);

            _serverShutdownRequested = true;
            _cancellationTokenSource?.Cancel();

            if (_client?.ConnectionState == ConnectionState.Connected)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }

            //_services?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// リソースの適切な解放
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
            //_services?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        private IServiceProvider BuildServices() => new ServiceCollection()
            // データベースコンテキスト - Scopedが最適（リクエストごとに新しいインスタンス）
            .AddDbContext<DeltaRaumiDbContext>()

            // 設定やクライアントなどの基本的なシングルトン
            .AddSingleton(_client)
            .AddSingleton(_config)
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
            .AddScoped<GameMetaService>()
            // Guild IDsのコレクション
            //.AddSingleton(provider => _client.Guilds.Select(guild => guild.Id).ToList())

            .BuildServiceProvider();
    }
}
