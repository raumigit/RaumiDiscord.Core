using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// LevelServiceは、ユーザーのレベルを管理するためのサービスです。
    /// </summary>
    public class LevelService
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private readonly DiscordSocketClient _client;
        private readonly DataEnsure _dataEnsure;
        private readonly Random _random;



        /// <summary>
        /// LevelServiceのコンストラクター
        /// </summary>
        /// <param name="client">Discord クライアント</param>
        /// <param name="logging">ロギングサービス</param>
        /// <param name="deltaRaumiDb">データベースコンテキスト</param>
        /// <param name="dataEnsure">データベースヘルパー</param>
        public LevelService(
            DiscordSocketClient client,
            ImprovedLoggingService logging,
            DeltaRaumiDbContext deltaRaumiDb
            , DataEnsure dataEnsure
            )
        {
            _client = client;
            _logger = logging;
            _deltaRaumiDb = deltaRaumiDb;
            _dataEnsure = dataEnsure;
            _random = new Random();
        }

        /// <summary>
        /// LevelsProsessAsyncは、メッセージを処理し、レベルアップの処理を行います。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LevelsProsessAsync(SocketMessage message)
        {
            // メッセージがDMチャネルからのものか、ボットからのものか、Webhookからのものであれば処理しない
            if (!(message is SocketUserMessage) || message.Channel is IDMChannel ||
                message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }

            var guildChannel = message.Channel as ITextChannel;
            var guild = guildChannel.Guild as SocketGuild;
            var guildUser = message.Author as SocketGuildUser;

            // var ensure = new DataEnsure(_deltaRaumiDb, _logger, _client);

            if (guild == null || guildUser == null)
            {
                return;
            }

            try
            {
                // GuildBaseDataの確認と初期化
                var guildData = await _dataEnsure.EnsureGuildBaseDataExistsAsync(guild); //

                // UserBaseDataの確認と初期化
                var userData = await _dataEnsure.EnsureUserBaseDataExistsAsync(guildUser);

                // UserGuildDataの確認と初期化または更新
                var userGuildData = await _dataEnsure.GetOrCreateUserGuildDataAsync(guildData, userData, guild, guildUser);

                // 経験値クールダウンのチェック
                if (DateTime.UtcNow > userGuildData.LatestExp.AddMinutes(1))
                {
                    // 経験値の付与とメッセージカウントの更新
                    await UpdateUserExperienceAsync(userGuildData, guildUser, guild);
                }
                else
                {
                    userGuildData.TotalMessage++;
                    await _deltaRaumiDb.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await _logger.Log($"レベリング処理中にエラーが発生しました: {ex.Message}", "LevelService", ImprovedLoggingService.LogLevel.Error);
            }

            Console.WriteLine("Level OK");
            //return Task.CompletedTask;
        }

        /// <summary>
        /// ユーザーの経験値を更新します
        /// </summary>
        private async Task UpdateUserExperienceAsync(UserGuildDataModel userGuildData, SocketGuildUser user, SocketGuild guild)
        {
            // 前回のレベル
            int previousLevel = CalculateLevel(userGuildData.GuildExp);

            // ランダムな経験値を追加 (1～10)
            int expToAdd = _random.Next(1, 11);
            userGuildData.GuildExp += expToAdd;
            userGuildData.LatestExp = DateTime.UtcNow;
            userGuildData.TotalMessage++;

            // 変更を保存
            await _deltaRaumiDb.SaveChangesAsync();

            // 現在のレベル
            int currentLevel = CalculateLevel(userGuildData.GuildExp);

            // レベルアップの確認
            if (currentLevel > previousLevel)
            {
                await HandleLevelUpAsync(user, guild, currentLevel);
            }

            await _logger.Log($"ユーザー {user.Username} に {expToAdd} 経験値を付与しました。合計: {userGuildData.GuildExp} EXP", "LevelService");
        }

        /// <summary>
        /// レベルアップ処理を行います
        /// </summary>
        private async Task HandleLevelUpAsync(SocketGuildUser user, SocketGuild guild, int newLevel)
        {
            try
            {
                // ギルドデータを取得
                var guildData = await _deltaRaumiDb.GuildBases
                    .Where(g => g.GuildId == guild.Id.ToString())
                    .FirstOrDefaultAsync();

                if (guildData == null)
                {
                    return;
                }

                // レベルアップメッセージを作成
                var embed = new EmbedBuilder()
                    .WithTitle("レベルアップ！")
                    .WithDescription($"{user.Mention} さんがレベル **{newLevel}** に到達しました！")
                    .WithColor(Color.Green)
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithCurrentTimestamp()
                    .Build();

                // ウェルカムチャンネルが設定されている場合はそこに送信
                if (ulong.TryParse(guildData.WelcomeChannnelId, out ulong channelId))
                {
                    var channel = guild.GetTextChannel(channelId);
                    if (channel != null)
                    {
                        await channel.SendMessageAsync(embed: embed);
                    }
                }
                else
                {
                    // メッセージを送信したチャンネルにレベルアップを通知
                    var defaultChannel = guild.DefaultChannel ?? guild.TextChannels.FirstOrDefault();
                    if (defaultChannel != null)
                    {
                        //await defaultChannel.SendMessageAsync(embed: embed);
                    }
                }

                await _logger.Log($"ユーザー {user.Username} がレベル {newLevel} にアップしました", "LevelService");
            }
            catch (Exception ex)
            {
                await _logger.Log($"レベルアップ処理中にエラーが発生しました: {ex.Message}", "LevelService", ImprovedLoggingService.LogLevel.Error);
            }
        }

        /// <summary>
        /// 経験値からレベルを計算します
        /// </summary>
        private int CalculateLevel(int exp)
        {
            // レベル計算式: Level = sqrt(exp / 100)
            return (int)Math.Floor(Math.Sqrt(exp / 100.0));
        }

        /// <summary>
        /// 次のレベルに必要な経験値を計算します
        /// </summary>
        private int CalculateExpForNextLevel(int currentLevel)
        {
            // 次のレベルに必要な経験値: (level + 1)^2 * 100
            int nextLevel = currentLevel + 1;
            return nextLevel * nextLevel * 100;
        }
    }
}
