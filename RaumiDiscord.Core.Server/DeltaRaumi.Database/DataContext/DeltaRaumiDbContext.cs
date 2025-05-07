using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Infrastructure.Configuration;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext
{
    /// <summary>
    /// DeltaRaumiのデータベースコンテキストを表します。
    /// </summary>
    public class DeltaRaumiDbContext : DbContext
    {
        /// <summary>
        /// コンポーネントモデルのデータセットを取得または設定します。
        /// </summary>
        public DbSet<DiscordComponentModel> Components { get; set; }

        /// <summary>
        /// ユーザーの基本データモデルのデータセットを取得または設定します。
        /// </summary>
        public DbSet<GuildBaseData> GuildBases { get; set; }

        /// <summary>
        /// ユーザーのベースデータモデルのデータセットを取得または設定します。
        /// </summary>
        public DbSet<UserBaseData> UserBases { get; set; }

        /// <summary>
        /// ユーザーのギルドデータモデルのデータセットを取得または設定します。
        /// </summary>
        public DbSet<UserGuildData> UserGuildData { get; set; }

        /// <summary>
        /// URLデータモデルのデータセットを取得または設定します。
        /// </summary>
        public DbSet<UrlDataModel> UrlDataModels { get; set; } = null!;

        private DatabaseType databaseType = DatabaseType.Sqlite;


        /// <summary>
        /// DeltaRaumiDbContextのコンストラクタ
        /// </summary>
        /// <param name="options"></param>
        public DeltaRaumiDbContext(DbContextOptions<DeltaRaumiDbContext> options)
        : base(options)
        {
        }

        /// <summary>
        /// データベースの接続を構成します。
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (databaseType)
            {
                case DatabaseType.MariaDb:
                    optionsBuilder.UseMySql(MySqlConfig.FromConfigFile().GetConnectionString(), new MariaDbServerVersion("11.6.2"));
                    break;

                case DatabaseType.Sqlite:
                    if (!File.Exists("Resources\\Data\\DeltaRaumiData.db"))
                    {
                        Directory.CreateDirectory("Resources\\Data");
                        File.Create("Resources\\Data\\DeltaRaumiData.db");
                    }

                    optionsBuilder.UseSqlite("Data Source=Resources\\Data\\DeltaRaumiData.db;Cache=Shared");
                    
                    break;

                case DatabaseType.InMemory:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unsupported database type");
            }
        }

        /// <summary>
        /// モデルの作成時に呼び出されます。
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscordComponentModel>(entity => { entity.HasKey(e => e.CustomId); });
            //modelBuilder.Entity
            //
            //
        }

        /// <summary>
        /// データベースの種類を定義します。
        /// </summary>
        public enum DatabaseType 
        {
            /// <summary>
            /// MySQL/MariaDB
            /// </summary>
            MariaDb,
            /// <summary>
            /// SQLite
            /// </summary>
            Sqlite,
            /// <summary>
            /// InMemory
            /// </summary>
            InMemory,
            /// <summary>
            /// PostgreSQL
            /// </summary>
            PostgreSql

        }
    }
}
