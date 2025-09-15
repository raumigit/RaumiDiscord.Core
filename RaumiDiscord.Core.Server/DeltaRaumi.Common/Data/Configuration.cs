using Discord;
using Nett;
using static RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration.ServerConfiguration;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Data
{
    class Configuration
    {
        string? _commandPrefix;

        //[TomlComment("")]

        public string? CommandPrefix
        {
            get => _commandPrefix;
            set => _commandPrefix = value;
        }

        public GeneralSettings? Setting { get; set; }
        public ConnectionServiceAppPath AppPath { get; set; }
        public SystemLogSettings? SystemLog { get; set; }
        public TokenSetting? TokenData { get; set; }

        public Configuration Config { get; set; }

        public TomlComment Comment { get; set; }
        

        public Configuration GetConfigFromFile()
        {
            if (!File.Exists(Directories.Config))
            {
                Initconfig();
            }
            Setconfig();

            string configFilePath = Directories.Config;
            var configToml = Toml.ReadFile<Configuration>(configFilePath);
            Config = configToml;
            return configToml;
        }

        public Configuration GetConfig()
        {
            string configFilePath = Directories.Config;
            var configToml = Toml.ReadFile<Configuration>(configFilePath);
            Config = configToml;
            return configToml;
        }

        public void Setconfig()
        {
            string configFilePath = Directories.Config;
            Configuration setcfg = Toml.ReadFile<Configuration>(configFilePath);

            setcfg.Setting.UpTime = DateTime.Now;
            setcfg.Setting.TimeZone = $"{TimeZoneInfo.Local}";
            setcfg.Setting.DiscordApiVersion = DiscordConfig.APIVersion.ToString();
            setcfg.Setting.DefaultRequestTimeout = DiscordConfig.DefaultRequestTimeout;
            setcfg.Setting.Cdnurl = DiscordConfig.CDNUrl;
            setcfg.Setting.InviteUrl = DiscordConfig.InviteUrl;
            setcfg.Setting.MaxApplicationDescriptionLength = DiscordConfig.MaxApplicationDescriptionLength;
            setcfg.Setting.MaxMessageSize = DiscordConfig.MaxMessageSize;
            setcfg.Setting.MaxPollAnswerTextLength = DiscordConfig.MaxPollAnswerTextLength;
            setcfg.Setting.MaxPollQuestionTextLength = DiscordConfig.MaxPollQuestionTextLength;
            setcfg.Setting.MaxVoiceChannelStatusLength = DiscordConfig.MaxVoiceChannelStatusLength;

            
            Toml.WriteFile(setcfg, Directories.Config);
        }

        public void Initconfig()
        {
            Configuration settings;

            TimeZoneInfo timeZone = TimeZoneInfo.Local;
            settings = new Configuration
            {

                // デフォルトの設定を作成
                Setting = new GeneralSettings
                {
                    UpdateTime = DateTime.Now,
                    UpTime = DateTime.Now,
                    TimeZone = $"{timeZone.DisplayName}",
                    DiscordApiVersion = "",
                    Cdnurl = "https://cdn.discordapp.com/",
                    DefaultRequestTimeout = 0,
                    InviteUrl = "https://discord.gg/",
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
                AppPath = new ConnectionServiceAppPath
                {
                    CoeiroinkAppPath = "",
                    LlmAppPath = "",
                    FfmpegPath = "",
                    YtdlpAppPath = ""
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
                // Configuration config = new();

                // TOMLファイルに書き込む
                Toml.WriteFile(settings, Directories.Config);
                Console.WriteLine("デフォルトの設定ファイルを作成しました。");
            }
        }
    }
}
