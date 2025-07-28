using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using NUlid;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils
{
    public class MentionHandling
    {
        private static ImprovedLoggingService _logger;
        private static DeltaRaumiDbContext _dbContext;

        public MentionHandling(DeltaRaumiDbContext dbContext, ImprovedLoggingService loggingService)
        {
            _dbContext = dbContext;
            _logger = loggingService;
            // Constructor logic if needed
        }

        internal async Task pingMentions(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage) return;

            var mentionedUsers = userMessage.MentionedUsers;
            if (mentionedUsers.Count == 0) return;

            var mentionedUserIds = mentionedUsers.Select(u => u.Id.ToString()).ToList();
            var mentionedUserCsv = string.Join(",", mentionedUserIds);
            var createdAt = userMessage.Timestamp.UtcDateTime;
            var authorId = userMessage.Author.Id.ToString();
            var guildId = (message.Channel as SocketGuildChannel)?.Guild.Id.ToString() ?? "DM";

            // 保存処理
            var newStat = new UserGuildStatsModel
            {
                StatUlid = Ulid.NewUlid(),
                GuildId = guildId,
                UserId = authorId,
                CreatedAt = createdAt,
                MentionedUserId = mentionedUserCsv
            };

            _dbContext.UserGuildStats.Add(newStat);
            await _dbContext.SaveChangesAsync();

            // 直近24時間の記録を取得
            var timeThreshold = DateTime.UtcNow.AddHours(-24);
            var recentStats = await _dbContext.UserGuildStats
                .Where(s => s.CreatedAt >= timeThreshold)
                .ToListAsync();

            // ユーザーごとのメンション回数をカウント
            var allMentioned = recentStats
                .SelectMany(s => (s.MentionedUserId ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(id => id)
                .ToDictionary(g => g.Key, g => g.Count());

            // 該当ユーザーを抽出
            var overThresholdUserIds = new List<string>();

            foreach (var mentionedId in mentionedUserIds.Distinct())
            {
                var count = allMentioned.GetValueOrDefault(mentionedId, 0);

                var userSetting = await _dbContext.UserBases
                    .FirstOrDefaultAsync(u => u.UserId == mentionedId);

                var threshold = userSetting?.SetToMention ?? 0;

                if (threshold != 0 && count >= threshold)
                {
                    overThresholdUserIds.Add(mentionedId);
                }
            }

            // DM送信
            if (overThresholdUserIds.Any())
            {
                try
                {
                    var dm = await message.Author.CreateDMChannelAsync();
                    var msg = string.Join("\n", overThresholdUserIds.Select(id =>
                        $"⚠️ ユーザーID `{id}` は直近24時間で {allMentioned[id]} 回メンションされました（許容値: {(_dbContext.UserBases.FirstOrDefault(u => u.UserId == id)?.SetToMention ?? -1)}）"));

                    await dm.SendMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DM送信失敗: {ex.Message}");
                }
            }
        }
    }
}
