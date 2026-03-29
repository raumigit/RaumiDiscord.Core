namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.ApplicationStartup
{
    public interface IServerStartup
    {
        string ApplicationName { get; }
        bool HasPendingRestart { get; }
        bool ShouldRestart { get; set; }
        Version ApplicationVersion { get; }
        string ApplicationVersionString { get; set; }
    }
}
