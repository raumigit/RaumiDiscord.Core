using Discord;
using Discord.Net;
using Discord.WebSocket;
using RaumiDiscord.Core.Server.DiscordBot;

internal class SlashCommandInterationService
{
    
    private readonly DiscordSocketClient Client;
    private readonly LoggingService LoggingService;

    public SlashCommandInterationService(DiscordSocketClient client, LoggingService logger)
    {
        this.Client = client;
        this.LoggingService = logger;
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

            default:
                await LoggingService.LogGeneral($"このコマンドは不明なため実行されませんでした: {command_arg.CommandName}");
                break;
        }
        throw new NotImplementedException();
        
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

    private SlashCommandProperties[] GetCmmands()
    {
        List<SlashCommandBuilder> commands = new List<SlashCommandBuilder>();
        #region /Faq
        SlashCommandBuilder faqBuilder = new SlashCommandBuilder();
        faqBuilder.WithName("faq").WithDescription("FAQメニューを開く");
        commands.Add(faqBuilder);
        #endregion

        #region /Faq
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
    //注意：この先すべて未実装
    private async Task Pat(SocketSlashCommand command_arg)
    {
        await command_arg.DeferAsync();
        RaumiDiscord.Core.Server.Models.DiscordComponentModel model = new RaumiDiscord.Core.Server.Models.DiscordComponentModel();

        
        
        throw new NotImplementedException();
    }

    private async Task Faq(SocketSlashCommand command_arg)
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