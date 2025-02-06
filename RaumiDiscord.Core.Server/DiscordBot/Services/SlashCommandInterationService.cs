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
        client.Ready += Client_GlobalAvailadle;
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
            case "vc-region":
                await VcRegion(command_arg);
                break;
            case "join":
                await JoinVC(command_arg);
                break;
            default:
                await LoggingService.LogGeneral($"このコマンドは不明なため実行されませんでした: {command_arg.CommandName}");
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

    private async Task Client_GlobalAvailadle()
    {
        SlashCommandProperties[] global_Commands = GetGlobalCommands();
        try
        {
            if (command_GlobalAvailadle == true)
            {
                await Client.Rest.DeleteAllGlobalCommandsAsync();
                await Client.Rest.BulkOverwriteGlobalCommands(global_Commands);
                await LoggingService.LogGeneral("グローバルコマンドが更新されました。");
                command_GlobalAvailadle = false;
            }
            else
            {
                await LoggingService.LogGeneral("グローバルコマンドの更新がスキップされました。");
            }
        }
        catch (HttpException e)
        {
            //前回の謎の苦渋からHttpエラーをどうにかして吐くように変更(本来あるべき姿)
            await LoggingService.LogGeneral($"コマンドの追加中にエラーが発生しました", LoggingService.LogGeneralSeverity.Error);
            await LoggingService.LogGeneral(e.ToString(), LoggingService.LogGeneralSeverity.Fatal);
            await LoggingService.LogGeneral(Newtonsoft.Json.JsonConvert.SerializeObject(e.Errors, Newtonsoft.Json.Formatting.Indented), LoggingService.LogGeneralSeverity.Fatal);
            Environment.Exit(1);
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

        #region /VcRegion
        SlashCommandBuilder vcRegionBuilder = new SlashCommandBuilder();
        vcRegionBuilder.WithName("vc-region").WithDescription("VCの接続リージョンを変更する")
            .AddOption(new SlashCommandOptionBuilder()
            .WithName("region")
            .WithDescription("変更するVCの地域")
            .WithRequired(true)
            .AddChoice("auto", "auto")
            .AddChoice("brazil", "brazil")
            .AddChoice("hongkong", "hongkong")
            .AddChoice("india", "india")
            .AddChoice("japan", "japan")
            .AddChoice("rotterdam", "rotterdam")
            .AddChoice("russia", "russia")
            .AddChoice("singapore", "singapore")
            .AddChoice("southafrica", "southafrica")
            .AddChoice("us-central", "us-central")
            .AddChoice("us-east", "us-east")
            .AddChoice("us-south", "us-south")
            .AddChoice("us-west", "us-west")
            .WithType(ApplicationCommandOptionType.String)
            )
                .AddOption(new SlashCommandOptionBuilder()
                .WithName("target")
                .WithDescription("変更を加えるチャンネル")
                .WithRequired(false)
                .WithType(ApplicationCommandOptionType.Channel)
                .AddChannelType(ChannelType.Voice)  // ボイスチャンネルのみ指定
            );



        commands.Add(vcRegionBuilder);
        #endregion

        #region /Join
        SlashCommandBuilder joinBuilder = new SlashCommandBuilder();
        joinBuilder.WithName("join").WithDescription("VCに入る(操作できません)");
        commands.Add(joinBuilder);
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

        #region /名刺
        SlashCommandBuilder CardBuilder = new SlashCommandBuilder();
        CardBuilder.WithName("名刺").WithDescription("名刺を作れます。カードは1920＊720で作られます。");
        globalcommands.Add(CardBuilder);
        #endregion

        #region /WebTools
        SlashCommandBuilder webtoolsBuilder = new SlashCommandBuilder();
        webtoolsBuilder.WithName("webtools").WithDescription("Webで使えるツール類に案内されます。(外部のウェブサイトへ行きます)");
        globalcommands.Add(webtoolsBuilder);
        #endregion

        #region /HoYoCode
        SlashCommandBuilder BookmarkBuilder = new SlashCommandBuilder();
        BookmarkBuilder.WithName("bookmark").WithDescription("登録されているURLを表示します。")
            .AddOption(new SlashCommandOptionBuilder()
            .WithName("type")
            .WithDescription("URLのタイプを指定します。")
            .WithRequired(true)
            .AddChoice("URL", "url")
            .AddChoice("GenshinImpact", "GI")
            .AddChoice("HonkaiStarRail", "HSR")
            .AddChoice("ZenlessZoneZero", "ZZZ")
            .WithType(ApplicationCommandOptionType.String)
            );
        globalcommands.Add(BookmarkBuilder);
        #endregion

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

        model.ChannelId = command_arg.Channel.Id;
        model.MessageId = msg.Id;
        model.DeltaRaumiComponentType = "DeltaraumiPat";
        model.OwnerId = command_arg.User.Id;

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

        model.ChannelId = command_arg.Channel.Id;
        model.MessageId = msg.Id;
        model.DeltaRaumiComponentType = "FAQ-Menu";
        model.OwnerId = command_arg.User.Id;

        DbContext.Components.Add(model);

        await DbContext.SaveChangesAsync();
    }

    public async Task VcRegion(SocketSlashCommand command_arg)
    {
        string? cmd_region = command_arg.Data.Options.First(op => op.Name == "region").Value.ToString();
        SocketSlashCommandDataOption? cmd_vcChannel = command_arg.Data.Options.FirstOrDefault(op => op.Name == "target");

        //string ? region_code;
        SocketVoiceChannel? voiceChannel = null;

        if (cmd_vcChannel != null)
        {
            voiceChannel = cmd_vcChannel.Value as SocketVoiceChannel;

            //static Array AllowedRegionCodeへの変更が推奨されている
            //注意：targetがnull以外の場合実行されるコードのため削ってしまうと指定ができない
        }

        await listVoiceRegion(voiceChannel);
        try
        {
            //Complex Method ：https://www.codefactor.io/repository/github/raumigit/raumidiscord.core/file/master/RaumiDiscord.Core.Server/DiscordBot/Services/SlashCommandInterationService.cs
            switch (cmd_region)
            {
                case "auto":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "brazil":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "hongkong":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "india":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "japan":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "rotterdam":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "russia":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "singapore":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "southafrica":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "us-central":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "us-east":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "us-south":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;
                case "us-west":
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, cmd_region, voiceChannel);
                    break;

                default:
                    await LoggingService.LogGeneral($"コマンドオプションが不明(E-R4007)引数：{cmd_region}", LoggingService.LogGeneralSeverity.Warning);
                    break;
            }
        }
        catch (Exception e)
        {
            await LoggingService.LogGeneral($"エラーが発生しました(E-R4007)", severity: LoggingService.LogGeneralSeverity.Error);
            await LoggingService.LogGeneral(e.ToString(), LoggingService.LogGeneralSeverity.Fatal);
            await LoggingService.LogGeneral("");
        }
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

    private async Task JoinVC(SocketSlashCommand command_arg)
    {
        var guilduser = (SocketGuildUser)command_arg.User;
        var userVoiceChannel = guilduser.VoiceChannel;

        //SocketVoiceChannel? voiceChannel;


        if (userVoiceChannel != null)
        {
            await command_arg.RespondAsync($"接続しましたが、音声ストリームが存在しません自動的に切断します。", ephemeral: true);
            _audioClient = await userVoiceChannel.ConnectAsync();
        }
        else
        {
            await command_arg.RespondAsync($"VCに入っていないためスキップされました。", ephemeral: true);
        }
    }
}