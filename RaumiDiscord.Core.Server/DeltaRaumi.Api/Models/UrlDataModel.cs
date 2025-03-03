using System.Diagnostics;

namespace RaumiDiscord.Core.Server.Api.Models
{
    public class UrlDataModel
    {
        public uint Id { get; set; }
        public string? Url { get; set; }
        public string? UrlType { get; set; }
        public string? DiscordUser { get; set; }
        public DateTime TTL { get; set; }
    }
}
