namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Dtos
{
    /// <summary>
    /// URL エントリーの API 用 DTO
    /// </summary>
    public class UrlDataModelsDto
    {
        // ① 一意識別子 (必須)
        public uint Id { get; set; }

        // ② 実際に保存されている URL（nullable はそのまま）
        public string? Url { get; set; }

        // ③ URL の種類。DB では string ですが、API 側で enum に変換したい場合は
        /// <summary>
        /// URL の種類を表す文字列。必要に応じて enum に変換して使用してください。
        /// </summary>
        public string UrlType { get; set; } = default!;

        // ④ Discord ユーザー ID（nullable）
        public string? DiscordUser { get; set; }

        // ⑤ 有効期限
        public DateTime Ttl { get; set; }

        // ⑥ 公開状態
        public bool Publish { get; set; }
    }
}
