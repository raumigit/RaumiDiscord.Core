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

        /// <summary>
        /// メンションされた場合のユーザーのコレクションを取得または設定します。
        /// </summary>
        public ICollection<MentionedUsers>? Mentioneds { get; set; }
    }

    /// <summary>
    /// メンションされたユーザーを表すモデルクラスです。
    /// </summary>
    public class MentionedUsers
    {
        /// <summary>
        /// MentionedUsersのコンストラクターです。
        /// </summary>
        public MentionedUsers()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// メンションしたユーザーの一意の識別子を取得または設定します。
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// データの作成日時を取得または設定します。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// メンションされたユーザーのIdを取得または設定します。
        /// </summary>
        public int MentionedUserId { get; set; }
    }
}