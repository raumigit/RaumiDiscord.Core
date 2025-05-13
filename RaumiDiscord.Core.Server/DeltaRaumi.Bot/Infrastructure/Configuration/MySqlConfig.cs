#nullable disable

using Newtonsoft.Json;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;
using System.Text;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration
{
    /// <summary>
    /// MySQLの設定を管理するクラス
    /// </summary>
    public class MySqlConfig
    {
        //Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
        /// <summary>
        /// MySQLの接続情報
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// MySQLのポート番号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// MySQLのデータベース名
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// MySQLのユーザー名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// MySQLのパスワード
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Override the "database=" portion of the connection string configured in the file
        /// </summary>
        private string DatabaseNameOverride { get; set; }

        /// <summary>
        /// MySQLの設定を管理するクラス
        /// </summary>
        [JsonConstructor]
        public MySqlConfig()
        {
            //コードがない
        }
        /// <summary>
        /// Override the "database=" portion of the connection string configured in the file
        /// </summary>
        /// <param name="database">Database name</param>
        public MySqlConfig(string database)
        {
            DatabaseNameOverride = database;
        }
        /// <summary>
        /// MySQLの接続文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            StringBuilder builder = new StringBuilder("Server=");
            builder.Append(Address);
            builder.Append("; Port=");
            builder.Append(Port);
            builder.Append("; Database=");
            if (string.IsNullOrEmpty(DatabaseNameOverride))
                builder.Append(Database);
            else builder.Append(DatabaseNameOverride);
            builder.Append("; Uid=");
            builder.Append(Username);
            builder.Append("; Pwd=");
            builder.Append(Password);
            builder.Append(';');
            return builder.ToString();
        }

        /// <summary>
        /// MySQLの設定をJSON文字列から取得する
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static MySqlConfig FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<MySqlConfig>(json);
        }
        /// <summary>
        /// MySQLの設定をファイルから取得する
        /// </summary>
        /// <returns></returns>
        public static MySqlConfig FromConfigFile()
        {
            if (!Directory.Exists(Directories.AppData)) Directory.CreateDirectory(Directories.AppData);
            if (!File.Exists(Directories.MySqlConfigPath))
            {
                MySqlConfig config = new MySqlConfig();
                config.Address = "Address";
                config.Port = "Port";
                config.Database = "Database";
                config.Username = "Username";
                config.Password = "Password";
                File.WriteAllText(Directories.MySqlConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                return config;
            }
            else return JsonConvert.DeserializeObject<MySqlConfig>(File.ReadAllText(Directories.MySqlConfigPath));
        }
    }
}