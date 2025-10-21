using Discord;
using Discord.Commands;
using System.Text;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers
{
    /// <summary>
    /// 汎用的なロギングサービス：コンソールとファイルの両方にログを出力します
    /// </summary>
    public class ImprovedLoggingService
    {
        private readonly string _logDirectory;
        private readonly object _fileLock = new object();

        /// <summary>
        /// ロギングのレベル。重要度の高い順に定義されています。
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// 致命的なエラー。システムが停止するようなエラー
            /// </summary>
            Fatal,
            /// <summary>
            /// エラー。機能が正常に動作しないエラー
            /// </summary>
            Error,
            /// <summary>
            /// 警告。注意が必要な状況
            /// </summary>
            Alert,
            /// <summary>
            /// 警告。注意が必要な状況
            /// </summary>
            Warning,
            /// <summary>
            /// 通知。重要だが緊急ではない通知
            /// </summary>
            Notice,
            /// <summary>
            /// 情報。通常の情報メッセージ
            /// </summary>
            Info,
            /// <summary>
            /// デバッグ情報。開発者向けの詳細な情報
            /// </summary>
            Debug,
            /// <summary>
            /// 詳細なデバッグ情報。開発者向けの詳細な情報
            /// </summary>
            Verbose
        }

        /// <summary>
        /// ロギングサービスのコンストラクター
        /// </summary>
        public ImprovedLoggingService(string? logDirectory = null)
        {
            // ログディレクトリの設定（指定がなければデフォルトディレクトリを使用）
            _logDirectory = logDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            EnsureLogDirectoryExists();
        }

        /// <summary>
        /// ログディレクトリが存在することを確認し、なければ作成します
        /// </summary>
        private void EnsureLogDirectoryExists()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// 現在のログファイル名を取得します（日付ベース）
        /// </summary>
        private string GetCurrentLogFileName()
        {
            return Path.Combine(_logDirectory, $"log-{DateTime.Now:yyyy-MM-dd}.log");
        }

        /// <summary>
        /// 一般ログを記録します
        /// </summary>
        public Task Log(string message, string source, LogLevel level = LogLevel.Info)
        {
            string logLevelShort = GetLogLevelShortName(level);
            string formattedMessage = FormatLogMessage(message, source, logLevelShort);

            // コンソールに出力
            WriteToConsole(formattedMessage, level);

            // ファイルに出力
            WriteToFile(formattedMessage);

            return Task.CompletedTask;
        }

        /// <summary>
        /// ログレベルの短縮名を取得します
        /// </summary>
        private string GetLogLevelShortName(LogLevel level)
        {
            return level switch
            {
                LogLevel.Fatal => "FTL",
                LogLevel.Error => "ERR",
                LogLevel.Alert => "ALT",
                LogLevel.Warning => "WRN",
                LogLevel.Notice => "NTC",
                LogLevel.Info => "INF",
                LogLevel.Debug => "DBG",
                LogLevel.Verbose => "VRB",
                _ => "???"
            };
        }

        /// <summary>
        /// ログメッセージを指定されたフォーマットに整形します
        /// </summary>
        private string FormatLogMessage(string message, string source, string levelShort)
        {
            // タイムゾーンを考慮した時刻フォーマット
            var now = DateTime.Now;
            string timestamp = $"{now:yyyy/MM/dd-HH:mm:ss}{GetTimezoneOffset()}";

            return $"[{timestamp}] [{levelShort}] [{source}] {message}";
        }

        /// <summary>
        /// タイムゾーンのオフセットを+HH:MM形式で取得します
        /// </summary>
        private string GetTimezoneOffset()
        {
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            string sign = offset.TotalMinutes >= 0 ? "+" : "-";
            return $"{sign}{Math.Abs(offset.Hours):D2}:{Math.Abs(offset.Minutes):D2}";
        }

        /// <summary>
        /// コンソールにメッセージを色付きで出力します
        /// </summary>
        private void WriteToConsole(string message, LogLevel level)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = level switch
            {
                LogLevel.Fatal => ConsoleColor.DarkMagenta,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Alert => ConsoleColor.DarkRed,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Notice => ConsoleColor.Cyan,
                LogLevel.Info => ConsoleColor.Green,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Verbose => ConsoleColor.DarkGray,
                _ => originalColor
            };

            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// ファイルにメッセージを出力します（スレッドセーフ）
        /// </summary>
        private void WriteToFile(string message)
        {
            lock (_fileLock)
            {
                string logFile = GetCurrentLogFileName();
                try
                {
                    File.AppendAllText(logFile, message + Environment.NewLine, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    // ファイル書き込みに失敗した場合はコンソールのみに出力
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Discord SDKからのログメッセージを処理します
        /// </summary>
        public Task LogDiscord(LogMessage message)
        {
            LogLevel level = ConvertDiscordLogLevel(message.Severity);
            string source = message.Source ?? "Discord";

            string logMessage = message.Exception != null
                ? $"{message.Message}: {message.Exception}"
                : message.Message;

            return Log(logMessage, source, level);
        }

        /// <summary>
        /// Discord SDKのログレベルを変換します
        /// </summary>
        private LogLevel ConvertDiscordLogLevel(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => LogLevel.Fatal,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Info,
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Verbose => LogLevel.Verbose,
                _ => LogLevel.Info
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
                return Log($"コマンド '{commandException.Command.Name}' でエラー発生: {commandException.Message}",
                    "Commands", LogLevel.Error);
            }

            return LogDiscord(message);
        }

        /// <summary>
        /// アプリケーション終了時に呼び出し、保留中のログを確実に書き込みます
        /// </summary>
        public void Shutdown()
        {
            // 必要に応じて、保留中のログをフラッシュするなどの処理を追加
            Log("ロギングサービスをシャットダウンしています", "LoggingService");
        }
    }
}
//memo: この修正により 初期化は _logger = new ImprovedLoggingService();
//Loggingには LoggingService.Log("Messsage", "Sorce",ImprovedLoggingService.LogLevel.info); へリファクタリングが必要になりました。