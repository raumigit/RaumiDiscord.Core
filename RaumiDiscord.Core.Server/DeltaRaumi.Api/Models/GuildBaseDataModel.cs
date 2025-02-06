using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace RaumiDiscord.Core.Server.Api.Models
{
    public class GuildBaseData
    {
        /// <summary>
        ///  ギルドのIdを取得または設定します。
        /// </summary>
        [Key]
        public required ulong GuildId { get; set; }

        /// <summary>
        ///  ギルドの名前を取得または設定します。
        /// </summary>
        public required string GuildName { get; set; }

        /// <summary>
        ///  アイコンのUrlを取得または設定します。
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        ///  バナーのUrlを取得または設定します。
        /// </summary>
        public string? BannerUrl { get; set; }

        /// <summary>
        ///  ギルドオーナーの取得または設定します。
        /// </summary>
        public ulong OwnerID { get; set; }

        /// <summary>
        ///  ウェルカムチャンネルのIdを取得または設定します。
        /// </summary>
        public ulong? WelcomeChannnelID { get; set; }

        /// <summary>
        ///  ギルドの作成日時を取得または設定します。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///  ギルドの説明を取得または設定します。
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        ///  ギルドのアップロード制限をバイト単位で取得または設定します。
        ///  この数値はギルドのブースト ステータスに依存します。
        /// </summary>
        public ulong MaxUploadLimit { get; set; }

        /// <summary>
        ///  メンバーの数を取得または設定します。
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        ///  このギルドのプレミアム加入者の数を取得または設定します。
        /// </summary>
        public int PremiumSubscriptionCount { get; set; }

        /// <summary>
        ///  このギルドのギルドブーストのティアを取得または設定します。
        /// </summary>
        public PremiumTier PremiumTier { get; set; }
    }
}
