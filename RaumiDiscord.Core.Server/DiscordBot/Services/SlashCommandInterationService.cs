using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RaumiDiscord.Core.Server.DataContext;
using RaumiDiscord.Core.Server.DiscordBot;
using RaumiDiscord.Core.Server.DiscordBot.Services;
using RaumiDiscord.Core.Server.Models;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

class SlashCommandInterationService
{
    private readonly DeltaRaumiDbContext dbContext;
    private readonly DiscordSocketClient Client;
    private readonly LoggingService LoggingService;

    private ComponentInteractionService ComponentInteractionService { get; set; }

    public SlashCommandInterationService(DiscordSocketClient client, LoggingService logger)
    {
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
            await guild_arg.DeleteApplicationCommandsAsync();
            await guild_arg.BulkOverwriteApplicationCommandAsync(commands);
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
            );

        commands.Add(vcRegionBuilder);
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
    //注意：この先すべて未実装
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

        dbContext.Components.Add(model);
        await dbContext.SaveChangesAsync();

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

        dbContext.Components.Add(model);
        
        await dbContext.SaveChangesAsync();
    }

    public async Task VcRegion(SocketSlashCommand command_arg)
    {
        string cmd_reagion = command_arg.Data.Options.First().Value.ToString();
        string region_code;

        try
        {

            switch (cmd_reagion)
            {
                case "auto":
                    region_code = null;
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "brazil":
                    region_code = "brazil";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "hongkong":
                    region_code = "hongkong";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "india":
                    region_code = "india";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "japan":
                    region_code = "japan";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "rotterdam":
                    region_code = "rotterdam";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "russia":
                    region_code = "russia";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "singapore":
                    region_code = "singapore";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "southafrica":
                    region_code = "southafrica";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "us-central":
                    region_code = "us-central";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "us-east":
                    region_code = "us-east";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "us-south":
                    region_code = "us-south";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;
                case "us-west":
                    region_code = "us-west";
                    await VoicertcregionService.HandleRTCSettingsCommand(command_arg, region_code);
                    break;

                default:
                    await LoggingService.LogGeneral($"コマンドオプションが不明(E-R4007)引数：{cmd_reagion}", LoggingService.LogGeneralSeverity.Warning);
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