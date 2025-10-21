using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils
{
    public class GameMetaService
    {
        private readonly List<GameConfig> _games;
        private readonly ImprovedLoggingService _logger;

        public GameMetaService(ImprovedLoggingService logger)
        {
            _logger = logger;
            _games = new List<GameConfig>();
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "GameCodeMetaData.json");

                if (!File.Exists(configPath))
                {
                    _logger.Log($"GameCodeMetaData.jsonが見つかりません: {configPath}","");
                    _games.Add(new GameConfig
                    {
                        Id = 0,
                        Name = "Url",
                        Shortname = "url",
                        BaseUrl = null,
                        Store = new StoreLinks { Google = null, Apple = null }
                    });
                    return;
                }

                var jsonContent = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var config = JsonSerializer.Deserialize<GameConfigRoot>(jsonContent, options);

                if (config?.Games != null)
                {
                    _games.AddRange(config.Games);
                    _logger.Log($"{_games.Count}個のゲーム設定を読み込みました。", "GameCode");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"GameCodeMetaData.jsonの読み込みに失敗しました。デフォルト設定を使用します。\n{ex}", "GameCode", ImprovedLoggingService.LogLevel.Error);
                _games.Add(new GameConfig
                {
                    Id = 0,
                    Name = "Url",
                    Shortname = "url",
                    BaseUrl = null,
                    Store = new StoreLinks { Google = null, Apple = null }
                });
            }
        }

        public GameConfig? FindGame(string nameOrShortname)
        {
            if (string.IsNullOrEmpty(nameOrShortname)) return null;

            return _games.FirstOrDefault(g =>
                g.Name.Equals(nameOrShortname, StringComparison.OrdinalIgnoreCase) ||
                g.Shortname.Equals(nameOrShortname, StringComparison.OrdinalIgnoreCase));
        }

        public List<GameConfig> GetAllGames()
        {
            return _games.ToList();
        }
    }

    public class GameConfigRoot
    {
        [JsonPropertyName("games")]
        public List<GameConfig> Games { get; set; } = new();
    }

    public class GameConfig
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("shortname")]
        public string Shortname { get; set; } = string.Empty;

        [JsonPropertyName("baseUrl")]
        public string? BaseUrl { get; set; }

        [JsonPropertyName("store")]
        public StoreLinks? Store { get; set; }
    }

    public class StoreLinks
    {
        [JsonPropertyName("google")]
        public string? Google { get; set; }

        [JsonPropertyName("apple")]
        public string? Apple { get; set; }
    }
}