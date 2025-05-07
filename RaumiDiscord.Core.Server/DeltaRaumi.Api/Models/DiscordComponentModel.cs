namespace RaumiDiscord.Core.Server.Api.Models
{
    public class DiscordComponentModel
    {
        /// <summary>
        /// コンポーネントの一意識別子を取得または設定します。
        /// </summary>
        public Guid CustomId { get; set; }

        /// <summary>
        /// コンポーネントの作成時間（ローカル時間）を取得または設定します。
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// コンポーネントのオーナーIDを取得または設定します。初期値は"0"です。
        /// </summary>
        public string OwnerId { get; set; } = "0";

        /// <summary>
        /// コンポーネントが所属するチャンネルIDを取得または設定します。初期値は"0"です。
        /// </summary>
        public string ChannelId { get; set; } = "0";

        /// <summary>
        /// コンポーネントが関連付けられたメッセージIDを取得または設定します。初期値は"0"です。
        /// </summary>
        public string MessageId { get; set; } = "0";

        /// <summary>
        /// DeltaRaumiコンポーネントのタイプを取得または設定します。
        /// </summary>
        public string? DeltaRaumiComponentType { get; set; }
    }
}