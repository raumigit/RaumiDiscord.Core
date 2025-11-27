using Discord;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Dtos
{
    /// <summary>
    /// ギルド情報の API 用 DTO
    /// </summary>
    public class GuildDto
    {
        public string GuildId { get; set; } = default!;           // 必須
        public string GuildName { get; set; } = default!;
        public string? IconUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string OwnerId { get; set; } = default!;
        public string? WelcomeChannnelId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public ulong MaxUploadLimit { get; set; }
        public int MemberCount { get; set; }
        public int PremiumSubscriptionCount { get; set; }
        public PremiumTier PremiumTier { get; set; }

        // API で公開したくない場合は除外
        public string? LogChannel { get; set; }      // 必要に応じてコメントアウト
        public string? OpenLogChannel { get; set; }  // 同上
    }
}
