using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using NUlid;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using System;
using System.Net.Sockets;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// StatServiceは、ユーザーのステータスを取得するためのサービスです。
    /// </summary>
    public class StatService
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDB;
        private readonly DataEnsure _dataEnsure;
        private readonly DiscordSocketClient _client;
        private ImprovedLoggingService _logger;

        /// <summary>
        /// StatServiceのインスタンスを初期化します。
        /// </summary>
        public StatService(DiscordSocketClient client,
            ImprovedLoggingService logging,
            DeltaRaumiDbContext deltaRaumiDb,
            DataEnsure dataEnsure)
        {
            _client = client;
            _logger = logging;
            _deltaRaumiDB = deltaRaumiDb;
            _dataEnsure = dataEnsure;
        }
       


        /// <summary>
        /// ユーザーのステータスを取得する
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task UserStatDetection(SocketMessage message)
        {
            // メッセージがDMチャネルからのものか、ボットからのものか、Webhookからのものであれば処理しない
            if (!(message is SocketUserMessage userMessage) || message.Channel is IDMChannel ||
                message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }
            else
            {
                
            }

            var guildChannel = message.Channel as ITextChannel;
            var guild = guildChannel.Guild as SocketGuild;
            var guildUser = message.Author as SocketGuildUser;

            var ensure = new DataEnsure(_deltaRaumiDB, _logger, _client);

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

                //var userGuilsStatsData = await _dataEnsure.EnsureGuildUserDataDataExistsAsync(guildData, userData, guild, guildUser);

                //var userGuildStats = new UserGuildStatsModel
                //{
                //    StatUlid = Ulid.NewUlid(),
                //    GuildId = guild.Id.ToString(),
                //    UserId = guildUser.Id.ToString(),
                //    CreatedAt = DateTime.UtcNow,
                //};
                var handler = new MentionHandling(_deltaRaumiDB, _logger);
                await handler.pingMentions(message);

                //await _deltaRaumiDB.UserGuildStats.AddAsync(userGuildStats);
                await _logger.Log($"Statの記録が完了", "StatService", ImprovedLoggingService.LogLevel.Verbose);


            }
            catch (Exception ex)
            {
                await _logger.Log($"Stat機能の処理中にエラーが発生しました: {ex.Message}", "StatService", ImprovedLoggingService.LogLevel.Error);

            }
            Console.WriteLine("StatService OK");
        }
    }
}
