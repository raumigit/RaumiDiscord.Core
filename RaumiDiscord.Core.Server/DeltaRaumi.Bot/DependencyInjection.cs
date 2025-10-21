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
    /// Provides methods for configuring and managing dependency injection in the application.
    /// </summary>
    /// <remarks>
    /// This class provides a centralized method to register all required services into the dependency 
    /// injection container, including database contexts, core application services, and Discord-related services.
    /// It ensures that services are registered with appropriate lifetimes based on their usage requirements.
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

            // Microsoft.Extensions.Logging integration
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            });
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
        /// サービス登録の検証
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        /// <returns>検証結果</returns>
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
        /// DeltaRaumiアプリケーション用の完全なサービス設定
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        /// <param name="configFilePath">設定ファイルのパス</param>
        /// <param name="connectionString">データベース接続文字列</param>
        /// <returns>設定されたサービスコレクション</returns>
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
