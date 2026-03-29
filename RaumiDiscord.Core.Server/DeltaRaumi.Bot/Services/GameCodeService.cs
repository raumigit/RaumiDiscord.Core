using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    public class GameCodeService: IGameCodeService
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private readonly GameMetaService _gameMetaService;

        public GameCodeService(DeltaRaumiDbContext deltaRaumiDb, GameMetaService gameMetaService)
        {
            _deltaRaumiDb = deltaRaumiDb;
            _gameMetaService = gameMetaService;
        }
        public async Task<IEnumerable<GameCodeModel>> GetActivePublicCodesAsync()
        {
            return await _deltaRaumiDb.GameCodeModels
                .Where(u => u.Publish && u.Ttl > DateTime.UtcNow)
                .OrderBy(u => u.Ttl)
                .ToListAsync();
        }
        public async Task<(bool Success, string Message)> RegisterCodeAsync(string url, string urlType, string userId, DateTime ttl, bool publish)
        {
            var gameConfig = _gameMetaService.FindGame(urlType);
            if (gameConfig == null) return (false, "指定されたゲームタイプが存在しません。");

            // URLの整形ロジックをここに集約 (GameCodeModuleから移植)
            if (!url.StartsWith("http") && !string.IsNullOrEmpty(gameConfig.BaseUrl))
            {
                url = gameConfig.BaseUrl + url;
            }

            if (await _deltaRaumiDb.GameCodeModels.AnyAsync(u => u.Url == url))
                return (false, "このコードは既に登録されています。");

            var newEntry = new GameCodeModel
            {
                Url = url,
                ContentType = gameConfig.Name,
                DiscordUser = userId,
                Ttl = ttl,
                Publish = publish
            };

            _deltaRaumiDb.GameCodeModels.Add(newEntry);
            await _deltaRaumiDb.SaveChangesAsync();
            return (true, "登録に成功しました。");
        }
        public async Task<(bool Success, string Message)> UpdateCodeAsync(string url, DateTime newTtl, bool publish)
        {
            var entry = await _deltaRaumiDb.GameCodeModels.FirstOrDefaultAsync(u => u.Url == url);
            if (entry == null) return (false, "指定されたコードが見つかりません。");
            entry.Ttl = newTtl;
            entry.Publish = publish;
            await _deltaRaumiDb.SaveChangesAsync();
            return (true, "更新に成功しました。");
        }
        public async Task<(bool Success, string Message)> DeleteCodeAsync(string url)
        {
            var entry = await _deltaRaumiDb.GameCodeModels.FirstOrDefaultAsync(u => u.Url == url);
            if (entry == null) return (false, "指定されたコードが見つかりません。");
            _deltaRaumiDb.GameCodeModels.Remove(entry);
            await _deltaRaumiDb.SaveChangesAsync();
            return (true, "削除に成功しました。");
        }
    }
}
