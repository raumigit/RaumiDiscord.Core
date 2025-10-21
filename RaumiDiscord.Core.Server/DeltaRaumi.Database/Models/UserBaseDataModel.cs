using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// ユーザーの基本データを表します。
    /// </summary>
    public class UserBaseDataModel
    {
        /// <summary>
        /// UserBaseDataModelのコンストラクターです。
        /// </summary>
        public UserBaseDataModel()
        {
        }

        /// <summary>
        /// このユーザーのIdを取得または設定します。
        /// この値はulongである必要があります。
        /// </summary>
        /// <remarks>注意：SocketMessage Author.Idで取得してください。</remarks>
        [Key]
        [MaxLength(50)]
        public required string UserId { get; set; }

        /// <summary>
        /// このユーザーのユーザー名を取得または設定します。
        /// </summary>
        [MaxLength(50)]
        public string? UserName { get; set; }

        /// <summary>
        /// このユーザーのアバターの識別子を取得または設定します。
        /// </summary>
        [MaxLength(256)]
        public string? AvatarId { get; set; }

        /// <summary>
        /// スノーフレークが作成された日時を取得または設定します。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// このユーザーがボットとして識別されるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsBot { get; set; }

        /// <summary>
        /// このユーザーが Webhook ユーザーであるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsWebhook { get; set; }

        /// <summary>
        /// このユーザーのExpを取得または設定します。
        /// </summary>
        public int Exp { get; set; }

        /// <summary>
        /// このユーザーの誕生日を取得または設定します。
        /// 年が使われることはありません。
        /// </summary>
        [MaxLength(32)]
        public string? Barthday { get; set; }

        /// <summary>
        /// UserStatusKindに基づいてユーザーのステータスを取得または設定します。
        ///</summary>
        public int Userstatus { get; set; }

        /// <summary>
        /// ユーザーが受け取ることのできるメンションの上限数を取得または設定します。
        /// </summary>
        public int SetToMention { get; set; }
    }
}