﻿using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.Utils;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using System.Diagnostics.Metrics;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    /// <summary>
    /// MessageServiceは、Discordのメッセージを処理するためのサービスです。
    /// </summary>
    public class MessageService
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DiscordSocketClient _client;
        private DeltaRaumiDbContext _deltaRaumiDb;
        private readonly DataEnsure _dataEnsure;
        private readonly LevelService _levelService;
        /// <summary>
        /// MessageServiceのコンストラクター
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logging"></param>
        public MessageService(DiscordSocketClient client, ImprovedLoggingService logging, DeltaRaumiDbContext deltaRaumiDb, DataEnsure dataEnsure, LevelService levelService)
        {
            _client = client;
            _logger = logging;
            _deltaRaumiDb = deltaRaumiDb;
            _dataEnsure = dataEnsure;
            _levelService = levelService;


            // イベントハンドラを登録
            //_client.MessageReceived += GetMessageReceivedAsync;
        }

        /// <summary>
        /// GetMessageReceivedAsyncは、メッセージを受信したときに呼び出されるメソッドです。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task GetMessageReceivedAsync(SocketMessage message)
        {
            if (true)
            {
                if (message.Channel is SocketGuildChannel guildChannel)
                {
                    SocketGuild guild=guildChannel.Guild;
                    Console.WriteLine($"*ReceivedServer:{guild.Name}");
                }
                else
                {
                    Console.WriteLine($"*ReceivedServer:DM");
                }
                Console.WriteLine($"|ReceivedChannel:{message.Channel}");
                Console.WriteLine($"|ReceivedUser:{message.Author}");
                Console.WriteLine($"|MessageReceived:{message.Content}");
                Console.WriteLine($"|CleanContent:{message.CleanContent}");
                Console.WriteLine($"|>EmbedelMessage:{message.Embeds.ToJson()}");
            }


            //ボットは自分自身に応答してはなりません。
            if (message.Author.Id == _client.CurrentUser.Id)
                return ; 

            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("pon!");
            }

            try
            {
                //サイクロマティック複雑度が高く、保守用意性が50切ってるので要修正
                string contentbase = "@Raumi#1195 *";
                switch (message.CleanContent)
                {
                    case "@Raumi#1195":
                        await message.Channel.SendMessageAsync("なに？");
                        break;

                    case string match when System.Text.RegularExpressions.Regex.IsMatch(message.CleanContent, contentbase):

                        await message.Channel.SendMessageAsync("該当するメッセージコマンドはないっぽい…");
                        break;
                    default:
                        break;
                }
                
            }
            catch (Exception e)
            {
                await _logger.Log("メッセージ送信エラー　(E-M4001)", "MessageReceive", ImprovedLoggingService.LogLevel.Warning);
                await _logger.Log($"{e}", "MessageReceive", ImprovedLoggingService.LogLevel.Warning);
                
            }
            
        }

        

        internal async Task GetMessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel socketMessageChannel)
        {
            // メッセージがキャッシュになかった場合、ダウンロードすると `after` のコピーが取得されます。
            var message = await before.GetOrDownloadAsync();
            bool embedsChanged = !Enumerable.SequenceEqual(
            message.Embeds.Select(e => e.ToString()),
            after.Embeds.Select(e => e.ToString())
    );
            if (embedsChanged)
            {
                return;
            }
            else if (message.Content != after.Content)
            {
                Console.WriteLine($"{message.Channel}|{message.Author}\n{message.Author}:```diff\n- {message}\n! {after}\n```");
                
            }
        }

        internal async Task GetMessageDeletedAsync(Cacheable<IMessage, ulong> before, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            if (!before.HasValue)
                return;

            // メッセージがキャッシュになかった場合、ダウンロードすると `after` のコピーが取得されます。
            IMessageChannel channel = await cachedChannel.GetOrDownloadAsync();
            var message = before.Value;
            Console.WriteLine($"{channel.Name}|{message.Author}\n{message.Author}:```diff\n- {message}\n```");
        }


        /// <summary>
        /// LevelsHandlerは、メッセージを受信したときにレベルアップの処理を行います。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LevelsHandler(SocketMessage message)
        {
            await _levelService.LevelsProsessAsync(message);
        }

        
    }
}