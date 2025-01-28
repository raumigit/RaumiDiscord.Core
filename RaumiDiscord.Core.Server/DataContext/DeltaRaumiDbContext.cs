using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Api.Models;

namespace RaumiDiscord.Core.Server.DataContext
{
    public class DeltaRaumiDbContext : DbContext
    {
        public DbSet<DiscordComponentModel> Components { get; set; }
        public DbSet<GuildBaseData> GuildBases { get; set; }
        public DbSet<UserBaseData> UserBases { get; set; }
        public DbSet<UserGuildData> UserGuildData { get; set; }
        public DbSet<UrlDetaModel> urlDetaModels { get; set; } = null!;

        private DatabaseType databaseType = DatabaseType.Sqlite;

        public DeltaRaumiDbContext(DbContextOptions<DeltaRaumiDbContext> options)
        : base(options)
        {
        }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (this.databaseType)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscordComponentModel>(entity => { entity.HasKey(e => e.CustomId); });
            //modelBuilder.Entity
            //
            //
        }

        public enum DatabaseType { MariaDb, Sqlite, InMemory };
    }
}
