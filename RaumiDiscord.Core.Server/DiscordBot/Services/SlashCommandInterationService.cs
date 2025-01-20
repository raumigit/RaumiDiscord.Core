﻿using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using RaumiDiscord.Core.Server.Models;
using System.Linq;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

class SlashCommandInterationService
{
    private readonly DeltaRaumiDbContext DbContext;
    private readonly DiscordSocketClient Client;
    private readonly LoggingService LoggingService;
    private bool commandUpgrade = false;

    private ulong guildID { get; set; }

    public Optional<IVoiceRegion> Region { get; set; }

    private ComponentInteractionService ComponentInteractionService { get; set; }

    private List<string> VoiceRegionLists { get; set; } = new List<string>();



    public SlashCommandInterationService(DiscordSocketClient client, LoggingService logger, DeltaRaumiDbContext dbContext)
    {
        this.DbContext = dbContext;
        this.Client = client;
        this.LoggingService = logger;
        client.GuildAvailable += Client_GuildAvailadle;
        client.SlashCommandExecuted += Client_SlashCommandExcuted;
        client.Ready += Client_GlobalAvailadle;
        
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
            if (commandUpgrade== true)
            {
                await guild_arg.DeleteApplicationCommandsAsync();
                await guild_arg.BulkOverwriteApplicationCommandAsync(commands);
            }
            else
            {
                await LoggingService.LogGeneral("コマンドの更新がスキップされました。");
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
            await Client.Rest.DeleteAllGlobalCommandsAsync();
            await Client.Rest.BulkOverwriteGlobalCommands(global_Commands);
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

        #region /VcRegion DefaultChannnel
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
        SlashCommandBuilder patBuilder = new SlashCommandBuilder();
        patBuilder.WithName("名刺").WithDescription("名刺を作れます。カードは1920＊720で作られます。");
        globalcommands.Add(patBuilder);
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
        menuBuilder.AddOption("現在の設定値は？","nowsettings", emote: new Emoji("🗒"));
        menuBuilder.AddOption("A3", "B3", emote: new Emoji("⏬"));
        menuBuilder.AddOption("A4", "B4");
        menuBuilder.AddOption("DeltaRaumiのホームページはある?", "website", emote: new Emoji("🌐"));
        menuBuilder.AddOption("Patreonとかしてるの？", "donate", emote: new Emoji("💰"));
        menuBuilder.AddOption("A7?", "B7", emote: new Emoji("📇"));
        menuBuilder.AddOption("A8", "B8", emote: new Emoji("🎮"));
        menuBuilder.AddOption("なぜ24時間上がってないの？", "wayoperate-24", emote: new Emoji("👀"));
        menuBuilder.AddOption("A10", "B10", emote: new Emoji("🗿"));
        menuBuilder.AddOption("A11", "B11", emote: new Emoji("🔻"));
        menuBuilder.AddOption("新しい機能を作る予定は？", "cards", emote: new Emoji("🔧"));

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
        string cmd_region = command_arg.Data.Options.First(op => op.Name == "region").Value.ToString();
        var cmd_vcChannel = command_arg.Data.Options.FirstOrDefault(op => op.Name == "target");
        
        //string ? region_code;
        SocketVoiceChannel voiceChannel = null;
        
        if (cmd_vcChannel != null)
        {
            voiceChannel = cmd_vcChannel.Value as SocketVoiceChannel;
            
            //static Array AllowedRegionCodeへの変更が推奨されている
            //注意：targetがnull以外の場合実行されるコードのため削ってしまうと指定ができない
        }



        await listVoiceRegion(voiceChannel);
        try
        {
            
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
        if (voiceChannel!=null)
        {
            var v = await Client.GetVoiceRegionsAsync();
            //regionのリストはnull
            foreach (var item in v)
            {
                VoiceRegionLists.Add(item.Id);
            }
        }

    }




    internal static async Task GlobalCommandUpdate()
    {
        throw new NotImplementedException();
    }
    internal static void Client_Ready(DiscordSocketClient client)
    {
        throw new NotImplementedException();
    }


    internal static async Task SlashCommandHandler(SocketSlashCommand command)
    {
        throw new NotImplementedException();
    }
}