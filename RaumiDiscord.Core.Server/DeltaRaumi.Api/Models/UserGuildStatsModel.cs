using NUlid;
using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.Api.Models
{
    /// <summary>
    /// Discordの統計データを表すモデルクラスです。
    /// </summary>
    public class UserGuildStatsModel
    {
        /// <summary>
        /// 統計データの一意識別子としてUlidを取得または設定します。
        /// </summary>
        [Key]
        public Ulid StatUlid { get; set; }

        /// <summary>
        /// 統計データが関連するギルドの一意識別子を取得または設定します。
        /// </summary>
        public string GuildId { get; set; }

        /// <summary>
        /// 統計データが関連するユーザーの一意識別子を取得または設定します。
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 統計データが作成された日時を取得または設定します。
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public string MentionedUserId { get; set; }
        
    }

    
}