using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    /// <summary>
    /// MessageServiceは、Discordのメッセージを処理するためのサービスです。
    /// </summary>
    public class MessageService
    {
        private readonly ImprovedLoggingService _logger;
        private readonly DiscordSocketClient _client;

        /// <summary>
        /// MessageServiceのコンストラクター
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logging"></param>
        public MessageService(DiscordSocketClient client, ImprovedLoggingService logging)
        {
            _client = client;
            _logger = logging;

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
                Console.WriteLine($"*ReceivedServer:");
                Console.WriteLine($"|ReceivedChannel:{message.Channel}");
                Console.WriteLine($"|ReceivedUser:{message.Author}");
                Console.WriteLine($"|MessageReceived:{message.Content}");
                Console.WriteLine($"|CleanContent:{message.CleanContent}");
                Console.WriteLine($"|>EmbedelMessage:{message.Embeds.ToString}");
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

        /// <summary>
        /// LevelsHandlerは、メッセージを受信したときにレベルアップの処理を行います。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LevelsHandler(SocketMessage message)
        {
            await LevelService.LevelsProsessAsync(message);
        }
    }
}