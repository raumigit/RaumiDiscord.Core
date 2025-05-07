using Discord;
using Nett;
using static RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration.ServerConfiguration;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Data
{
    class Configuration
    {
        string? commandPrefix;


        public string? CommandPrefix
        {
            get => commandPrefix;
            set => commandPrefix = value;
        }

        public GeneralSettings? Setting { get; set; }
        public ConnectionServiceAppPath appPath { get; set; }
        public SystemLogSettings? SystemLog { get; set; }
        public TokenSetting? TokenData { get; set; }

        public Configuration _Config { get; set; }

        

        public Configuration GetConfigFromFile()
        {
            if (!File.Exists(Directories.Config))
            {
                initconfig();
            }
            setconfig();

            string config_FilePath = Directories.Config;
            var configToml = Toml.ReadFile<Configuration>(config_FilePath);
            _Config = configToml;
            return configToml;
        }

        public Configuration GetConfig()
        {
            string config_FilePath = Directories.Config;
            var configToml = Toml.ReadFile<Configuration>(config_FilePath);
            _Config = configToml;
            return configToml;
        }

        public void setconfig()
        {
            string config_FilePath = Directories.Config;
            Configuration setcfg = Toml.ReadFile<Configuration>(config_FilePath);

            setcfg.Setting.UpTime = DateTime.Now;
            setcfg.Setting.TimeZone = $"{TimeZoneInfo.Local}";
            setcfg.Setting.DiscordAPIVersion = DiscordConfig.APIVersion.ToString();
            setcfg.Setting.DefaultRequestTimeout = DiscordConfig.DefaultRequestTimeout;
            setcfg.Setting.CDNURL = DiscordConfig.CDNUrl;
            setcfg.Setting.InviteURL = DiscordConfig.InviteUrl;
            setcfg.Setting.MaxApplicationDescriptionLength = DiscordConfig.MaxApplicationDescriptionLength;
            setcfg.Setting.MaxMessageSize = DiscordConfig.MaxMessageSize;
            setcfg.Setting.MaxPollAnswerTextLength = DiscordConfig.MaxPollAnswerTextLength;
            setcfg.Setting.MaxPollQuestionTextLength = DiscordConfig.MaxPollQuestionTextLength;
            setcfg.Setting.MaxVoiceChannelStatusLength = DiscordConfig.MaxVoiceChannelStatusLength;

            Toml.WriteFile(setcfg, Directories.Config);
        }

        public void initconfig()
        {
            Configuration settings;

            TimeZoneInfo tzi = TimeZoneInfo.Local;
            settings = new Configuration
            {
                // デフォルトの設定を作成
                Setting = new GeneralSettings
                {
                    UpdateTime = DateTime.Now,
                    UpTime = DateTime.Now,
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
                appPath = new ConnectionServiceAppPath
                {
                    CoeiroinkAppPath="",
                    LLMAppPath="",
                    ffmpegPath="",
                    ytdlpAppPath=""
                },
                SystemLog = new SystemLogSettings
                {
                    LogFilePath = "",
                    LogDataWebHook = ""
                },
                TokenData = new TokenSetting
                {
                    Token = ""
                }
            };

            if (!Directory.Exists(Directories.ProgramData)) Directory.CreateDirectory(Directories.ProgramData);
            if (File.Exists(Directories.Config))
            {
                Toml.ReadFile<Configuration>(Directories.Config);
            }
            else
            {
                Configuration config = new();

                // TOMLファイルに書き込む
                Toml.WriteFile(settings, Directories.Config);
                Console.WriteLine("デフォルトの設定ファイルを作成しました。");
            }
        }
    }
}
