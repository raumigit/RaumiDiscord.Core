using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
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

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot
{
    /// <summary>
    /// DI コンテナ設定を簡素化するための拡張クラスです。<br/>
    /// サービス登録や検証、共通ユーティリティへのアクセスを提供します。
    /// </summary>
    /// <remarks>
    /// このクラスは、データベースコンテキスト、コアアプリケーションサービス、Discord関連サービスを含む、
    /// 必要なすべてのサービスを依存性注入コンテナに登録するための中央集約的な方法を提供します。
    /// これにより、各サービスの使用要件に基づいて適切なライフタイムでサービスが登録されることが保証されます。
    /// </remarks>
    public static class DependencyInjection
    {
        

        /// <summary>
        /// DIコンテナにすべてのサービスを登録します。
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        /// <param name="configFilePath">設定ファイルのパス</param>
        /// <param name="connectionString">データベース接続文字列（省略可能）</param>
        public static IServiceCollection AddAllServices(
            this IServiceCollection services,
            string configFilePath,
            string? connectionString,
            DiscordSocketClient? client,
            ImprovedLoggingService? logger)
        {
            // ========================================================================
            // Core Configuration Services - 設定関連サービス
            // ========================================================================

            // 新しいConfiguration システムを登録
            services.AddDeltaRaumiConfiguration(configFilePath);

            // ========================================================================
            // Discord Client Configuration - Discordクライアント設定
            // ========================================================================

            // DiscordSocketClient を設定と一緒に登録
            services.AddSingleton(provider =>
            {
                return new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    ConnectionTimeout = 8000,
                    HandlerTimeout = 3000,
                    MessageCacheSize = 1000,
                    LogLevel = LogSeverity.Verbose,
                    LogGatewayIntentWarnings = true,
                });
            });
            services.AddSingleton(client ?? new DiscordSocketClient());

            // ========================================================================
            // Database Services - データベース関連サービス
            // ========================================================================

            // データベースコンテキスト
            services.AddDbContext<DeltaRaumiDbContext>(options =>
            {
                var connStr = connectionString ?? "Data Source=deltaraumi.db";
                options.UseSqlite(connStr);
            }, ServiceLifetime.Scoped);

            // ========================================================================
            // Logging Services - ログ関連サービス
            // ========================================================================

            services.AddSingleton<ImprovedLoggingService>();
            services.AddSingleton<DiscordLoggingAdapter>();

            // Microsoft.Extensions.Logging integration　計画：このコードをコンフィグから切り替えれるように再実装
            //services.AddLogging(builder =>
            //{
            //    builder.AddConsole();
            //    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            //});
            services.AddSingleton(logger ?? new ImprovedLoggingService());
            // ========================================================================
            // Discord Core Services - Discord コアサービス
            // ========================================================================

            services.AddSingleton<CommandService>();

            // InteractionService を DiscordSocketClient と一緒に登録
            services.AddSingleton<InteractionService>(provider => new InteractionService(provider.GetRequiredService<DiscordSocketClient>()));

            //services.AddSingleton(_client);

            // ========================================================================
            // Discord Event Handlers - Discordイベントハンドラー
            // ========================================================================

            services.AddSingleton<InteractionHandler>();
            services.AddSingleton<DeltaRaumiHandler>();
            services.AddSingleton<DeltaRaumiEventHandler>();

            // ========================================================================
            // Discord Interaction Services - Discord相互作用サービス
            // ========================================================================

            services.AddSingleton<ComponentInteractionService>();
            services.AddSingleton<SlashCommandInterationService>();

            // ========================================================================
            // Core Application Services - コアアプリケーションサービス
            // ========================================================================

            services.AddSingleton<DiscordCoordinationService>();
            services.AddSingleton<MessageService>();
            services.AddSingleton<WelcomeMessageService>();
            services.AddSingleton<StatService>();
            services.AddSingleton<VoicertcregionService>();
            services.AddSingleton<ScheduledTask>();

            // ========================================================================
            // Scoped Services - スコープ付きサービス（リクエストごと）
            // ========================================================================

            services.AddScoped<DataEnsure>();
            services.AddScoped<LevelService>();
            services.AddScoped<GameMetaService>();

            // ========================================================================
            // Utility Services - ユーティリティサービス  
            // ========================================================================

            // Guild IDsのコレクション（遅延初期化）
            services.AddSingleton<Func<List<ulong>>>(provider =>
            {
                return () =>
                {
                    var client = provider.GetRequiredService<DiscordSocketClient>();
                    return client.Guilds?.Select(guild => guild.Id).ToList() ?? new List<ulong>();
                };
            });

            return services;
        }

        /// <summary>
        /// 基本的なサービスのみを登録する軽量版
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        /// <param name="configFilePath">設定ファイルのパス</param>
        public static IServiceCollection AddMinimalServices(
            this IServiceCollection services,
            string configFilePath)
        {
            // 基本設定
            services.AddDeltaRaumiConfiguration(configFilePath);

            // 基本ログ
            services.AddSingleton<ImprovedLoggingService>();

            // Discord クライアント
            services.AddSingleton<DiscordSocketClient>();

            return services;
        }

        /// <summary>
        /// DI コンテナに必要な主要サービスが正しく登録されているかを確認します。<br/>
        /// ビルド時にバリデーションを有効化し、例外が発生した場合は false を返却します。
        /// </summary>
        /// <param name="services">検証対象の IServiceCollection。</param>
        /// <returns>すべての必須サービスが登録済みなら true、そうでなければ false。</returns>
        public static bool ValidateServiceRegistration(IServiceCollection services)
        {
            try
            {
                var provider = services.BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

                // 重要なサービスが登録されているかチェック
                var config = provider.GetService<BotConfiguration>();
                var client = provider.GetService<DiscordSocketClient>();
                var logger = provider.GetService<ImprovedLoggingService>();
                var coordinator = provider.GetService<DiscordCoordinationService>();

                return config != null && client != null && logger != null && coordinator != null;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// DIコンテナ設定のヘルパークラス
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// DeltaRaumi アプリケーションで使用するすべてのサービスを DI コンテナに登録します。<br/>
        /// これには、Discord クライアント、ロギング、データベース接続、ビジネスロジック層などが含まれます。
        /// </summary>
        /// <param name="services">DI コンテナのサービスコレクション。</param>
        /// <param name="configFilePath">BotConfiguration を読み込む設定ファイルへのパス。</param>
        /// <param name="connectionString">データベース接続文字列。</param>
        /// <returns>登録済みの IServiceCollection を返します。</returns>
        public static IServiceCollection AddDeltaRaumiServices(
            this IServiceCollection services,
            string configFilePath,
            string? connectionString,
            DiscordSocketClient? client,
            ImprovedLoggingService? logger)
        {
            return services.AddAllServices(configFilePath, connectionString,client,logger);
        }

        /// <summary>
        /// 開発用の軽量サービス設定
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        /// <param name="configFilePath">設定ファイルのパス</param>
        /// <returns>設定されたサービスコレクション</returns>
        public static IServiceCollection AddDeltaRaumiDevelopmentServices(
            this IServiceCollection services,
            string configFilePath)
        {
            return services.AddMinimalServices(configFilePath);
        }
    }
}
