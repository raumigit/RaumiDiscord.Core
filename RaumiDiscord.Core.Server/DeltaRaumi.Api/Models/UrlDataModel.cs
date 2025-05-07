namespace RaumiDiscord.Core.Server.Api.Models
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
        public string? Url { get; set; }

        /// <summary>
        /// URLの種類を取得または設定します。
        /// </summary>
        public string? UrlType { get; set; }

        /// <summary>
        /// URLに関連付けられたDiscordユーザーIDを取得または設定します。
        /// </summary>
        public string? DiscordUser { get; set; }

        /// <summary>
        /// URLの有効期限（Time To Live）を取得または設定します。
        /// </summary>
        public DateTime TTL { get; set; }
        /// <summary>
        /// 保存されたURLの公開状態を取得または設定します。
        /// </summary>
        public bool publish { get; set; }
    }
}