using System;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common.Configuration
{
    /// <summary>
    /// TOML設定の検証ルールを指定するAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TomlValidationAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object? DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ValidationPattern { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ValidationMessage { get; set; }
    }

    /// <summary>
    /// TOMLプロパティ名を指定するAttribute（snake_case変換を無効にする場合など）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TomlKeyAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string KeyName { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        public TomlKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }
}
