using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using NUlid;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using System.Text;
using System.Text.Json;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils
{
    /// <summary>
    /// MentionHandlingは、メンションの処理を行うサービスです。
    /// </summary>
    public class MentionHandling
    {
        private static ImprovedLoggingService _logger;
        private static DeltaRaumiDbContext _dbContext;
        private static DiscordSocketClient _client;
        /// <summary>
        /// MentionHandlingのコンストラクタ
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="loggingService"></param>
        public MentionHandling(DeltaRaumiDbContext dbContext, ImprovedLoggingService loggingService ,DiscordSocketClient Client)
        {
            _dbContext = dbContext;
            _logger = loggingService;
            _client = Client;
            // Constructor logic if needed
        }

        internal async Task PingMentions(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage) return;

            
            //if (mentionedUserIds.Count == 0) return;

            List<string> mentionedUserIds;

            string? mentionedUsersJson = null;
            if (message.MentionedUsers != null && message.MentionedUsers.Any())
            {
                mentionedUserIds = message.MentionedUsers
                    .Select(u => u.Id.ToString())
                    .ToList();
                mentionedUsersJson = JsonSerializer.Serialize(mentionedUserIds);
            }

            var createdAt = userMessage.Timestamp.UtcDateTime;
            var authorId = userMessage.Author.Id.ToString();
            var guildId = (message.Channel as SocketGuildChannel)?.Guild.Id.ToString() ?? "DM";

            // 保存処理
            var userGuildStats = new UserGuildStatsModel
            {
                StatUlid = Ulid.NewUlid(),
                GuildId = guildId,
                UserId = message.Author.Id.ToString(),
                CreatedAt = message.Timestamp.UtcDateTime,
                MentionedUserId = mentionedUsersJson
            };

            _dbContext.UserGuildStats.Add(userGuildStats);
            await _dbContext.SaveChangesAsync();

            // 直近24時間の記録を取得
            var timeThreshold = DateTime.UtcNow.AddHours(-24);
            var recentStats = await _dbContext.UserGuildStats
                .Where(s => s.CreatedAt >= timeThreshold)
                .ToListAsync();

            // ユーザーごとのメンション回数をカウント(古い)
            //var allMentioned = recentStats
            //    .SelectMany(s => (s.MentionedUserId)
            //    .Split(',', StringSplitOptions.RemoveEmptyEntries))
            //    .GroupBy(id => id)
            //    .ToDictionary(g => g.Key, g => g.Count());

            // 該当ユーザーを抽出
            var mentionCounts = new Dictionary<string, int>();// ユーザーIDごとのメンション回数

            foreach (var item in recentStats)
            {
                var mentionedUsers = DeserializeMentionedUsers(item);// Json文字列をリストに変換
                if (mentionedUsers != null)
                {
                    foreach (var mentionedUserId in mentionedUsers)
                    {
                        if (mentionCounts.ContainsKey(mentionedUserId))
                            mentionCounts[mentionedUserId]++;
                        else
                            mentionCounts[mentionedUserId] = 0;
                    }
                }
            }

            // オーバーしたユーザーをチェック
            var overageUsers = new List<(string UserId, string UserName, int MentionCount, int SetToMention)>();

            foreach (var kvp in mentionCounts)
            {
                var userId = kvp.Key;
                var mentionCount = kvp.Value;

                // UserBasesからSetToMentionを取得
                var userBase = await _dbContext.UserBases
                    .FirstOrDefaultAsync(ub => ub.UserId == userId);
                
                if (userBase != null && mentionCount > userBase.SetToMention)
                {
                    // ユーザー名を取得
                    var user = _client.GetUser(ulong.Parse(userId));
                    var userName = user?.Username ?? $"UserId:{userId}";

                    overageUsers.Add((userId, userName, mentionCount, userBase.SetToMention));
                }
            }

            // オーバーしたユーザーがいる場合、DMで通知
            if (overageUsers.Any())
            {
                await SendOverageNotificationAsync(message.Author, overageUsers);
            }



            // DM送信
            //if (true)
            //{
            //    try
            //    {
            //        var dm = await message.Author.CreateDMChannelAsync();
            //        var msg = string.Join("\n", overThresholdUserIds.Select(id =>
            //            $"⚠️ ユーザーID `{id}` は直近24時間で {allMentioned[id]} 回メンションされました（許容値: {(_dbContext.UserBases.FirstOrDefault(u => u.UserId == id)?.SetToMention ?? -1)}）"));

            //        await dm.SendMessageAsync(msg);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"DM送信失敗: {ex.Message}");
            //    }
            //}
        }

        private async Task SendOverageNotificationAsync(SocketUser author, List<(string UserId, string UserName, int MentionCount, int SetToMention)> overageUsers)
        {
            try
            {
                // DMチャンネルを開く
                var dmChannel = await author.CreateDMChannelAsync();

                // メッセージを構築
                var messageBuilder = new StringBuilder();

                foreach (var (userId, userName, mentionCount, setToMention) in overageUsers)
                {
                    messageBuilder.AppendLine(
                        $"ユーザー{userName}は直近24時間で{mentionCount}回メンションされました。(許容値：{setToMention})");
                }

                // DMを送信
                await dmChannel.SendMessageAsync(messageBuilder.ToString().TrimEnd());
            }
            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
            {
                // DMが送信できない場合（ユーザーがDMをブロックしている等）
                Console.WriteLine($"ユーザー {author.Username} にDMを送信できませんでした。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DM送信中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// 特定のレコードからメンションされたユーザーIDリストを取得
        /// </summary>
        public List<string>? DeserializeMentionedUsers(UserGuildStatsModel stats)
        {
            if (string.IsNullOrEmpty(stats.MentionedUserId))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<string>>(stats.MentionedUserId);
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
