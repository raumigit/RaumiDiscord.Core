using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// ユーザーのギルドデータを表します。
    /// </summary>

    public class UserGuildDataModel
    {
        /// <summary>
        /// UserGuildDataModelのコンストラクターです。
        /// </summary>
        public UserGuildDataModel()
        {
            SetToMention = -1;
        }

        /// <summary>
        /// ギルドの基本データへの外部キー参照を取得または設定します。
        /// </summary>
        [ForeignKey("GuildId")]
        public required GuildBaseDataModel GuildBaseData { get; set; }

        /// <summary>
        /// ユーザーの基本データへの外部キー参照を取得または設定します。
        /// </summary>
        [ForeignKey("UserId")]
        public required UserBaseDataModel UserBaseData { get; set; }

        /// <summary>
        /// ギルドユーザーを管理する一意のID
        /// </summary>
        [Key]
        public Guid GuId { get; set; }

        /// <summary>
        /// ギルドの一意識別子を取得または設定します。
        /// </summary>
        [MaxLength(50)]
        public string GuildId { get; set; }

        /// <summary>
        /// ユーザーの一意識別子を取得または設定します。
        /// </summary>
        [MaxLength(50)]
        public string UserId { get; set; }

        /// <summary>
        /// このギルド内でのユーザーのアバターIDを取得または設定します。
        /// </summary>
        [MaxLength(256)]
        public string? GuildAvatarId { get; set; }

        /// <summary>
        /// ギルド内でのユーザーのフラグを取得または設定します。
        /// </summary>
        public int GuildUserFlags { get; set; }

        /// <summary>
        /// ユーザーがギルドに参加した日時を取得または設定します。
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// ユーザーのタイムアウトが終了する日時を取得または設定します。
        /// </summary>
        public DateTime TimedOutUntil { get; set; }

        /// <summary>
        /// ユーザーのギルド内でのExpを取得または設定します。
        /// </summary>
        public int GuildExp { get; set; }

        /// <summary>
        /// ユーザーが最後に経験値を得た時間を取得または設定します。
        /// </summary>
        public DateTime LatestExp { get; set; }

        /// <summary>
        /// ユーザーがギルド内で送信したメッセージの総数を取得または設定します。
        /// </summary>
        public int TotalMessage { get; set; }

        /// <summary>
        /// ユーザーが受け取ることのできるメンションの上限数を取得または設定します。
        /// </summary>
        public int SetToMention { get; set; }

    }
}