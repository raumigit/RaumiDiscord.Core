using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.Models;

namespace RaumiDiscord.Core.Server.DataContext
{
    class DeltaRaumiDbContext : DbContext
    {
        public DbSet<DiscordComponentModel> Components { get; set; }
        private DatabaseType databaseType;

        public DeltaRaumiDbContext(DatabaseType type = DatabaseType.Sqlite)
        {
            this.databaseType = type;
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
                    break;
                default:
                    throw new ArgumentException("Unsupported database type");
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscordComponentModel>(entity => { entity.HasKey(e => e.CustomId); });
        }

        public enum DatabaseType { MariaDb, Sqlite, InMemory };
    }
}
