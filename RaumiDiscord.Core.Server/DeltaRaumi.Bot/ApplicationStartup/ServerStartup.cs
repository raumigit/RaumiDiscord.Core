namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.ApplicationStartup
{

    public class ServerStartup : IServerStartup
    {
        public ServerStartup()
        {
            var version = typeof(ServerStartup).Assembly.GetName().Version;
            ApplicationVersion = version ?? new Version(0, 0, 0);
            ApplicationVersionString = ApplicationVersion.ToString(3);

        }

        public string ApplicationName { get; } = "DeltaRaumi";
        public bool HasPendingRestart { get; } = false;
        public bool ShouldRestart { get; set; } = false;
        public Version ApplicationVersion { get; }
        public string ApplicationVersionString { get; set; }
    }
}