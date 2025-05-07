using Discord.WebSocket;
using System.Net.Sockets;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    public class StatService
    {
        public StatService()
        {
            
        }

        

        public async Task UserStatDetection(SocketMessage Message) 
        {
            Console.WriteLine("StatService"+Message.Content);
        }
    }
}
