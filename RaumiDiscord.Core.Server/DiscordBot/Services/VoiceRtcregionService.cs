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
        public static HashSet<ulong> allowedRoleIds = new HashSet<ulong>
        {
            1329621030637015040,    //ハードコート：ロール名：アプリ開発(ラウミの裏小屋)
            1157017168471400682     //ハードコート：ロール名：VCモデレーター(ラウミの裏小屋)
        };
        

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

        internal static async Task HandleRTCSettingsCommand(SocketSlashCommand command, string region_code, SocketVoiceChannel cmd_vcChannel)
        {
            
            Console.WriteLine(new LogMessage(LogSeverity.Info, "RTCfunc", $"{region_code}" ));


            var guildId = (command.Channel as SocketGuildChannel)?.Guild;
            var guildUser = (SocketGuildUser)command.User;
            //var vchannel = guildId.GetVoiceChannel(VoiceChannelId);
            var userVoiceChannel = guildUser.VoiceChannel;
            bool AllowRores = guildUser.Roles.Any(role => allowedRoleIds.Contains(role.Id));

            if (!guildUser.GuildPermissions.ManageChannels && !AllowRores) 
            {
                await command.RespondAsync($"権限不足：大いなる力にはそれ相応の責任があるのです…", ephemeral: true);
                return;
            }
            if (cmd_vcChannel != null)
            {
                ulong VoiceChannelId = cmd_vcChannel.Id;
                Console.WriteLine($"引数で追加されました。{VoiceChannelId}");
            }
            if (cmd_vcChannel == null && userVoiceChannel != null)//
            {
                cmd_vcChannel = userVoiceChannel;
                Console.WriteLine($"コマンド使用ユーザーのチャンネルを使って追加されました。{cmd_vcChannel}");

            }
            if (cmd_vcChannel == null)
            {
                
                await command.RespondAsync($"指定されたIDのボイスチャンネルが見つかりません。\n" +
                    $"ボイスチャンネルに入ってからコマンドを実行するかVCを指定してください。\n" +
                    $"現在のチャンネルID:null",ephemeral:true);
                return;
            }

            try
            {
                await cmd_vcChannel.ModifyAsync(properties => { properties.RTCRegion = region_code =="auto"?null:region_code; });
                if (region_code == null)
                {
                    await command.RespondAsync($"チャンネル `{cmd_vcChannel.Name}` のリージョンを`AUTO`に変更しました。");
                }
                else
                {
                    await command.RespondAsync($"チャンネル `{cmd_vcChannel.Name}` のリージョンを `{region_code}` に変更しました。");
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
