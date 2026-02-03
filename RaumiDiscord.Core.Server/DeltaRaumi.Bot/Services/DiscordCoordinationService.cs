using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;
using RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models;
using System;
using System.Threading.Tasks;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// Discord との連携を調整するサービス
    /// </summary>
    public class DiscordCoordinationService
    {
        private readonly DiscordSocketClient _client;
        private readonly SlashCommandInterationService? _slashCommandService;
        private readonly ComponentInteractionService? _componentInteractionService;
        private readonly ImprovedLoggingService _loggingService;
        private readonly WelcomeMessageService? _welcomeMessageService;
        private readonly ILogger<DiscordCoordinationService>? _logger;

        public BotConfiguration Config { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="loggingService"></param>
        /// <param name="config"></param>
        /// <param name="slashCommandService"></param>
        /// <param name="componentInteractionService"></param>
        /// <param name="welcomeMessageService"></param>
        /// <param name="logger"></param>
        public DiscordCoordinationService(
            DiscordSocketClient client,
            ImprovedLoggingService loggingService,
            BotConfiguration config,
            SlashCommandInterationService? slashCommandService = null,
            ComponentInteractionService? componentInteractionService = null,
            WelcomeMessageService? welcomeMessageService = null,
            ILogger<DiscordCoordinationService>? logger = null)
        {
            _client = client;
            Config = config;

            _slashCommandService = slashCommandService;
            _componentInteractionService = componentInteractionService;
            _welcomeMessageService = welcomeMessageService;
            _logger = logger;
            _loggingService = loggingService;

            // イベントハンドラーを登録
            _client.Ready += OnReady;
            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnected;
        }

        /// <summary>
        /// Discord クライアントが準備完了した時のイベントハンドラー
        /// </summary>
        private async Task OnReady()
        {
            try
            {
                // カスタムゲームステータスを設定
                string? game = Config.Setting?.CustomStatusGame;
                if (!string.IsNullOrWhiteSpace(game))
                {
                    await _client.SetGameAsync(game);
                    await _loggingService.Log($"Custom game status set: {game}", "DiscordCoordination");
                }
                else
                {
                    await _loggingService.Log("No custom game status configured", "DiscordCoordination");
                }

                // 起動完了ログ
                await _loggingService.Log("Startup Complete", "Startup");
                await _loggingService.Log($"Login as {_client.CurrentUser?.Username ?? "Unknown User"}", "Startup");

                //_logger?.LogInformation("Bot startup complete. Logged in as: {Username}", _client.CurrentUser?.Username ?? "Unknown User");

                // 追加の初期化処理
                await InitializeServices();
            }
            catch (Exception ex)
            {
                await _loggingService.Log($"Error in OnReady: {ex.Message}", "DiscordCoordination", ImprovedLoggingService.LogLevel.Error);
                //_logger?.LogError(ex, "Error occurred in OnReady event handler");
            }
        }

        /// <summary>
        /// Discord に接続した時のイベントハンドラー
        /// </summary>
        private async Task OnConnected()
        {
            try
            {
                await _loggingService.Log("Connected to Discord", "DiscordCoordination", ImprovedLoggingService.LogLevel.Info);
                //_logger?.LogInformation("Connected to Discord");
            }
            catch (Exception ex)
            {
                await _loggingService.Log($"Error in OnConnected: {ex.Message}", "DiscordCoordination", ImprovedLoggingService.LogLevel.Error);
                //_logger?.LogError(ex, "Error occurred in OnConnected event handler");
            }
        }

        /// <summary>
        /// Discord から切断した時のイベントハンドラー
        /// </summary>
        private async Task OnDisconnected(Exception exception)
        {
            try
            {
                string message = exception != null ?
                    $"Disconnected from Discord: {exception.Message}" :
                    "Disconnected from Discord";

                await _loggingService.Log(message, "DiscordCoordination", ImprovedLoggingService.LogLevel.Warning);
                //_logger?.LogWarning(exception, "Disconnected from Discord");
            }
            catch (Exception ex)
            {
                await _loggingService.Log($"Error in OnDisconnected: {ex.Message}", "DiscordCoordination", ImprovedLoggingService.LogLevel.Error);
                //_logger?.LogError(ex, "Error occurred in OnDisconnected event handler");
            }
        }

        /// <summary>
        /// サービスの初期化
        /// </summary>
        private async Task InitializeServices()
        {
            try
            {
                // SlashCommand サービスの初期化
                if (_slashCommandService != null)
                {
                    await _loggingService.Log("Initializing SlashCommand service", "DiscordCoordination");
                    // 必要に応じて初期化処理を追加
                }

                // ComponentInteraction サービスの初期化
                if (_componentInteractionService != null)
                {
                    await _loggingService.Log("Initializing ComponentInteraction service", "DiscordCoordination");
                    // 必要に応じて初期化処理を追加
                }

                // WelcomeMessage サービスの初期化
                if (_welcomeMessageService != null)
                {
                    await _loggingService.Log("Initializing WelcomeMessage service", "DiscordCoordination");
                    // 必要に応じて初期化処理を追加
                }

                await _loggingService.Log("Service initialization completed", "DiscordCoordination", ImprovedLoggingService.LogLevel.Info);
            }
            catch (Exception ex)
            {
                await _loggingService.Log($"Error during service initialization: {ex.Message}", "DiscordCoordination", ImprovedLoggingService.LogLevel.Error);
                //_logger?.LogError(ex, "Error occurred during service initialization");
            }
        }

        /// <summary>
        /// サービスの正常なシャットダウン
        /// </summary>
        public async Task ShutdownAsync()
        {
            try
            {
                await _loggingService.Log("Starting DiscordCoordinationService shutdown", "DiscordCoordination", ImprovedLoggingService.LogLevel.Info);

                // イベントハンドラーを解除
                _client.Ready -= OnReady;
                _client.Connected -= OnConnected;
                _client.Disconnected -= OnDisconnected;

                await _loggingService.Log("DiscordCoordinationService shutdown completed", "DiscordCoordination", ImprovedLoggingService.LogLevel.Info);
            }
            catch (Exception ex)
            {
                await _loggingService.Log($"Error during shutdown: {ex.Message}", "DiscordCoordination", ImprovedLoggingService.LogLevel.Error);
                //_logger?.LogError(ex, "Error occurred during DiscordCoordinationService shutdown");
            }
        }

        /// <summary>
        /// 現在の接続状態を取得
        /// </summary>
        public bool IsConnected => _client.ConnectionState == Discord.ConnectionState.Connected;

        /// <summary>
        /// 現在のユーザー情報を取得
        /// </summary>
        public string CurrentUsername => _client.CurrentUser?.Username ?? "Not logged in";

        /// <summary>
        /// ギルド数を取得
        /// </summary>
        public int GuildCount => _client.Guilds?.Count ?? 0;
    }
}