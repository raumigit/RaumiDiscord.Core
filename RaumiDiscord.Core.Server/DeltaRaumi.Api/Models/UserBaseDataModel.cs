﻿using System.ComponentModel.DataAnnotations;
using System.Data;

namespace RaumiDiscord.Core.Server.Api.Models
{
    public class UserBaseData
    {
        /// <summary>
        /// このユーザーのIdを取得または設定します。
        /// この値はulongである必要があります。
        /// </summary>
        /// 注意：SocketMessage Author.Idで取得してください。
        [Key]
        public required string UserId { get; set; }

        /// <summary>
        /// このユーザーのユーザー名を取得または設定します。
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// このユーザーのアバターの識別子を取得または設定します。
        /// </summary>
        public string? AvatarId { get; set; }

        /// <summary>
        /// スノーフレークが作成された日時を取得または設定します。
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// このユーザーがボットとして識別されるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool Isbot { get; set; }

        /// <summary>
        /// このユーザーが Webhook ユーザーであるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsWebhook { get; set; }

        /// <summary>
        /// このユーザーのレベル取得または設定します。
        /// </summary>
        public int Level {  get; set; }

        /// <summary>
        /// このユーザーの誕生日の取得または設定します。
        /// 年が使われることはありません。
        /// </summary>
        public string? Barthday { get; set; }
    }
}
