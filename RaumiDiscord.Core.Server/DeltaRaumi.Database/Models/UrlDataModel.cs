using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// URLデータモデルを表します。
    /// </summary>
    public class UrlDataModel
    {
        /// <summary>
        /// URLエントリーの一意識別子を取得または設定します。
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// 保存されたURLを取得または設定します。
        /// </summary>
        [MaxLength(65535)]
        public string? Url { get; set; }

        /// <summary>
        /// URLの種類を取得または設定します。
        /// ここはStringsからintに変更されています。
        /// </summary>
        public int UrlType { get; set; }

        /// <summary>
        /// URLに関連付けられたDiscordユーザーIDを取得または設定します。
        /// </summary>
        [MaxLength(50)]
        public string? DiscordUser { get; set; }

        /// <summary>
        /// URLの有効期限（Time To Live）を取得または設定します。
        /// </summary>
        public DateTime Ttl { get; set; }
        /// <summary>
        /// 保存されたURLの公開状態を取得または設定します。
        /// </summary>
        public bool Publish { get; set; }
    }
}