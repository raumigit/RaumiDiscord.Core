namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Data
{
    internal class Directories
    {
        public static readonly Directories Instance = new Directories();

        public static string MySqlConfigPath => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi{Path.DirectorySeparatorChar}mysqlconfig.json";

        public static string AppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi{Path.DirectorySeparatorChar}";

        public static string Config => $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi{Path.DirectorySeparatorChar}config.toml";

        public static string ProgramData => $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi";

        public static string DiscordToken => $"F:/ProgramData/Deltaraumi.toml";
    }
}