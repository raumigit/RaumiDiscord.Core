namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.JsonModels
{

    /// <summary>
    /// ゲームのメタデータを表します。
    /// </summary>
    public class GameMeta
    {
        /// <summary>
        /// ゲームの一意の識別子です。
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ゲームの名前です。
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// ゲームの基本URLです。
        /// </summary>
        public string? BaseUrl { get; set; }
        /// <summary>
        /// ゲームのストアリンクを表します。
        /// </summary>
        public StoreLinks Store { get; set; } = new();
    }

    /// <summary>
    /// ゲームのストアリンクを表します。
    /// </summary>
    public class StoreLinks
    {
        /// <summary>
        /// Google Play ストアのリンクです。
        /// </summary>
        public string? Google { get; set; }
        /// <summary>
        /// Apple App Store のリンクです。
        /// </summary>
        public string? Apple { get; set; }
    }

    /// <summary>
    /// ゲームのメタデータのコレクションを表します。
    /// </summary>
    public class GameCodeMetaData
    {
        /// <summary>
        /// ゲームのメタデータのリストです。
        /// </summary>
        public List<GameMeta> Games { get; set; } = new();
    }
}
