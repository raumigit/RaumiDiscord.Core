namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old
{
    /// <summary>
    /// VoicertcregionServiceは、ボイスチャンネルのRTCリージョンを変更するためのサービスです。
    /// </summary>
    public class VoicertcregionService
    {
        /// <summary>
        /// ボイスチャンネルのRTCリージョンを変更する
        /// </summary>
        public static HashSet<ulong> AllowedRoleIds = new HashSet<ulong>
        {
            1329621030637015040,    //ハードコート：ロール名：アプリ開発(ラウミの裏小屋)
            1157017168471400682     //ハードコート：ロール名：VCモデレーター(ラウミの裏小屋)
        };
    }
}
