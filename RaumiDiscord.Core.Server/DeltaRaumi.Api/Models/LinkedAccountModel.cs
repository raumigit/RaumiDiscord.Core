namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Models
{
    public class LinkedAccountModel
    {
        Guid Id { get; set; }
        public ulong ParentUserId { get; set; }
        public ulong SubUserId { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
