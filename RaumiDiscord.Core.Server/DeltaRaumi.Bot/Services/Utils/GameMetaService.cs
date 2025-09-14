namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils
{
    using RaumiDiscord.Core.Server.DeltaRaumi.Database.JsonModels;
    using System.Text.Json;

    public class GameMetaService
    {
        private GameCodeMetaData _config = new();

        public GameMetaService(string configPath)
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<GameCodeMetaData>(json);
            if (config != null) _config = config;
        }

        public GameMeta? GetGame(int id) =>
            _config.Games.FirstOrDefault(g => g.Id == id);

        public IEnumerable<GameMeta> GetAllGames() => _config.Games;
    }

}
