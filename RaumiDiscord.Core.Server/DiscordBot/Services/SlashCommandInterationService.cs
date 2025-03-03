using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.Net.Queue;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol;
using RaumiDiscord.Core.Server.Api.Models;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using System.Linq;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

class SlashCommandInterationService
{
    private readonly DeltaRaumiDbContext DbContext;
    private readonly DiscordSocketClient Client;
    private readonly LoggingService LoggingService;
    //private bool commandUpgrade = false;

    private ulong guildID { get; set; }

    public Optional<IVoiceRegion> Region { get; set; }

    private ComponentInteractionService ComponentInteractionService { get; set; }

    private List<string> VoiceRegionLists { get; set; } = new List<string>();

    private bool command_GuildAvailadle { get; set; } = true;
    private bool command_GlobalAvailadle { get; set; } = true;

    private IAudioClient _audioClient;


    public SlashCommandInterationService(DiscordSocketClient client, LoggingService logger, DeltaRaumiDbContext dbContext)
    {
        this.DbContext = dbContext;
        this.Client = client;
        this.LoggingService = logger;
        //client.Ready += Client_GlobalAvailadle;
        client.GuildAvailable += Client_GuildAvailadle;
        client.SlashCommandExecuted += Client_SlashCommandExcuted;
    }




    private async Task Client_SlashCommandExcuted(SocketSlashCommand command_arg)
    {
        switch (command_arg.Data.Name)
        {
            case "faq":
                await Faq(command_arg);
                break;
            case "pat":
                await Pat(command_arg);
                break;
            //case "vc-region":
            //    await VcRegion(command_arg);
            //    break;
            //case "join":
            //    await JoinVC(command_arg);
            //    break;
            default:
                await LoggingService.LogGeneral($"このコマンドはギルドコマンドに存在しないためギルドコマンドとして実行されませんでした: {command_arg.CommandName}");
                break;
        }
    }

    

    private async Task Client_GuildAvailadle(SocketGuild guild_arg)
    {
        SlashCommandProperties[] commands = GetCmmands();
        try
        {
            if (command_GuildAvailadle == true)
            {
                await guild_arg.DeleteApplicationCommandsAsync();
                await guild_arg.BulkOverwriteApplicationCommandAsync(commands);
                command_GuildAvailadle = false;
            }
            else
            {
                await LoggingService.LogGeneral("ギルドコマンドの更新がスキップされました。");
            }
        }
        catch (HttpException e)
        {
            //前回の謎の苦渋からHttpエラーをどうにかして吐くように変更(本来あるべき姿)
            await LoggingService.LogGeneral($"コマンドの追加中にエラーが発生しました", LoggingService.LogGeneralSeverity.Error);
            await LoggingService.LogGeneral(e.ToString(), LoggingService.LogGeneralSeverity.Fatal);
            await LoggingService.LogGeneral(Newtonsoft.Json.JsonConvert.SerializeObject(e.Errors, Newtonsoft.Json.Formatting.Indented), LoggingService.LogGeneralSeverity.Fatal);
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
        var guild = Client.GetGuild(guildID);

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

    public async Task Pat(SocketSlashCommand command_arg)
    {
        await command_arg.DeferAsync();
        DiscordComponentModel model = new DiscordComponentModel();
        model.CustomId = Guid.NewGuid();

        ButtonBuilder Pat = new ButtonBuilder()
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
        componentBuilder.WithButton(Pat);
        componentBuilder.WithButton(dontPat);

        var msg = await command_arg.FollowupAsync("DeltaRaumiを撫でる？", components: componentBuilder.Build());

        model.ChannelId = command_arg.Channel.Id.ToString();
        model.MessageId = msg.Id.ToString();
        model.DeltaRaumiComponentType = "DeltaraumiPat";
        model.OwnerId = command_arg.User.Id.ToString();

        DbContext.Components.Add(model);
        await DbContext.SaveChangesAsync();
    }

    public async Task Faq(SocketSlashCommand command_arg)
    {
        await command_arg.DeferAsync();
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
        componentBuilder.WithSelectMenu(menuBuilder, 0);

        var msg = await command_arg.FollowupAsync("何を聞きたいんだい？:", components: componentBuilder.Build());

        model.ChannelId = command_arg.Channel.Id.ToString();
        model.MessageId = msg.Id.ToString();
        model.DeltaRaumiComponentType = "FAQ-Menu";
        model.OwnerId = command_arg.User.Id.ToString();

        DbContext.Components.Add(model);

        await DbContext.SaveChangesAsync();
    }

    public async Task listVoiceRegion(SocketVoiceChannel? voiceChannel)
    {
        if (voiceChannel != null)
        {
            var v = await Client.GetVoiceRegionsAsync();
            //regionのリストはnull
            foreach (var item in v)
            {
                VoiceRegionLists.Add(item.Id);
            }
        }
    }
}