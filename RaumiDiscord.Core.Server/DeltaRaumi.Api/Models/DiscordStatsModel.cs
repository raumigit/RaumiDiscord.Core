using NUlid;

namespace RaumiDiscord.Core.Server.Api.Models
{
    /// <summary>
    /// Discordの統計データを表すモデルクラスです。
    /// </summary>
    public class DiscordStatsModel
    {
        /// <summary>
        /// 統計データの一意識別子としてUlidを取得または設定します。
        /// </summary>
        public Ulid ulid { get; set; }

        /// <summary>
        /// 統計データが関連するギルドの一意識別子を取得または設定します。
        /// </summary>
        public string guildid { get; set; }

        /// <summary>
        /// 統計データが関連するユーザーの一意識別子を取得または設定します。
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 統計データが作成された日時を取得または設定します。
        /// </summary>
        public DateTime createdAt { get; set; } = DateTime.Now;
    }
}