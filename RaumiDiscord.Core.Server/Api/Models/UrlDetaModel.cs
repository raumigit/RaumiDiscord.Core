using System.Diagnostics;

namespace RaumiDiscord.Core.Server.Api.Models
{
    public class UrlDetaModel
    {
        public uint Id { get; set; }
        public string? Url { get; set; }
        public string? UrlType { get; set; }
        public DateTime TTL { get; set; }
    }
}
