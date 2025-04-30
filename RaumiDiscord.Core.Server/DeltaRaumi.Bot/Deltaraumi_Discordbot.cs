using Discord.WebSocket;
using Discord;
using Nett;
using Newtonsoft.Json.Linq;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DiscordBot.Data;
using Discord.Net;
using RaumiDiscord.Core.Server.DataContext;
using Discord.Rest;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    internal class Deltaraumi_Discordbot
    {
        //public IReadOnlyCollection<SocketGuildUser> ConnectedUsers { get; private set; }
        //VCユーザーを取るためのコマンド　検証と実装はこれから
        public List<ulong>? guildIDs { get; private set; }

        public static ulong vc_chid { get; set; }
        public static string? vc_region { get; set; }

        //public static ulong GuildId { get; private set; }
        //おそらく出番なし

        public static DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private IServiceProvider? _services;
        private static IConfiguration _configuration;
        public static Configuration? _Config { get; set; }
        public static SqlMode AppSqlMode { get; set; }
        public static ImprovedLoggingService _logger;

        public enum SqlMode { Sqlite, MariaDb }
        private DiscordCoordinationService? DiscordCoordinationService;

        public Deltaraumi_Discordbot()
        {
            _logger = new ImprovedLoggingService();
            
        }


        public static void Deltaraumi_load(string[] args)
        {
            Console.WriteLine($"Directories.Config = {Directories.Config}");
            Console.WriteLine($"Directories.Appdata = {Directories.AppData}");
            Console.WriteLine($"Directories.ProgramData = {Directories.ProgramData}");
            Console.WriteLine();
            try
            {
                new Deltaraumi_Discordbot().MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        private async Task MainAsync(string[] args)
        {
            try
            {
                await _logger.Log("DeltaRaumiを初期化中", "Startup");

                _Config = new Configuration().GetConfigFromFile();

                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
                    ConnectionTimeout = 8000,
                    HandlerTimeout = 3000,
                    MessageCacheSize = 1000,
                    LogLevel = LogSeverity.Verbose,
                    LogGatewayIntentWarnings = true,
                    //AlwaysDownloadUsers = true

                });
                _client.Log += LogAsync;
                _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
                _services = BuildServices();

                await _logger.Log("サービスプロバイダを設定中...", "Startup");

                var dbContext = _services.GetRequiredService<DeltaRaumiDbContext>();
                this.DiscordCoordinationService = _services.GetRequiredService<DiscordCoordinationService>();
                
                

                //起動時に新しいデータベース移行を適用する
                var migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Count() > 0)
                {
                    Console.WriteLine("Applying database migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("Done.");
                }

                await _logger.Log("初期化が完了", "Startup");

                await _logger.Log("ログイン→", "Startup");
                try
                {
                    string _token = _Config.TokenData.Token;

                    await _client.LoginAsync(TokenType.Bot, _token);
                    //await Log(new LogMessage(LogSeverity.Debug, "Startup", $"Tokun:{_token}"));

                    await _client.StartAsync();
                }
                catch (HttpException e)
                {
                    await _logger.Log("ログイン失敗", "Startup", ImprovedLoggingService.LogLevel.Error);
                    await _logger.Log($"{e}", "Startup", ImprovedLoggingService.LogLevel.Error);
                    Environment.Exit(1);
                }

                _client.MessageReceived += MessageReceivedAsync;
                _client.MessageUpdated += MessageUpdated;
                //_client.MessageDeleted += MessegeDeleted;
                //まもなく分離　分離後はサービスプロバイダに登録予定

                guildIDs = _client.Guilds.Select(guild => guild.Id).ToList();
                await _services.GetRequiredService<InteractionHandler>().InitializeAsync();

                Console.ForegroundColor = ConsoleColor.Magenta;
                await _logger.Log("DeltaRaumi接続中", "Startup", ImprovedLoggingService.LogLevel.Notice);
                Console.ResetColor();

                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                await _logger.Log("e.Message", "Startup", ImprovedLoggingService.LogLevel.Fatal);
                await _logger.Log("===============================", "Startup", ImprovedLoggingService.LogLevel.Fatal);
                await _logger.Log("e.StackTrace", "Startup", ImprovedLoggingService.LogLevel.Fatal);


                Environment.Exit(1);
            }
        }


        //複雑なメソッド：https://www.codefactor.io/repository/github/raumigit/raumidiscord.core/file/master/RaumiDiscord.Core.Server/DiscordBot/Deltaraumi_Discordbot.cs
        private async Task MessageReceivedAsync(SocketMessage message)
        {

            Console.WriteLine($"*ReceivedServer:");
            Console.WriteLine($"|ReceivedChannel:{message.Channel}");
            Console.WriteLine($"|ReceivedUser:{message.Author}");
            Console.WriteLine($"|MessageReceived:{message.Content}");
            Console.WriteLine($"|CleanContent:{message.CleanContent}");
            Console.WriteLine($"|>EmbedelMessage:{message.Embeds}");

            //ボットは自分自身に応答してはなりません。
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
            {

            }

            try
            {
                //サイクロマティック複雑度が高く、保守用意性が50切ってるので要修正
                string contentbase = "@Raumi#1195 *";
                switch (message.CleanContent)
                {
                    case "@Raumi#1195":
                        await message.Channel.SendMessageAsync("なに？");
                        break;

                    case string match when System.Text.RegularExpressions.Regex.IsMatch(message.CleanContent, contentbase):

                        await message.Channel.SendMessageAsync("該当するメッセージコマンドはないっぽい…");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                await _logger.Log("メッセージ送信エラー　(E-M4001)", "MessageReceive", ImprovedLoggingService.LogLevel.Warning);
                await _logger.Log($"{e}", "MessageReceive", ImprovedLoggingService.LogLevel.Warning);

            }
        }

        private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // メッセージがキャッシュになかった場合、ダウンロードすると `after` のコピーが取得されます。
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message.Channel}|{message.Author}\n{message.Author}:```diff\n- {message}\n! {after}\n```");

        }

        //private readonly InteractionHandler;
        public static Task LogAsync(LogMessage log)
        {
            // LogMessageをImprovedLoggingServiceに変換して使用
            ImprovedLoggingService.LogLevel level = DiscordLoggingAdapter.ConvertDiscordLogLevel(log.Severity);
            _logger.Log($"{log.Message}", $"{log.Source}", level);
            
            return Task.CompletedTask;
        }



        private IServiceProvider BuildServices()
        => new ServiceCollection()
            .AddDbContext<DeltaRaumiDbContext>()
            .AddSingleton(_client)
            .AddSingleton(_configuration)
            .AddSingleton<CommandService>()
            .AddSingleton<ImprovedLoggingService>()
            .AddSingleton<SlashCommandInterationService>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton<WelcomeMessageService>()
            .AddSingleton<ComponentInteractionService>()
            .AddSingleton<DiscordCoordinationService>()
            .AddSingleton<VoicertcregionService>()
            .AddSingleton<MessageResiveService>()
            .AddSingleton<DiscordLoggingAdapter>()
            .AddSingleton(provider => _client.Guilds.Select(guild => guild.Id).ToList())
            .BuildServiceProvider();
    }
}