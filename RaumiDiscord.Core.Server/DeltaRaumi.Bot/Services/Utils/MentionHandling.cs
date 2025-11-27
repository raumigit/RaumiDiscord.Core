using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using NUlid;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using System.Linq;
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
        public MentionHandling(DeltaRaumiDbContext dbContext, ImprovedLoggingService loggingService, DiscordSocketClient Client)
        {
            _dbContext = dbContext;
            _logger = loggingService;
            _client = Client;
            // Constructor logic if needed
        }

        internal async Task PingMentions(SocketMessage message)
        {
            if (message is not SocketUserMessage userMessage) return;

            // 今回のメッセージでメンションされたユーザーがいない場合は処理終了
            if (message.MentionedUsers == null || !message.MentionedUsers.Any()) return;

            List<string> mentionedUserIds;
            string? mentionedUsersJson = null;

            mentionedUserIds = message.MentionedUsers
                .Select(u => u.Id.ToString())
                .ToList();
            mentionedUsersJson = JsonSerializer.Serialize(mentionedUserIds);

            var createdAt = userMessage.Timestamp.UtcDateTime;
            var authorId = userMessage.Author.Id.ToString();
            var guildId = (message.Channel as SocketGuildChannel)?.Guild.Id.ToString() ?? "DM";

            // UserGuildStats保存処理
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

            // 直近24時間の記録を取得（今回メンションされたユーザーに関連するもののみ）
            var timeThreshold = DateTime.UtcNow.AddHours(-24);
            var recentStats = await _dbContext.UserGuildStats
                .Where(s => s.CreatedAt >= timeThreshold && s.GuildId == guildId)
                .ToListAsync();

            // 今回メンションされたユーザーごとのメンション回数をカウント
            var mentionCounts = new Dictionary<string, int>();

            // 今回メンションされたユーザーIDで初期化
            foreach (var userId in mentionedUserIds)
            {
                mentionCounts[userId] = 0;
            }

            // 過去24時間の統計から今回メンションされたユーザーのカウントを集計
            foreach (var item in recentStats)
            {
                var mentionedUsers = DeserializeMentionedUsers(item);
                if (mentionedUsers != null)
                {
                    foreach (var mentionedUserId in mentionedUsers)
                    {
                        // 今回メンションされたユーザーのみカウント
                        if (mentionCounts.ContainsKey(mentionedUserId))
                        {
                            mentionCounts[mentionedUserId]++;
                        }
                    }
                }
            }

            // オーバーしたユーザーをチェック
            var overageUsers = new List<(string UserId, string UserName, int MentionCount, int SetToMention)>();

            foreach (var kvp in mentionCounts)
            {
                var userId = kvp.Key;
                var mentionCount = kvp.Value;

                // UserGuildDataからSetToMentionを取得（ギルドIDも考慮）
                var userGuildData = await _dbContext.UserGuildData
                    .FirstOrDefaultAsync(ugd => ugd.UserId == userId && ugd.GuildId == guildId);

                if (userGuildData != null && userGuildData.SetToMention != -1 && mentionCount > userGuildData.SetToMention)
                {
                    // ユーザー名を取得
                    string? dbUserName = await _dbContext.UserBases
                        .Where(u => u.UserId == userId)
                        .Select(u => u.UserName)
                        .FirstOrDefaultAsync();

                    dbUserName ??= $"不明なユーザー (ID:{userId})";

                    overageUsers.Add((userId, dbUserName, mentionCount, userGuildData.SetToMention));
                }
            }

            await _logger.Log($"オーバーユーザー数: {overageUsers.Count}", "LevelService: MentionHandling");

            // オーバーしたユーザーがいる場合、DMで通知
            if (overageUsers.Count > 0)
            {
                await SendOverageNotificationAsync(message.Author, overageUsers);
            }
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
                await _logger.Log($"ユーザー {author.Username} にDMを送信できませんでした。", "LevelService: MentionHandling");
            }
            catch (Exception ex)
            {
                await _logger.Log($"DM送信中にエラーが発生しました: {ex.Message}", "LevelService: MentionHandling");
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