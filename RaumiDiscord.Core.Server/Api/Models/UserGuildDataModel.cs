using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaumiDiscord.Core.Server.Api.Models
{
    [Keyless]
    public class UserGuildData
    {
        [ForeignKey("GuildId")]
        public required GuildBaseData GuildBaseData { get; set; }
        [ForeignKey("UserId")]
        public required UserBaseData UserBaseData { get; set; }
        
        
        public ulong GuildId { get; set; }
        
        public ulong UserId { get; set; }
        public string? GuildAvatarId { get; set; }
        public int GuildUserFlags { get; set; }
        public DateTime JoinedAt {  get; set; }
        public DateTime TimedOutUntil { get; set; }
        public int GuildLebel {  get; set; }
        public int TotalMessage {  get; set; } 

    }
}
