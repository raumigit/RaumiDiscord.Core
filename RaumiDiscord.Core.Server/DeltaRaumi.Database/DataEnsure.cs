using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database
{
    /// <summary>
    /// データベース操作に関するヘルパークラスです。
    /// </summary>
    public class DataEnsure
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private readonly DiscordSocketClient _client;
        private readonly ImprovedLoggingService _logger;
        private readonly SocketGuild _guild;
        private string? _logChannelId;

        /// <summary>
        /// DatabaseHelperのコンストラクター
        /// </summary>
        /// <param name="deltaRaumiDb">データベースコンテキスト</param>
        /// <param name="logging">ロギングサービス</param>
        /// <param name="client"></param>
        /// <param name="guild"></param>
        /// <param name="logChannelId"></param>
        public DataEnsure(DeltaRaumiDbContext deltaRaumiDb, ImprovedLoggingService logging, DiscordSocketClient client, SocketGuild guild, string? logChannelId)
        {
            _deltaRaumiDb = deltaRaumiDb;
            _logger = logging;
            _client = client;
            _guild = guild;
            _logChannelId = logChannelId;
        }


        //private void CompletionUrlDataModelAsync()
        //{
        //}
        /// <summary>
        /// GuildBaseDataModelを確認し、存在しない場合は新規作成します。
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public async Task<GuildBaseDataModel> EnsureGuildBaseDataExistsAsync(SocketGuild guild)
        {
            var guildId = guild.Id.ToString();

            // GuildBaseDataを検索
            var guildData = await _deltaRaumiDb.GuildBases
                .Where(g => g.GuildId == guildId)
                .FirstOrDefaultAsync();

            // 存在しない場合は作成
            if (guildData == null)
            {
                guildData = await CompletionGuildBaseModelAsync(guild);
            }
            return guildData;
        }
        /// <summary>
        /// EnsureUserBaseDataExistsAsyncを確認し、存在しない場合は新規作成します。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserBaseDataModel> EnsureUserBaseDataExistsAsync(SocketGuildUser user)
        {
            var userId = user.Id.ToString();

            // UserBaseDataを検索
            var userData = await _deltaRaumiDb.UserBases
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            // 存在しない場合は作成
            if (userData == null)
            {
                userData = await CompletionUserBaseModelAsync(user);
            }
            return userData;
        }
        /*
                private void CompletionUserGuildModel()
                {

                }
        */
        //private Task Completion()
        //{

        //}
        /// <summary>
        /// GetOrCreateUserGuildDataAsyncを確認し、存在しない場合は新規作成します。
        /// </summary>
        /// <param name="guildData"></param>
        /// <param name="userData"></param>
        /// <param name="guild"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserGuildDataModel> GetOrCreateUserGuildDataAsync(
            GuildBaseDataModel guildData,
            UserBaseDataModel userData,
            SocketGuild guild,
            SocketGuildUser user)
        {
            var guildId = guild.Id.ToString();
            var userId = user.Id.ToString();

            // UserGuildDataを検索
            var userGuildData = await _deltaRaumiDb.UserGuildData
                .Where(ug => ug.GuildId == guildId && ug.UserId == userId)
                .FirstOrDefaultAsync();

            // 存在しない場合は作成
            if (userGuildData == null)
            {
                userGuildData = new UserGuildDataModel
                {
                    GuildBaseData = guildData,
                    UserBaseData = userData,
                    GuildId = guildId,
                    UserId = userId,
                    GuildAvatarId = user.GetGuildAvatarUrl() ?? user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(),
                    JoinedAt = user.JoinedAt?.UtcDateTime ?? DateTime.UtcNow,
                    TimedOutUntil = user.TimedOutUntil?.UtcDateTime ?? DateTime.MinValue,
                    GuildExp = 0,
                    LatestExp = DateTime.MinValue,
                    TotalMessage = 0
                };
                _deltaRaumiDb.UserGuildData.Add(userGuildData);
                await _deltaRaumiDb.SaveChangesAsync();

                await _logger.Log($"新しいUserGuildDataを作成しました: Guild: {guild.Name}, User: {user.Username}", "DataEnsure");
            }
            return userGuildData;
        }

        /// <summary>
        /// GuildBaseDataを初期化します
        /// </summary>
        private async Task<GuildBaseDataModel> CompletionGuildBaseModelAsync(SocketGuild guild)
        {
            var guildData = new GuildBaseDataModel
            {
                GuildId = guild.Id.ToString(),
                GuildName = guild.Name,
                IconUrl = guild.IconUrl,
                BannerUrl = guild.BannerUrl,
                OwnerId = guild.OwnerId.ToString(),
                WelcomeChannnelId = null,
                CreatedAt = guild.CreatedAt.UtcDateTime,
                Description = guild.Description,
                MaxUploadLimit = guild.MaxUploadLimit,
                MemberCount = guild.MemberCount,
                PremiumSubscriptionCount = guild.PremiumSubscriptionCount,
                PremiumTier = guild.PremiumTier,
                LogChannel = null
            };

            _deltaRaumiDb.GuildBases.Add(guildData);
            await _deltaRaumiDb.SaveChangesAsync();

            await _logger.Log($"新しいGuildBaseDataを作成しました: {guild.Name}", "DataEnsure");

            return guildData;
        }

        /// <summary>
        /// UserBaseDataを初期化します
        /// </summary>
        private async Task<UserBaseDataModel> CompletionUserBaseModelAsync(SocketGuildUser user)
        {
            var userData = new UserBaseDataModel()
            {
                UserId = user.Id.ToString(),
                UserName = user.Username,
                AvatarId = user.AvatarId,
                CreatedAt = user.CreatedAt.UtcDateTime,
                IsBot = user.IsBot,
                IsWebhook = user.IsWebhook,
                Exp = 0,
                Barthday = null, // 初期値はnull、後で設定可
                SetToMention = 0, // 初期値は-1、後で設定可
                Userstatus = 0
            };

            _deltaRaumiDb.UserBases.Add(userData);
            await _deltaRaumiDb.SaveChangesAsync();

            await _logger.Log($"新しいUserBaseDataを作成しました: {user.Username}", "DataEnsure");

            return userData;
        }
    }
}
