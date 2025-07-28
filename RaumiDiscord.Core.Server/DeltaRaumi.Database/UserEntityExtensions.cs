using Microsoft.CodeAnalysis.CSharp.Syntax;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Database
{
    public static class UserEntityExtensions
    {
        public static void AddDefaultDiscordData(this UserBaseDataModel baseEntity, bool isBot, bool isWebhook)
        {
            if (baseEntity == null)
            {
                throw new ArgumentNullException(nameof(baseEntity), "UserBaseDataModel cannot be null.");
            }
            // デフォルトのDiscordデータを設定
            //baseEntity.AvatarId = null; // デフォルトではアバターはなし
            //baseEntity.CreatedAt = DateTime.UtcNow; // 現在のUTC日時を設定
            baseEntity.IsBot = isBot;
            baseEntity.IsWebhook = isWebhook;
            baseEntity.Exp = 0; // 初期経験値は0
            baseEntity.Barthday = null; // 初期誕生日は設定しない
            baseEntity.SetToMention = 0; // 初期値は-1
        }
    }
}
