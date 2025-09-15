namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils
{
    using Database.JsonModels;
    using System.Text.Json;

    /// <summary>
    /// GameMetaServiceは、ゲームのメタデータを管理および提供するサービスです。
    /// </summary>
    public class GameMetaService
    {
        private GameCodeMetaData _config = new();

        /// <summary>
        /// GameMetaServiceのコンストラクタ
        /// </summary>
        /// <param name="configPath"></param>
        public GameMetaService(string configPath)
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<GameCodeMetaData>(json);
            if (config != null) _config = config;
        }

        /// <summary>
        /// GameMeta IDからゲームのメタデータを取得します。
        /// </summary>
        /// <param name="id">JsonからIDを取得</param>
        /// <returns></returns>
        public GameMeta? GetGame(int id) =>
            _config.Games.FirstOrDefault(g => g.Id == id);

        /// <summary>
        /// GameMetaのリストをすべて取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameMeta> GetAllGames() => _config.Games;
    }

}
