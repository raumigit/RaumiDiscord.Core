using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.old;

class SlashCommandInterationService
{
    private readonly DeltaRaumiDbContext _dbContext;
    private readonly DiscordSocketClient _client;
    private readonly ImprovedLoggingService _loggingService;
    //private bool commandUpgrade = false;

    private ulong GuildId { get; set; }

    public Optional<IVoiceRegion> Region { get; set; }

    private ComponentInteractionService ComponentInteractionService { get; set; }

    private List<string> VoiceRegionLists { get; set; } = new();

    private bool CommandGuildUpdate { get; set; } = true;
    private bool CommandGlobalAvailadle { get; set; } = true;

    public int CommandGuildCount { get; set; }

    private IAudioClient _audioClient;


    public SlashCommandInterationService(DiscordSocketClient client, ImprovedLoggingService logger, DeltaRaumiDbContext dbContext, IAudioClient audioClient)
    {
        _dbContext = dbContext;
        _audioClient = audioClient;
        _client = client;
        _loggingService = logger;
        //client.Ready += Client_GlobalAvailadle;
        client.GuildAvailable += Client_GuildAvailadle;
        client.SlashCommandExecuted += Client_SlashCommandExcuted;
    }




    private async Task Client_SlashCommandExcuted(SocketSlashCommand commandArg)
    {
        switch (commandArg.Data.Name)
        {
            case "faq":
                await Faq(commandArg);
                break;
            case "pat":
                await Pat(commandArg);
                break;
            //case "vc-region":
            //    await VcRegion(command_arg);
            //    break;
            //case "join":
            //    await JoinVC(command_arg);
            //    break;
            default:

                await _loggingService.Log($"このコマンドはギルドコマンドに存在しないためギルドコマンドとして実行されませんでした: {commandArg.CommandName}", "SlashCommandExcuted", ImprovedLoggingService.LogLevel.Warning);
                break;
        }
    }



    private async Task Client_GuildAvailadle(SocketGuild guildArg)
    {
        ApplicationCommandProperties[] commands = GetCmmands();
        try
        {
            if (CommandGuildUpdate)
            {
                await guildArg.DeleteApplicationCommandsAsync();
                await guildArg.BulkOverwriteApplicationCommandAsync(commands);
                CommandGuildCount++;
                if (CommandGuildCount >= 20)
                {
                    CommandGuildUpdate = false;
                }

            }
            else
            {
                await _loggingService.Log("ギルドコマンドの更新がスキップされました。", "Startup");
            }
        }
        catch (HttpException e)
        {
            //前回の謎の苦渋からHttpエラーをどうにかして吐くように変更(本来あるべき姿)
            await _loggingService.Log($"コマンドの追加中にエラーが発生しました", "Startup", ImprovedLoggingService.LogLevel.Fatal);
            await _loggingService.Log(e.ToString(), "Startup", ImprovedLoggingService.LogLevel.Fatal);
            await _loggingService.Log(Newtonsoft.Json.JsonConvert.SerializeObject(e.Errors, Newtonsoft.Json.Formatting.Indented), "Startup", ImprovedLoggingService.LogLevel.Fatal);
            Environment.Exit(1);
            //続行させてもいいけどぶち切ったほうが良さそうと判断
        }
    }



    /// <summary>
    /// ギルドコマンドを設定するためのリストが作られます。
    /// </summary>
    /// <returns></returns>
    private SlashCommandProperties[] GetCmmands()
    {
        _client.GetGuild(GuildId);

        List<SlashCommandBuilder> commands = new List<SlashCommandBuilder>();
        #region /Faq
        SlashCommandBuilder faqBuilder = new SlashCommandBuilder();
        faqBuilder.WithName("faq").WithDescription("FAQメニューを開く");
        commands.Add(faqBuilder);
        #endregion

        #region /Pat
        SlashCommandBuilder patBuilder = new SlashCommandBuilder();
        patBuilder.WithName("pat").WithDescription("この狐を撫でてみる");
        commands.Add(patBuilder);
        #endregion



        List<SlashCommandProperties> slashCommandBuildCommands = new List<SlashCommandProperties>();
        foreach (SlashCommandBuilder builder1 in commands)
        {
            slashCommandBuildCommands.Add(builder1.Build());
        }
        return slashCommandBuildCommands.ToArray();
    }

    /// <summary>
    /// グローバルコマンドを設定するためのリストが作られます。
    /// </summary>
    /// <returns></returns>
    private SlashCommandProperties[] GetGlobalCommands()
    {
        List<SlashCommandBuilder> globalcommands = new List<SlashCommandBuilder>();

        //#region
        //SlashCommandBuilder GlobalBuilder = new SlashCommandBuilder();
        //GlobalBuilder.WithName("コマンド名").WithDescription("説明");
        //globalcommands.Add(GlobalBuilder);
        //#endregion

        List<SlashCommandProperties> slashGlobalCommandsBuilder = new List<SlashCommandProperties>();
        foreach (var item in globalcommands)
        {
            slashGlobalCommandsBuilder.Add(item.Build());
        }
        return slashGlobalCommandsBuilder.ToArray();
    }

    public async Task Pat(SocketSlashCommand commandArg)
    {
        await commandArg.DeferAsync();
        DiscordComponentModel model = new DiscordComponentModel();
        model.CustomId = Guid.NewGuid();

        ButtonBuilder pat = new ButtonBuilder()
        {
            Label = "撫でてみる",
            CustomId = "DoPat",
            Style = ButtonStyle.Success,
        };
        ButtonBuilder dontPat = new ButtonBuilder()
        {
            Label = "やめておく",
            CustomId = "DontPat",
            Style = ButtonStyle.Secondary,
        };

        ComponentBuilder componentBuilder = new ComponentBuilder();
        componentBuilder.WithButton(pat);
        componentBuilder.WithButton(dontPat);

        var msg = await commandArg.FollowupAsync("DeltaRaumiを撫でる？", components: componentBuilder.Build());

        model.ChannelId = commandArg.Channel.Id.ToString();
        model.MessageId = msg.Id.ToString();
        model.DeltaRaumiComponentType = "DeltaraumiPat";
        model.OwnerId = commandArg.User.Id.ToString();

        _dbContext.Components.Add(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Faq(SocketSlashCommand commandArg)
    {
        await commandArg.DeferAsync();
        DiscordComponentModel model = new DiscordComponentModel();
        model.CustomId = Guid.NewGuid();
        SelectMenuBuilder menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithPlaceholder("オプションを選択");
        menuBuilder.WithCustomId(model.CustomId.ToString());
        menuBuilder.AddOption("DeltaRaumiとは？", "about", emote: new Emoji("❓"));
        menuBuilder.AddOption("現在の設定値は？", "nowsettings", emote: new Emoji("🗒"));
        menuBuilder.AddOption("サーバーの状態は?", "serverstat", emote: new Emoji("📶"));
        menuBuilder.AddOption("DeltaRaumiのホームページはある?", "website", emote: new Emoji("🌐"));
        menuBuilder.AddOption("Patreonとかしてるの？", "donate", emote: new Emoji("💰"));
        menuBuilder.AddOption("ブックマークって何?", "bookmark", emote: new Emoji("🔖"));
        menuBuilder.AddOption("なぜ24時間上がってないの？", "wayoperate-24", emote: new Emoji("👀"));
        menuBuilder.AddOption("新しい機能を作る予定は？", "enhancement", emote: new Emoji("🔧"));
        menuBuilder.AddOption("最新で行われた変更は？", "updatenow", emote: new Emoji("☑️"));



        ComponentBuilder componentBuilder = new ComponentBuilder();
        componentBuilder.WithSelectMenu(menuBuilder);

        var msg = await commandArg.FollowupAsync("何を聞きたいんだい？:", components: componentBuilder.Build());

        model.ChannelId = commandArg.Channel.Id.ToString();
        model.MessageId = msg.Id.ToString();
        model.DeltaRaumiComponentType = "FAQ-Menu";
        model.OwnerId = commandArg.User.Id.ToString();

        _dbContext.Components.Add(model);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ListVoiceRegion(SocketVoiceChannel? voiceChannel)
    {
        if (voiceChannel != null)
        {
            var v = await _client.GetVoiceRegionsAsync();
            //regionのリストはnull
            foreach (var item in v)
            {
                VoiceRegionLists.Add(item.Id);
            }
        }
    }
}