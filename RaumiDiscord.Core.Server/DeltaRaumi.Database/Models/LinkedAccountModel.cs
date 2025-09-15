using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// LinkedAccountModel は、ユーザーのメインアカウントとサブアカウントのリンク情報を管理するためのデータモデルです。
    /// </summary>
    public class LinkedAccountModel
    {
        /// <summary>
        /// Id は、リンク情報の一意の識別子です。
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// ParentUserId は、メインアカウントのユーザーIDを表します。
        /// </summary>
        [MaxLength(50)]
        public string ParentUserId { get; set; }
        /// <summary>
        /// SubUserId は、サブアカウントのユーザーIDを表します。
        /// </summary>
        [MaxLength(50)]
        public string SubUserId { get; set; }

        /// <summary>
        /// LinkedAt は、アカウントがリンクされた日時を表します。
        /// </summary>
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
