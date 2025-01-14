using Discord;

namespace RaumiDiscord.Core.Server.Models
{
    public class DiscordComponentModel
    {
        //GUID
        public Guid CustomId { get; set; } 

        //時間(ローカル)
        public DateTime CreateTime { get; set; } = DateTime.Now;

        //オーナーのID　初期値は0
        public ulong OwnerId { get; set; } = 0;

        //チャンネルID　初期値は0
        public ulong ChannnelId { get; set; } = 0;

        //メッセージID　初期値は0
        public ulong MessegeId { get; set; } = 0;

        //用途不明
        public string DeltaRaumiComponentType { get; set; }
    }
}
