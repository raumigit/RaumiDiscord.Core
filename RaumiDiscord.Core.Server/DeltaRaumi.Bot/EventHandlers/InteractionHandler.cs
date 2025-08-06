using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using System.Reflection;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers
{
    internal class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly List<ulong> _guildIds; // ギルドIDリスト
        private readonly ImprovedLoggingService _logger; // 追加: ロギングサービス


        public InteractionHandler(
            DiscordSocketClient client,
            InteractionService handler,
            IServiceProvider services,
            IConfiguration config,
            List<ulong>? guildIds,
            ImprovedLoggingService logger) // 追加: ロギングサービスのパラメータ
        {
            _client = client;
            _handler = handler;
            _services = services;
            _guildIds = guildIds;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // ロギングサービスをnullチェック
        }



        public async Task InitializeAsync()
        {
            // クライアントの準備ができたら処理し、コマンドを登録できるようにします。
            _client.Ready += RegisterCommandsAsync;
            _handler.Log += LogAsync;

            // InteractionModuleBase<T> を継承するパブリックモジュールを InteractionService に追加します
            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // InteractionCreated ペイロードを処理して Interactions コマンドを実行します
            _client.InteractionCreated += HandleInteraction;

            // コマンド実行の結果も処理します
            _handler.InteractionExecuted += HandleInteractionExecute;
            await _logger.Log("Initialization completed", "InteractionHandler", ImprovedLoggingService.LogLevel.Info);
        }

        private Task LogAsync(LogMessage log)
        {
            // LogMessageをImprovedLoggingServiceに変換して使用
            ImprovedLoggingService.LogLevel level = DiscordLoggingAdapter.ConvertDiscordLogLevel(log.Severity);

            _logger.Log($"{log.Message} | {log.Exception}", $"{log.Source}", level);
            return Task.CompletedTask;
        }

        // Discord SDKのログレベルを変換するヘルパーメソッド
        private ImprovedLoggingService.LogLevel ConvertDiscordLogLevel(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Critical => ImprovedLoggingService.LogLevel.Fatal,
                LogSeverity.Error => ImprovedLoggingService.LogLevel.Error,
                LogSeverity.Warning => ImprovedLoggingService.LogLevel.Warning,
                LogSeverity.Info => ImprovedLoggingService.LogLevel.Info,
                LogSeverity.Debug => ImprovedLoggingService.LogLevel.Debug,
                LogSeverity.Verbose => ImprovedLoggingService.LogLevel.Verbose,
                _ => ImprovedLoggingService.LogLevel.Info
            };
        }

        private async Task RegisterCommandsAsync()
        {
            // グローバルコマンド登録
            await RegisterGlobalCommandsAsync();

            //// ギルドごとのコマンド登録(実行はしない)
            //foreach (var guildId in _guildIds)
            //{
            //    await RegisterGuildCommandsAsync(guildId);
            //}
        }

        private async Task RegisterGlobalCommandsAsync()
        {
            await _handler.RegisterCommandsGloballyAsync();
            await _logger.Log("Registered global commands", "InteractionHandler", ImprovedLoggingService.LogLevel.Info);
        }

        //public async Task RegisterGuildCommandsAsync(ulong guildId)
        //{
        //    await _handler.RegisterCommandsToGuildAsync(guildId);
        //    await Log(new LogMessage(LogSeverity.Info, "InteractionHandler", $"Registered commands for guild: {guildId}"));
        //}

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // InteractionModuleBase<T> モジュールのジェネリック型パラメータに一致する実行コンテキストを作成します。
                var context = new SocketInteractionContext(_client, interaction);

                // 受信したコマンドを実行します。
                var result = await _handler.ExecuteCommandAsync(context, _services);

                // InteractionFramework の非同期性により、ここでの結果は常に成功になる可能性があります。
                // そのため、InteractionExecuted イベントも処理する必要があります。
                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // 警告をログに記録
                            await _logger.Log($"{result.ErrorReason}", "InteractionHandler", ImprovedLoggingService.LogLevel.Warning);
                            break;
                        default:
                            break;
                    }
            }
            catch (Exception ex)
            {
                // エラーをログに記録
                await _logger.Log($"Error handling interaction: {ex.Message}", "InteractionHandler", ImprovedLoggingService.LogLevel.Error);

                // スラッシュコマンドの実行が失敗した場合、元の対話確認が残る可能性が高くなります。元の対話確認を削除することをおすすめします。
                // 応答を返すか、少なくともコマンドの実行中に問題が発生したことをユーザーに知らせます。
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

        private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, Discord.Interactions.IResult result)
        {
            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // 警告をロギングサービスに記録
                        _logger.Log(result.ErrorReason, "InteractionHandler", ImprovedLoggingService.LogLevel.Warning);
                        break;
                    default:
                        break;
                }

            return Task.CompletedTask;
        }
    }
}