using Discord.WebSocket;
using System.Net.Sockets;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// StatServiceは、ユーザーのステータスを取得するためのサービスです。
    /// </summary>
    public class StatService
    {
        /// <summary>
        /// StatServiceのインスタンスを初期化します。
        /// </summary>
        public StatService()
        {
            
        }


        /// <summary>
        /// ユーザーのステータスを取得する
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public Task UserStatDetection(SocketMessage Message)
        {
            Console.WriteLine("StatService OK");
            return Task.CompletedTask;
        }
    }
}
