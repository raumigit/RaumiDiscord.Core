using Nett;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaumiDiscord.Core.Server.DiscordBot.Data;
using static RaumiDiscord.Core.Server.DiscordBot.Config;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    class Config
    {
        string commandPrefix;
        private string token;
        public string Token
        {
            get => token;
            set => token = value;
        }
        public string CommandPrefix
        {
            get => commandPrefix;
            set => commandPrefix = value;
        }
        public GeneralSettings Setting { get; set; }
        public SystemLogSettings SystemLog { get; set; }
        public class GeneralSettings
        {
            public DateTime UpdateTime { get; set; }
            public string TimeZone { get; set; }
            public string DiscordAPIVersion { get; set; }
            public string CDNURL { get; set; }
            public int DefaultRequestTimeout { get; set; }
            public string InviteURL { get; set; }
            public int MaxApplicationDescriptionLength { get; set; }
            public int MaxMessageSize { get; set; }
            public int MaxPollAnswerTextLength { get; set; }
            public int MaxPollQuestionTextLength { get; set; }
            public int MaxVoiceChannelStatusLength { get; set; }
            public int ClientType { get; set; }
            public string Color { get; set; }
            public string CustomStatusGame { get; set; }
            public bool SystemFatal { get; set; }
        }

        public class SystemLogSettings
        {
            public string LogFilePath { get; set; }
            public string LogDataWebHook { get; set; }
        }

        public string GetConfigToken()
        {
            var toml = Toml.ReadFile("F:/ProgramData/Deltaraumi.toml");
            var environmentalValue = toml.Get<TomlTable>("EnvironmentalValue");
            return token = environmentalValue.Get<string>("Tokun");
            
        }
        public Config GetConfigFromFile()
        {

            
            Config settings;

            System.TimeZoneInfo tzi = System.TimeZoneInfo.Local;
            settings = new Config
            {
                // デフォルトの設定を作成
                Setting = new Config.GeneralSettings
                {
                    UpdateTime = DateTime.Now,
                    TimeZone = $"{tzi.DisplayName}",
                    DiscordAPIVersion = "",
                    CDNURL = "https://cdn.discordapp.com/",
                    DefaultRequestTimeout = 0,
                    InviteURL = "https://discord.gg/",
                    MaxApplicationDescriptionLength = 0,
                    MaxMessageSize = 0,
                    MaxPollAnswerTextLength = 0,
                    MaxPollQuestionTextLength = 0,
                    MaxVoiceChannelStatusLength = 0,
                    ClientType = 0,
                    Color = "0xFFFFFF",
                    CustomStatusGame = "",
                    SystemFatal = false
                },
                SystemLog = new Config.SystemLogSettings
                {
                    LogFilePath = "",
                    LogDataWebHook = ""
                }
            };

            if (!Directory.Exists(Directories.ProgramData)) Directory.CreateDirectory(Directories.ProgramData);
            if (File.Exists(Directories.Config))
            {

                return Toml.ReadFile<Config>(Directories.Config);

            }
            else
            {
                var config = new Config();

                // TOMLファイルに書き込む
                Toml.WriteFile(settings, Directories.Config);
                Console.WriteLine("デフォルトの設定ファイルを作成しました。");
                return config;
            }
        }

    }
}
 