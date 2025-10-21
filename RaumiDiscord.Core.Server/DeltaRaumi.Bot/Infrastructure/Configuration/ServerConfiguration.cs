namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration
{
    /// <summary>
    /// ServerConfigurationは、サーバーの設定を管理するクラスです。
    /// </summary>
    public class ServerConfiguration : BaseApplicationConfiguration
    {
        /// <summary>
        /// 接続サービスの設定
        /// </summary>
        public GeneralSettings? Setting { get; set; }

        /// <summary>
        /// システムログの設定
        /// </summary>
        public SystemLogSettings? SystemLog { get; set; }

        /// <summary>
        /// トークン
        /// </summary>
        public TokenSetting? TokenData { get; set; }



        /// <summary>
        /// 接続サービスの設定
        /// </summary>
        public class GeneralSettings
        {
            /// <summary>
            /// 更新日
            /// </summary>
            public DateTime UpdateTime { get; set; }

            /// <summary>
            /// 起動した時間
            /// </summary>
            public DateTime UpTime { get; set; }

            /// <summary>
            /// タイムゾーン
            /// </summary>
            public required string TimeZone { get; set; }

            /// <summary>
            /// DiscordAPIバージョン
            /// </summary>
            public string? DiscordApiVersion { get; set; }

            /// <summary>
            /// DiscordcdnのURL
            /// </summary>
            public string? Cdnurl { get; set; }

            /// <summary>
            /// 既定のリクエストタイムアウト
            /// </summary>
            public int DefaultRequestTimeout { get; set; }

            /// <summary>
            /// 招待URL
            /// </summary>
            public string? InviteUrl { get; set; }

            /// <summary>
            /// アプリケーションの説明の最大長
            /// </summary>
            public int MaxApplicationDescriptionLength { get; set; }

            /// <summary>
            /// メッセージの最大サイズ
            /// </summary>
            public int MaxMessageSize { get; set; }

            /// <summary>
            /// 投票の回答の最大長
            /// </summary>
            public int MaxPollAnswerTextLength { get; set; }

            /// <summary>
            /// 投票の質問の最大長
            /// </summary>
            public int MaxPollQuestionTextLength { get; set; }

            /// <summary>
            /// ボイスチャンネルのステータスの最大長
            /// </summary>
            public int MaxVoiceChannelStatusLength { get; set; }

            /// <summary>
            /// Discordのクライアントタイプ
            /// </summary>
            public int ClientType { get; set; }

            /// <summary>
            /// 色
            /// </summary>
            public string? Color { get; set; }

            /// <summary>
            /// カスタムステータス
            /// </summary>
            public string? CustomStatusGame { get; set; }

            /// <summary>
            /// systemに関するエラーが発生した場合、trueにします。
            /// </summary>
            public bool SystemFatal { get; set; }
        }

        /// <summary>
        /// 接続サービスの設定
        /// </summary>
        public class ConnectionServiceAppPath
        {
            /// <summary>
            /// Coerioinkのパス
            /// </summary>
            public string CoeiroinkAppPath { get; set; }

            /// <summary>
            /// Coerioinkのパス
            /// </summary>
            public string LlmAppPath { get; set; }

            /// <summary>
            /// ffmpegのパス
            /// </summary>
            public string FfmpegPath { get; set; }

            /// <summary>
            /// ytdlpのパス
            /// </summary>
            public string YtdlpAppPath { get; set; }
        }

        /// <summary>
        /// システムログの設定
        /// </summary>
        public class SystemLogSettings
        {
            /// <summary>
            /// Discord Botのログレベル
            /// </summary>
            public string? LogFilePath { get; set; }

            /// <summary>
            /// Discord WebHook URL
            /// </summary>
            public string? LogDataWebHook { get; set; }
        }

        /// <summary>
        /// Discord Bot Token
        /// </summary>
        public class TokenSetting
        {
            /// <summary>
            /// Discord Bot Token
            /// </summary>
            public required string Token { get; set; }
        }
    }
}