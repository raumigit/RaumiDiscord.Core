namespace RaumiDiscord.Core.Server.DiscordBot.Data
{
    internal class Directories
    {
        public static readonly Directories Instance = new Directories();
        public static string AppData 
        { 
            get 
            { 
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi{Path.DirectorySeparatorChar}"; 
            } 
        }
        public static string Config
        {
            get
            {
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi{Path.DirectorySeparatorChar}config.toml";
            } 
        }

        public static string ProgramData 
        { 
            get 
            { 
                return $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{Path.DirectorySeparatorChar}Raumi{Path.DirectorySeparatorChar}DeltaRaumi"; 
            } 
        }
        public static string DiscordToken
        {
            get
            {
                return $"F:/ProgramData/Deltaraumi.toml";
            }
        }
    }
}