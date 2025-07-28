namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration
{
    /// <summary>
    /// BaseApplicationConfigurationは、アプリケーションの基本設定を管理するクラスです。
    /// </summary>
    public class BaseApplicationConfiguration
    {
        /// <summary>
        /// BaseApplicationConfigurationのコンストラクタ
        /// </summary>
        public BaseApplicationConfiguration()
        {
            LogFileRetentionDays = 7;
        }

        /// <summary>
        /// ログファイルの保持期間　この期間中は自動でログは消えません
        /// </summary>
        public int LogFileRetentionDays { get; set; }

        /// <summary>
        /// スタートアップが完了しているかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsStaetupWizardCompleted { get; set; }

        /// <summary>
        /// キャッシュのパスを取得または設定します。
        /// </summary>
        public string CachePath { get; set; }

        /// <summary>
        /// 前のアプリケーションのバージョンを取得または設定します。
        /// </summary>
        public Version? PreviousVersion { get; set; }

        /// <summary>
        /// 前のアプリケーションのバージョンを文字列として取得または設定します。
        /// </summary>
        public string? PreviousVersionStr
        {
            get => PreviousVersion?.ToString();
            set
            {
                if (Version.TryParse(value, out var version))
                {
                    PreviousVersion = version;
                }
            }
        }
    }
}