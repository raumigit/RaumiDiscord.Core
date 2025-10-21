namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Enums
{
    /// <summary>
    /// ゲームのギフトコードの種類を定義する列挙型です。
    /// 将来的に定義は変更される場合があります。
    /// </summary>
    public enum GameGiftCodeType
    {
        /// <summary>
        /// URLのみが該当します。
        /// </summary>
        Url = 0,
        /// <summary>
        /// 原神のギフトコードを表します。
        /// </summary>
        GenshinImpact = 1,
        /// <summary>
        /// 崩壊：スターレイルのギフトコードを表します。
        /// </summary>
        HonkaiStarRail = 2,
        /// <summary>
        /// ゼンレス・ゾーン・ゼロのギフトコードを表します。
        /// </summary>
        ZenlessZoneZero = 3,
        /// <summary>
        /// 鳴潮のギフトコードを表します。
        /// </summary>
        WutheringWaves = 4,
        /// <summary>
        /// ブルーアーカイブのギフトコードを表します。
        /// </summary>
        BlueArchive = 5,
        /// <summary>
        /// レゾナンスのギフトコードを表します。
        /// </summary>
        Resonance = 6,
    }
}
