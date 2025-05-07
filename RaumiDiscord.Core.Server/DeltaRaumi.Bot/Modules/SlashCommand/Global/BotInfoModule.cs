using Discord.Interactions;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    /// <summary>
    /// BotInfoModuleは、ボットの情報を取得するためのモジュールです。
    /// </summary>
    public class BotInfoModule : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// InteractionServiceは、インタラクションサービスのインスタンスです。
        /// </summary>
        public InteractionService Commands { get; set; }

        private InteractionHandler handler;

        /// <summary>
        /// BotInfoModuleのコンストラクタです。
        /// </summary>
        public BotInfoModule()
        {
        }

        /// <summary>
        /// PingCommandは、ボットのレイテンシを測定するためのコマンドです。
        /// </summary>
        /// <returns></returns>
        [SlashCommand("ping", "pingをします")]
        public async Task PingCommand()
        {
            await RespondAsync($"DiscordGateway:{Context.Client.Latency}ms");
        }
    }
}
