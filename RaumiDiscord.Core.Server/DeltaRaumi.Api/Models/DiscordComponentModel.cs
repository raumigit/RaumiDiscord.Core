using Discord;

namespace RaumiDiscord.Core.Server.Api.Models
{
    public class DiscordComponentModel
    {
        //GUID
        public Guid CustomId { get; set; }

        //時間(ローカル)
        public DateTime CreateTime { get; set; } = DateTime.Now;

        //オーナーのId　初期値は0
        public string OwnerId { get; set; } = "0";

        //チャンネルId　初期値は0
        public string ChannelId { get; set; } = "0";

        //メッセージId　初期値は0
        public string MessageId { get; set; } = "0";

        //用途不明
        public string? DeltaRaumiComponentType { get; set; }
    }
}
