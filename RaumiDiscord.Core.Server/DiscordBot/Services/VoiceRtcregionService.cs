using Discord;
using Discord.WebSocket;
using System.Drawing;
using System.Threading.Channels;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    public class VoicertcregionService
    {

        private DiscordSocketClient? _client;
        private SocketMessage socketMessage;
        private LoggingService loggingService;

        public VoicertcregionService(DiscordSocketClient client, LoggingService logger)
        {
            this.loggingService = logger;
            this._client = client;
        }

        


        public static async void SetRTCRegion(SocketMessage message, string region)
        {
            ulong channelId = Deltaraumi_Discordbot.vc_chid;

            //if (!ulong.TryParse(parts[1], out ulong channelId))
            //{
            //    await message.Channel.SendMessageAsync("チャンネルIDが無効です。");
            //    return;
            //}

            //var region ;
            var guild = (message.Channel as SocketGuildChannel)?.Guild;

            if (guild == null)
            {
                await message.Channel.SendMessageAsync("サーバー内でコマンドを実行してください。");
                return;
            }

            var channel = guild.GetVoiceChannel(channelId);
            if (channel == null)
            {
                await message.Channel.SendMessageAsync("指定されたIDのボイスチャンネルが見つかりません。");
                await message.Channel.SendMessageAsync($"現在のチャンネルID:{channelId}");
                return;
            }

            try
            {
                await channel.ModifyAsync(properties =>
                {
                    properties.RTCRegion = region == "auto" ? null : region;
                });
                if (region== null)
                {
                    await message.Channel.SendMessageAsync($"チャンネル `{channel.Name}` のリージョンを`AUTO`に変更しました。");
                }
                else
                {
                    await message.Channel.SendMessageAsync($"チャンネル `{channel.Name}` のリージョンを `{region}` に変更しました。");
                }
                
            }
            catch (Exception ex)
            {
                await message.Channel.SendMessageAsync($"リージョン変更に失敗しました(E-4007): {ex.Message}");
            }
        }

        internal static async Task HandleRTCSettingsCommand(SocketSlashCommand command, string region_code)
        {
            
            Console.WriteLine(new LogMessage(LogSeverity.Info, "Discord_Slashcommand", region_code ));

            ulong VoiceChannelId = Deltaraumi_Discordbot.vc_chid;
            var guildId = (command.Channel as SocketGuildChannel)?.Guild;
            var vchannel = guildId.GetVoiceChannel(VoiceChannelId);
            if (VoiceChannelId == 0)
            {
                await command.RespondAsync($"指定されたIDのボイスチャンネルが見つかりません。\n現在のチャンネルID:{VoiceChannelId}");
                
                return;
            }

            try
            {
                await vchannel.ModifyAsync(properties => { properties.RTCRegion = region_code =="auto"?null:region_code; });
                if (region_code == null)
                {
                    await command.RespondAsync($"チャンネル `{vchannel.Name}` のリージョンを`AUTO`に変更しました。");
                }
                else
                {
                    await command.RespondAsync($"チャンネル `{vchannel.Name}` のリージョンを `{region_code}` に変更しました。");
                }
            }
            catch (Exception ex)
            {
                await command.RespondAsync($"リージョン変更に失敗しました(E-4007): {ex.Message}");
            }


            //throw new NotImplementedException();
        }
    }
}
