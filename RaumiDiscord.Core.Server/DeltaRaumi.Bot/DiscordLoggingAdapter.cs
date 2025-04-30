using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
//using RaumiDiscord.Core.Server.Logging;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    /// <summary>
    /// ImprovedLoggingServiceとDiscordクライアントを接続するアダプター
    /// </summary>
    public class DiscordLoggingAdapter
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        /// <summary>
        /// DiscordLoggingAdapterのコンストラクター
        /// </summary>
        /// <param name="logger">使用するロギングサービス</param>
        /// <param name="client">Discordクライアント</param>
        /// <param name="commands">コマンドサービス</param>
        public DiscordLoggingAdapter(
            ImprovedLoggingService logger,
            DiscordSocketClient client,
            CommandService commands)
        {
            _logger = logger;
            _client = client;
            _commands = commands;

            // イベントハンドラの登録
            _client.Log += LogDiscord;
            _commands.Log += LogCommand;
        }

        /// <summary>
        /// Discord SDKからのログメッセージを処理します
        /// </summary>
        public Task LogDiscord(LogMessage message)
        {
            ImprovedLoggingService.LogLevel level = ConvertDiscordLogLevel(message.Severity);
            string source = message.Source ?? "Discord";

            string logMessage = message.Exception != null
                ? $"{message.Message}: {message.Exception}"
                : message.Message;

            return _logger.Log(logMessage, source, level);
        }

        /// <summary>
        /// Discord SDKのログレベルを変換します
        /// </summary>
        public static ImprovedLoggingService.LogLevel ConvertDiscordLogLevel(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => ImprovedLoggingService.LogLevel.Fatal,
                LogSeverity.Error => ImprovedLoggingService.LogLevel.Error,
                LogSeverity.Warning => ImprovedLoggingService.LogLevel.Warning,
                LogSeverity.Info => ImprovedLoggingService.LogLevel.Info,
                LogSeverity.Debug => ImprovedLoggingService.LogLevel.Debug,
                LogSeverity.Verbose => ImprovedLoggingService.LogLevel.Verbose,
                _ => ImprovedLoggingService.LogLevel.Info
            };
        }

        /// <summary>
        /// コマンドからのログメッセージを処理します
        /// </summary>
        public Task LogCommand(LogMessage message)
        {
            // コマンドエラーの特別な処理
            if (message.Exception is CommandException commandException)
            {
                // チャンネルにエラーメッセージを送信
                _ = commandException.Context.Channel.SendMessageAsync($"コマンドエラー: {commandException.Message}");

                // 詳細なエラー情報をログに記録
                return _logger.Log($"コマンド '{commandException.Command.Name}' でエラー発生: {commandException.Message}",
                    "Commands", ImprovedLoggingService.LogLevel.Error);
            }

            return LogDiscord(message);
        }
    }
}
//memo: DiscordLogを用いるサービスは以下のコードでの記述が必要 _discordLogger = new DiscordLoggingAdapter(_logger, _client, _commands);
