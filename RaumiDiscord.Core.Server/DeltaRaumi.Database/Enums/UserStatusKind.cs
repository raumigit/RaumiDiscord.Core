namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Enums
{
    /// <summary>
    /// UserStatusの種類を定義する列挙型です。
    /// </summary>
    public class UserStatusKind
    {
        /// <summary>
        /// UserStatusの種類を定義する列挙型です。
        /// これはDiscordのシステムとは関係なく、RaumiDiscordの内部で使用されるステータスを表します。
        /// </summary>
        public enum userstatsType
        {
            /// <summary>
            /// 通常の状態を示す
            /// </summary>
            Normal = 0,
            /// <summary>
            /// UserはBotからBANされた
            /// 詳細：ユーザーはサービスが拒否されサービスを受けることはできません。
            /// </summary>
            UserBAN = 1,
            /// <summary>
            /// UserはBotへのアクセスロックされている
            /// 詳細：ユーザーはBotの機能を使用できませんが、他の機能は利用可能です。
            /// </summary>
            UserLock = 2,
            /// <summary>
            /// UserはWebからBANされている
            /// 詳細：ユーザーはWebインターフェースからのアクセスが拒否され、サービスを受けることはできません。
            /// </summary>
            UserWebBAN = 3,
            /// <summary>
            /// Userは非表示にされている
            /// 詳細：ユーザーはリーダーボード等への提供を拒否しています。この状態では名前等がフォールバックされます。
            /// </summary>
            UserHidden = 4,
            /// <summary>
            /// Userは無効化されている
            /// 詳細：ユーザーはすべての情報が提供されないため、サービスによる反映はされません。
            /// </summary>
            UserDisable = 5,
            /// <summary>
            /// UserはBotであることを示す
            /// 詳細：該当ユーザーはBotであり、通常のユーザーとは異なる扱いを受けます。
            /// </summary>
            UserIsBot = 6,
            /// <summary>
            /// UserはWebhookであることを示す
            /// 詳細：ユーザーはWebhookとして機能し、通常のユーザーとは異なる扱いを受けます。特に、リーダーボートやStatの情報は反映されることはありません。
            /// </summary>
            UserWebHook = 7,
            /// <summary>
            /// Userは管理者権限を持つことを示す
            /// 詳細：該当ユーザーは管理者権限を持ち、特別な操作や設定が可能です。また、インスタンスオーナである可能性があります。
            /// </summary>
            UserAdmin = 8,
            /// <summary>
            /// Userはタイムアウトされている
            /// 詳細：ユーザーは一時的にサービスからのアクセスが制限されており、特定の期間中は一部を除き操作ができません。
            /// </summary>
            UserTimeOut = 9,
            /// <summary>
            /// Userはテスターであることを示す
            /// 詳細：該当ユーザーはテスト目的で特別な権限を持ち、通常のユーザーとは異なる扱いを受ける場合があります。
            /// </summary>
            UserTester = 10,
            /// <summary>
            /// Userはライセンスを有効にしている
            /// 詳細：該当ユーザーはライセンスを持ち、特定の機能やサービスを利用することができます。
            /// </summary>
            UserLisenceEnable = 11,
            /// <summary>
            /// Userはライセンスを無効にしている
            /// 詳細：該当ユーザーはライセンスを持っていないか、ライセンスが無効化されており、特定の機能やサービスを利用できません。
            /// </summary>
            UserLicenseDisable = 12,
            /// <summary>
            /// UserはライセンスをBANされている
            /// 該当ユーザーはライセンスがBANされており、raセンスの付与をすることができません。
            /// </summary>
            UserLicenseBaned = 13,
        }
    }
}
