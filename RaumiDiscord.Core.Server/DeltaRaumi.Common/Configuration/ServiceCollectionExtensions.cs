using Microsoft.Extensions.Primitives;
using RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models;


namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static IServiceCollection AddDeltaRaumiConfiguration(
            this IServiceCollection services,
            string configFilePath)
        {
            services.AddSingleton(provider =>
            {
                var configManager = new ConfigurationManager(configFilePath);
                var config = configManager.GetConfiguration();

                // Discord設定の自動更新
                configManager.UpdateDiscordSettings();
                configManager.SaveConfiguration();

                return config;
            });

            services.AddSingleton<ConfigurationManager>(provider =>
                new ConfigurationManager(configFilePath));

            services.AddSingleton<IConfiguration>(provider =>
            {
                var botConfig = provider.GetRequiredService<BotConfiguration>();
                return new BotConfigurationAdapter(botConfig);
            });

            return services;
        }
    }

    /// <summary>
    /// BotConfigurationをIConfigurationインターフェースにアダプトするクラス
    /// </summary>
    public class BotConfigurationAdapter : IConfiguration
    {
        private readonly BotConfiguration _config;
        private readonly Dictionary<string, string?> _flatConfig;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BotConfigurationAdapter(BotConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _flatConfig = FlattenConfiguration();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? this[string key]
        {
            get => _flatConfig.TryGetValue(key, out var value) ? value : null;
            set => _flatConfig[key] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            var sections = new List<IConfigurationSection>();
            var sectionKeys = new HashSet<string>();

            foreach (var kvp in _flatConfig)
            {
                var firstColonIndex = kvp.Key.IndexOf(':');
                if (firstColonIndex > 0)
                {
                    var sectionKey = kvp.Key.Substring(0, firstColonIndex);
                    if (sectionKeys.Add(sectionKey))
                    {
                        sections.Add(new ConfigurationSection(this, sectionKey));
                    }
                }
            }

            return sections;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IChangeToken GetReloadToken()
        {
            // 簡単な実装：常に変更なしとして扱う
            return new NoChangeToken();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IConfigurationSection GetSection(string key)
        {
            return new ConfigurationSection(this, key);
        }

        private Dictionary<string, string?> FlattenConfiguration()
        {
            var result = new Dictionary<string, string?>();

            // Root level properties
            result["CommandPrefix"] = _config.CommandPrefix;

            // Setting section (元の構造に合わせて)
            if (_config.Setting != null)
            {
                result["Setting:CommandPrefix"] = _config.Setting.CommandPrefix;
                result["Setting:UpTime"] = _config.Setting.UpTime.ToString("O");
                result["Setting:UpdateTime"] = _config.Setting.UpdateTime.ToString("O");
                result["Setting:TimeZone"] = _config.Setting.TimeZone;
                result["Setting:DiscordApiVersion"] = _config.Setting.DiscordApiVersion;
                result["Setting:CdnUrl"] = _config.Setting.CdnUrl;
                result["Setting:DefaultRequestTimeout"] = _config.Setting.DefaultRequestTimeout.ToString();
                result["Setting:InviteUrl"] = _config.Setting.InviteUrl;
                result["Setting:MaxApplicationDescriptionLength"] = _config.Setting.MaxApplicationDescriptionLength.ToString();
                result["Setting:MaxMessageSize"] = _config.Setting.MaxMessageSize.ToString();
                result["Setting:MaxPollAnswerTextLength"] = _config.Setting.MaxPollAnswerTextLength.ToString();
                result["Setting:MaxPollQuestionTextLength"] = _config.Setting.MaxPollQuestionTextLength.ToString();
                result["Setting:MaxVoiceChannelStatusLength"] = _config.Setting.MaxVoiceChannelStatusLength.ToString();
                result["Setting:ClientType"] = _config.Setting.ClientType.ToString();
                result["Setting:Color"] = _config.Setting.Color;
                result["Setting:CustomStatusGame"] = _config.Setting.CustomStatusGame;
                result["Setting:SystemFatal"] = _config.Setting.SystemFatal.ToString();
            }

            // AppPath section
            if (_config.AppPath != null)
            {
                result["AppPath:CoeiroinkAppPath"] = _config.AppPath.CoeiroinkAppPath;
                result["AppPath:LlmAppPath"] = _config.AppPath.LlmAppPath;
                result["AppPath:FfmpegPath"] = _config.AppPath.FfmpegPath;
                result["AppPath:YtdlpAppPath"] = _config.AppPath.YtdlpAppPath;
            }

            // SystemLog section
            if (_config.SystemLog != null)
            {
                result["SystemLog:LogFilePath"] = _config.SystemLog.LogFilePath;
                result["SystemLog:LogDataWebHook"] = _config.SystemLog.LogDataWebHook;
            }

            // TokenData section
            if (_config.TokenData != null)
            {
                result["TokenData:Token"] = _config.TokenData.Token;
            }

            return result;
        }

        private class ConfigurationSection : IConfigurationSection
        {
            private readonly IConfiguration _root;
            private readonly string _path;

            public ConfigurationSection(IConfiguration root, string path)
            {
                _root = root;
                _path = path;
            }

            public string? this[string key]
            {
                get => _root[ConfigurationPath.Combine(_path, key)];
                set => _root[ConfigurationPath.Combine(_path, key)] = value;
            }

            public string Key => _path.Split(':').Last();
            public string Path => _path;
            public string? Value
            {
                get => _root[_path];
                set => _root[_path] = value;
            }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                return _root.GetChildren().Where(section => section.Path.StartsWith(_path + ":"));
            }

            public IChangeToken GetReloadToken() => _root.GetReloadToken();
            public IConfigurationSection GetSection(string key) => _root.GetSection(ConfigurationPath.Combine(_path, key));
        }

        private class NoChangeToken : IChangeToken
        {
            public bool HasChanged => false;
            public bool ActiveChangeCallbacks => false;
            public IDisposable RegisterChangeCallback(Action<object?> callback, object? state) => new NoOpDisposable();
        }

        private class NoOpDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationPath
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public static string Combine(params string[] pathSegments)
        {
            return string.Join(":", pathSegments.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
