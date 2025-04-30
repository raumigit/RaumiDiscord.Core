using Discord;
using Discord.WebSocket;
using System.Drawing;
using System.Threading.Channels;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    public class VoicertcregionService
    {
        public static HashSet<ulong> allowedRoleIds = new HashSet<ulong>
        {
            1329621030637015040,    //ハードコート：ロール名：アプリ開発(ラウミの裏小屋)
            1157017168471400682     //ハードコート：ロール名：VCモデレーター(ラウミの裏小屋)
        };
    }
}
