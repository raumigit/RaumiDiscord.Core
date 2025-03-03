using RaumiDiscord.Core.Server.DiscordBot.Data;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Models.Configuration
{
    public class ServerConfiguration : BaseApplicationConfiguration
    {
        public GeneralSettings? Setting { get; set; }
        public SystemLogSettings? SystemLog { get; set; }
        public TokenSetting? TokenData { get; set; }



        public class GeneralSettings
        {
            public DateTime UpdateTime { get; set; }
            public DateTime UpTime { get; set; }
            public required string TimeZone { get; set; }
            public string? DiscordAPIVersion { get; set; }
            public string? CDNURL { get; set; }
            public int DefaultRequestTimeout { get; set; }
            public string? InviteURL { get; set; }
            public int MaxApplicationDescriptionLength { get; set; }
            public int MaxMessageSize { get; set; }
            public int MaxPollAnswerTextLength { get; set; }
            public int MaxPollQuestionTextLength { get; set; }
            public int MaxVoiceChannelStatusLength { get; set; }
            public int ClientType { get; set; }
            public string? Color { get; set; }
            public string? CustomStatusGame { get; set; }
            public bool SystemFatal { get; set; }
        }
        public class ConnectionServiceAppPath
        {
            public string CoeiroinkAppPath { get; set; }
            public string LLMAppPath { get; set; }
            public string ffmpegPath { get; set; }
            public string ytdlpAppPath { get; set; }
        }
        public class SystemLogSettings
        {
            public string? LogFilePath { get; set; }
            public string? LogDataWebHook { get; set; }
        }
        public class TokenSetting
        {
            public required string Token { get; set; }
        }
    }
}