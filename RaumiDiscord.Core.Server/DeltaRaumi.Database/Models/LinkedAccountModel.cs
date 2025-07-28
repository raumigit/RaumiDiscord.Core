using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    public class LinkedAccountModel
    {
        [Key]
        public Guid Id { get; set; }
        public string ParentUserId { get; set; }
        public string SubUserId { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
