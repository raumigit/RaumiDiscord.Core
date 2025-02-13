using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;
using static Nett.TomlObjectFactory;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    internal class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly List<ulong> _guildIds; // ギルドIDリスト

        //private readonly IConfiguration _configuration;

        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration config, List<ulong>? guildIds)
        {
            _client = client;
            this._handler = handler;
            _services = services;
            _guildIds = guildIds;
            //_configuration = config;
        }

        public static Task Log(LogMessage msg) => Task.Run(() => Console.WriteLine(msg.ToString()));

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
            await Log(new LogMessage(LogSeverity.Info, "InteractionHandler", $"Completed"));
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(new LogMessage(LogSeverity.Info, "Log", $"{log}"));
            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            // グローバルコマンド登録
            await RegisterGlobalCommandsAsync();

            // ギルドごとのコマンド登録
            //foreach (var guildId in _guildIds)
            //{
            //    await RegisterGuildCommandsAsync(guildId);
            //}
        }

        private async Task RegisterGlobalCommandsAsync()
        {
            await _handler.RegisterCommandsGloballyAsync();
            await Log(new LogMessage(LogSeverity.Info, "InteractionHandler", "Registered global commands"));
        }

        public async Task RegisterGuildCommandsAsync(ulong guildId)
        {
            await _handler.RegisterCommandsToGuildAsync(guildId);
            await Log(new LogMessage(LogSeverity.Info, "InteractionHandler", $"Registered commands for guild: {guildId}"));
        }

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
                            // 埋め込む
                            await Log(new LogMessage(LogSeverity.Warning, "InteractionHandler", $"{result.ErrorReason}"));
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
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
                        // implement
                        Log(new LogMessage(LogSeverity.Warning, "InteractionHandler", $"{result.ErrorReason}"));
                        break;
                    default:
                        break;
                }

            return Task.CompletedTask;
        }
    }
}