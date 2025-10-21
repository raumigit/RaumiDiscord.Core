using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// Discordコンポーネントモデルを表します。
    /// </summary>
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
        [MaxLength(50)]
        public string OwnerId { get; set; } = "0";
        
        /// <summary>
        /// コンポーネントが所属するチャンネルIDを取得または設定します。初期値は"0"です。
        /// </summary>
        [MaxLength(50)]
        public string ChannelId { get; set; } = "0";

        /// <summary>
        /// コンポーネントが関連付けられたメッセージIDを取得または設定します。初期値は"0"です。
        /// </summary>
        [MaxLength(50)]
        public string MessageId { get; set; } = "0";

        /// <summary>
        /// DeltaRaumiコンポーネントのタイプを取得または設定します。
        /// </summary>
        [MaxLength(256)]
        public string? DeltaRaumiComponentType { get; set; }

        /// <summary>
        /// LinkCodeを取得または設定します。
        /// </summary>
        [MaxLength(256)]
        public string? LinkCode { get; set; }

        /// <summary>
        /// TimeToLiveを取得または設定します。初期値は現在のUTC時間から15分後です。
        /// </summary>
        public DateTime? TimeToLive { get; set; } = DateTime.UtcNow.AddMinutes(15);
    }
}