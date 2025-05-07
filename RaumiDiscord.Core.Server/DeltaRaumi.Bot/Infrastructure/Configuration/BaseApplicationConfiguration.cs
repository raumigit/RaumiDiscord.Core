namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration
{
    public class BaseApplicationConfiguration 
    {
        public BaseApplicationConfiguration()
        {
            LogFileRetentionDays = 7;
        }
        /// <summary>
        /// ログファイルの保持期間　この期間中は自動でログは消えません
        /// </summary>
        public int LogFileRetentionDays { get; set; }

        public bool IsStaetupWizardCompleted { get; set; }

        public string CachePath { get; set; }

        public Version? PreviousVersion { get; set; }

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