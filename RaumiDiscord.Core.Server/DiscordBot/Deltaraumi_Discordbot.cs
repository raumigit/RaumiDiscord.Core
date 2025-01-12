using Discord.WebSocket;
using Discord;
using Nett;
using RaumiDiscord.Core.Server.DiscordBot;
using Newtonsoft.Json.Linq;
internal class Deltaraumi_Discordbot
{
    public static DiscordSocketClient? Client { get; set; }

    private readonly LoggingService? LoggingService;

    private static readonly SocketVoiceChannel? _voiceChannel;

    public IReadOnlyCollection<SocketGuildUser> ConnectedUsers { get; private set; }

    private readonly SocketGuild _guild;
    public static ulong vc_chid { get; set; }
    public static string vc_region { get; set; }
    public static ulong GuildId { get; private set; }

    internal static async Task Deltaraumi_load()
    {
        var toml = Toml.ReadFile("F:/ProgramData/Deltaraumi.toml");
        var environmentalValue = toml.Get<TomlTable>("EnvironmentalValue");
        try
        {

            string token = environmentalValue.Get<string>("Tokun");

            var _config = new DiscordSocketConfig
            {
                MessageCacheSize = 40,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            Client = new DiscordSocketClient(_config);
            Client.Log += LogAsync;
            Client.Ready += ReadyAsync;
            Client.MessageReceived += MessageReceivedAsync;
            Client.InteractionCreated += InteractionCreatedAsync;
            //Client.Ready += SlashCommandInterationService.Client_Ready;
            Client.SlashCommandExecuted += SlashCommandInterationService.SlashCommandHandler;




            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            await LogAsync(new LogMessage(LogSeverity.Info, "Startup", "Discordプロバイダに接続中"));



            Client.MessageUpdated += MessageUpdated;
            Client.Ready += () =>
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("DeltaRaumi接続中");
                Console.ResetColor();
                Client.SetGameAsync("DeltaRaumi調教中");
                return Task.CompletedTask;
            };

            //Console.WriteLine("....");

            Task task = Task.Run(static () => SlashCommandInterationService.Client_Ready(Client));



            await Task.Delay(-1);
        }
        catch (Exception)
        {

            throw;
        }

    }

    private static async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        // safety-casting is the best way to prevent something being cast from being null.
        // If this check does not pass, it could not be cast to said type.
        if (interaction is SocketMessageComponent component)
        {
            // Check for the ID created in the button mentioned above.
            if (component.Data.CustomId == "pingbtn")
                await interaction.RespondAsync("やぁ、ラウミことデルタラウミだよ。");

            else
                Console.WriteLine("ハンドラーのない ID が飛んできたぞ…？");
        }
    }

    private static async Task ReadyAsync()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{Client.CurrentUser} 接続中");
        Console.ResetColor();

        //return Task.CompletedTask;
    }

    private static async Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        //return Task.CompletedTask;
    }

    private static async Task MessageReceivedAsync(SocketMessage message)
    {
        Console.WriteLine($"*ReceivedServer:{message.Channel}");
        Console.WriteLine($"|ReceivedUser:{message.Author}");
        Console.WriteLine($"|MessageReceived:{message.Content}");
        Console.WriteLine($"|CleanContent:{message.CleanContent}");
        //ボットは自分自身に応答してはなりません。
        if (message.Author.Id == Client.CurrentUser.Id)
            return;


        if (message.Content == "!ping")
        {
            Console.WriteLine(">PING");
            // ドロップダウンとボタンを作成できる新しいコンポーネント ビルダーを作成します。
            var cb = new ComponentBuilder().WithButton("クリック", "pingbtn", ButtonStyle.Primary);

            // ボタンを含む、コンテンツ 'pong' を含むメッセージを送信します。
            // このボタンは、呼び出しに渡される前に .Build() を呼び出してビルドする必要があります。
            await message.Channel.SendMessageAsync("pong!", components: cb.Build());
        }

        
        try
        {
            switch (message.CleanContent)
            {

                case "@Raumi#1195":

                    await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/20241004114923.png", "ん？");
                    break;

                case "@Raumi#1195 VCADD":

                    await message.Channel.SendMessageAsync("該当のVC(924574864143171599)を(勝手に)追加しました");
                    vc_chid = 924574864143171599;

                    break;

                case "@Raumi#1195 Discordリージョン：適当" or "@Raumi#1195 Discordリージョン：" or "@Raumi#1195 reset":

                    await message.Channel.SendMessageAsync("*リージョンを適当に変更中(実装中)");
                    vc_region = null;
                    VC_RtcRegion.SetRTCRegion(message, vc_region);
                    break;

                case "@Raumi#1195 Discordリージョン：HK" or "@Raumi#1195 Discordリージョン：香港" or "@Raumi#1195 VCHK":

                    await message.Channel.SendMessageAsync("*リージョンを香港に変更中(実装中)");
                    vc_region = "hongkong";
                    VC_RtcRegion.SetRTCRegion(message, vc_region);
                    break;

                case "@Raumi#1195 Discordリージョン：JP" or "@Raumi#1195 Discordリージョン：日本" or "@Raumi#1195 VCJP":

                    await message.Channel.SendMessageAsync("*リージョンを日本に変更中(実装中)");
                    vc_region = "japan";
                    VC_RtcRegion.SetRTCRegion(message, vc_region);
                    break;

                case "@Raumi#1195 Discordリージョン：BR" or "@Raumi#1195 Discordリージョン：ブラジル" or "@Raumi#1195 VCBR":

                    await message.Channel.SendMessageAsync("*リージョンをブラジルに変更中(実装中)");
                    vc_region = "brazil";
                    VC_RtcRegion.SetRTCRegion(message, vc_region);
                    break;

                case "@Raumi#1195 Discordリージョン：SG" or "@Raumi#1195 Discordリージョン：シンガポール" or "@Raumi#1195 VCSG":

                    await message.Channel.SendMessageAsync("*リージョンをシンガポールに変更中(実装中)");
                    vc_region = "singapore";
                    VC_RtcRegion.SetRTCRegion(message, vc_region);
                    break;

                case "@Raumi#1195 画像１":

                    //await message.Channel.SendMessageAsync("*TBA");
                    await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/IMG_2265.JPG", "ん？");
                    break;

                case "@Raumi#1195 画像２":

                    //await message.Channel.SendMessageAsync("*TBA");
                    await message.Channel.SendFileAsync("./DiscordBot/Assets/Image/20241004114915.png", "ん？");
                    break;

                case "@Raumi#1195 誰がいる？" or "@Raumi#1195 !vcls":
                    await message.Channel.SendMessageAsync("[銀河スラング]め！この機能は[銀河スラング]過ぎて実装できてないんだYO[銀河スラング]");
                    Console.WriteLine($"未実装");
                    break;


                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("メッセージ送信エラー　(E-M001)");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(e);
            Console.ResetColor();
        }
        

    }
    private static async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }
}