using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tomlyn;
using Tomlyn.Model;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Configuration
{
    /// <summary>
    /// Manages the bot's configuration, including loading, saving, and validating configuration data.
    /// </summary>
    /// <remarks>This class provides functionality to handle configuration files in TOML format. It allows
    /// loading configuration data from a file, saving changes back to the file, and validating the configuration
    /// against defined rules. The class also supports initializing default configuration values and managing metadata
    /// associated with the configuration. <para> The configuration file path is specified during construction, and the
    /// class ensures that the configuration is loaded or initialized as needed. Any changes to the configuration can be
    /// persisted using the <see cref="SaveConfiguration"/> method. </para> <para> Exceptions are thrown for invalid
    /// operations, such as attempting to load a non-existent or malformed configuration file, or when validation rules
    /// are violated. </para></remarks>
    public class ConfigurationManager
    {
        private readonly string _configFilePath;
        private BotConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager"/> class with the specified configuration
        /// file path.
        /// </summary>
        /// <param name="configFilePath">The path to the configuration file. This cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configFilePath"/> is <see langword="null"/>.</exception>
        public ConfigurationManager(string configFilePath)
        {
            _configFilePath = configFilePath ?? throw new ArgumentNullException(nameof(configFilePath));
            _config = new BotConfiguration();
        }

        /// <summary>
        /// Retrieves the current bot configuration, initializing a default configuration if none exists.
        /// </summary>
        /// <remarks>If the configuration file does not exist at the specified path, a default
        /// configuration is created and saved.  The method then loads the configuration from the file and returns
        /// it.</remarks>
        /// <returns>The current <see cref="BotConfiguration"/> instance representing the bot's settings.</returns>
        public BotConfiguration GetConfiguration()
        {
            if (!File.Exists(_configFilePath))
            {
                InitializeDefaultConfiguration();
            }
            LoadConfiguration();
            return _config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void LoadConfiguration()
        {
            try
            {
                var tomlText = File.ReadAllText(_configFilePath);
                _config = Toml.ToModel<BotConfiguration>(tomlText);

                // nullチェックと初期化
                _config.Setting ??= new GeneralSettings();
                _config.AppPath ??= new ConnectionServiceAppPath();
                _config.SystemLog ??= new SystemLogSettings();
                _config.TokenData ??= new TokenSetting();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load configuration from {_configFilePath}", ex);
            }
        }

        /// <summary>
        /// Saves the current configuration to a file in TOML format.
        /// </summary>
        /// <remarks>This method converts the current configuration object to a TOML representation and
        /// writes it to the file specified by the configuration file path. If the target directory does not exist, it
        /// will be created.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the configuration cannot be saved due to an error, such as an I/O issue or invalid file path. The
        /// inner exception provides additional details about the failure.</exception>
        public void SaveConfiguration()
        {
            try
            {
                // 現在時刻の更新
                if (_config.Setting != null)
                {
                    _config.Setting.UpdateTime = DateTime.Now;
                }

                var tomlText = Toml.FromModel(_config);

                // ディレクトリが存在しない場合は作成
                var directory = Path.GetDirectoryName(_configFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_configFilePath, tomlText);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save configuration to {_configFilePath}", ex);
            }
        }

        /// <summary>
        /// Initializes the default configuration for the bot.
        /// </summary>
        /// <remarks>This method resets the bot's configuration to its default state and persists the
        /// changes. It should be called to ensure the bot starts with a clean configuration.</remarks>
        public void InitializeDefaultConfiguration()
        {
            var timeZone = TimeZoneInfo.Local;
            _config = new BotConfiguration
            {
                CommandPrefix = "!",
                Setting = new GeneralSettings
                {
                    CommandPrefix = "!",
                    UpdateTime = DateTime.Now,
                    UpTime = DateTime.Now,
                    TimeZone = timeZone.DisplayName,
                    DiscordApiVersion = DiscordConfig.APIVersion.ToString(),
                    CdnUrl = DiscordConfig.CDNUrl,
                    DefaultRequestTimeout = DiscordConfig.DefaultRequestTimeout,
                    InviteUrl = DiscordConfig.InviteUrl,
                    MaxApplicationDescriptionLength = DiscordConfig.MaxApplicationDescriptionLength,
                    MaxMessageSize = DiscordConfig.MaxMessageSize,
                    MaxPollAnswerTextLength = DiscordConfig.MaxPollAnswerTextLength,
                    MaxPollQuestionTextLength = DiscordConfig.MaxPollQuestionTextLength,
                    MaxVoiceChannelStatusLength = DiscordConfig.MaxVoiceChannelStatusLength,
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

            SaveConfiguration();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ValidateConfiguration()
        {
            try
            {
                return ValidateObject(_config, nameof(_config));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private bool ValidateObject(object obj, string path)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                var validation = property.GetCustomAttribute<TomlValidationAttribute>();
                if (validation == null) continue;

                var value = property.GetValue(obj);
                var propertyPath = $"{path}.{property.Name}";

                if (validation.Required && (value == null || (value is string str && string.IsNullOrWhiteSpace(str))))
                {
                    throw new InvalidOperationException($"Required configuration property '{propertyPath}' is missing or empty.");
                }

                if (!string.IsNullOrEmpty(validation.ValidationPattern) && value is string stringValue)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(stringValue, validation.ValidationPattern))
                    {
                        var message = validation.ValidationMessage ?? $"Property '{propertyPath}' does not match required pattern.";
                        throw new InvalidOperationException(message);
                    }
                }
            }

            return true;
        }

        // ITomlMetadataProvider implementation
        /// <summary>
        /// 
        /// </summary>
        public void UpdateDiscordSettings()
        {
            if (_config.Setting != null)
            {
                _config.Setting.DiscordApiVersion = DiscordConfig.APIVersion.ToString();
                _config.Setting.CdnUrl = DiscordConfig.CDNUrl;
                _config.Setting.DefaultRequestTimeout = DiscordConfig.DefaultRequestTimeout;
                _config.Setting.InviteUrl = DiscordConfig.InviteUrl;
                _config.Setting.MaxApplicationDescriptionLength = DiscordConfig.MaxApplicationDescriptionLength;
                _config.Setting.MaxMessageSize = DiscordConfig.MaxMessageSize;
                _config.Setting.MaxPollAnswerTextLength = DiscordConfig.MaxPollAnswerTextLength;
                _config.Setting.MaxPollQuestionTextLength = DiscordConfig.MaxPollQuestionTextLength;
                _config.Setting.MaxVoiceChannelStatusLength = DiscordConfig.MaxVoiceChannelStatusLength;
                _config.Setting.UpdateTime = DateTime.Now;
                _config.Setting.TimeZone = TimeZoneInfo.Local.DisplayName;
            }
        }
    }
}
