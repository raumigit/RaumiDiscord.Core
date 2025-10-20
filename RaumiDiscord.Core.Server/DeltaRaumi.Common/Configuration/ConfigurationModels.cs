using Discord;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Configuration;
using System;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models
{
    // 既存のTOMLファイル構造に合わせてクラス名を維持
    /// <summary>
    /// 
    /// </summary>
    public class GeneralSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [TomlValidation(Required = true, DefaultValue = "!")]
        public string CommandPrefix { get; set; } = "!";

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string TimeZone { get; set; } = TimeZoneInfo.Local.DisplayName;
        /// <summary>
        /// 
        /// </summary>
        public string DiscordApiVersion { get; set; } = DiscordConfig.APIVersion.ToString();
        /// <summary>
        /// 
        /// </summary>
        public string CdnUrl { get; set; } = DiscordConfig.CDNUrl;
        /// <summary>
        /// 
        /// </summary>
        public int DefaultRequestTimeout { get; set; } = DiscordConfig.DefaultRequestTimeout;
        /// <summary>
        /// 
        /// </summary>
        public string InviteUrl { get; set; } = DiscordConfig.InviteUrl;
        /// <summary>
        /// 
        /// </summary>
        public int MaxApplicationDescriptionLength { get; set; } = DiscordConfig.MaxApplicationDescriptionLength;
        /// <summary>
        /// 
        /// </summary>
        public int MaxMessageSize { get; set; } = DiscordConfig.MaxMessageSize;
        /// <summary>
        /// 
        /// </summary>
        public int MaxPollAnswerTextLength { get; set; } = DiscordConfig.MaxPollAnswerTextLength;
        /// <summary>
        /// 
        /// </summary>
        public int MaxPollQuestionTextLength { get; set; } = DiscordConfig.MaxPollQuestionTextLength;
        /// <summary>
        /// 
        /// </summary>
        public int MaxVoiceChannelStatusLength { get; set; } = DiscordConfig.MaxVoiceChannelStatusLength;
        /// <summary>
        /// 
        /// </summary>
        public int ClientType { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        [TomlValidation(ValidationPattern = @"^0x[0-9A-Fa-f]{6}$", DefaultValue = "0xFFFFFF")]
        public string Color { get; set; } = "0xFFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public string CustomStatusGame { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public bool SystemFatal { get; set; } = false;
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionServiceAppPath
    {
        /// <summary>
        /// 
        /// </summary>
        public string CoeiroinkAppPath { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string LlmAppPath { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string FfmpegPath { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string YtdlpAppPath { get; set; } = "";
    }
    /// <summary>
    /// 
    /// </summary>
    public class SystemLogSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string LogFilePath { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string LogDataWebHook { get; set; } = "";
    }
    /// <summary>
    /// 
    /// </summary>
    public class TokenSetting
    {
        /// <summary>
        /// 
        /// </summary>
        [TomlValidation(Required = true)]
        public string Token { get; set; } = "";
    }

    // 既存のTOMLファイル構造と互換性を保つため、元のプロパティ名を使用
    /// <summary>
    /// 
    /// </summary>
    public class BotConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string? CommandPrefix { get; set; } = "!";

        // 既存のTOMLファイル構造に合わせた名前
        /// <summary>
        /// 
        /// </summary>
        public GeneralSettings? Setting { get; set; } = new();
        /// <summary>
        /// 
        /// </summary>
        public ConnectionServiceAppPath? AppPath { get; set; } = new();
        /// <summary>
        /// 
        /// </summary>
        public SystemLogSettings? SystemLog { get; set; } = new();
        /// <summary>
        /// 
        /// </summary>
        public TokenSetting? TokenData { get; set; } = new();
    }
}
