using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    public interface IGameCodeService
    {
        Task<IEnumerable<GameCodeModel>> GetActivePublicCodesAsync();
        Task<(bool Success, string Message)> RegisterCodeAsync(string url, string urlType, string userId, DateTime ttl, bool publish);
        Task<(bool Success, string Message)> UpdateCodeAsync(string url, DateTime newTtl, bool publish);
    }
}
