namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.JsonModels
{

    public class GameMeta
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? BaseUrl { get; set; }
        public StoreLinks Store { get; set; } = new();
    }

    public class StoreLinks
    {
        public string? Google { get; set; }
        public string? Apple { get; set; }
    }

    public class GameCodeMetaData
    {
        public List<GameMeta> Games { get; set; } = new();
    }
}
