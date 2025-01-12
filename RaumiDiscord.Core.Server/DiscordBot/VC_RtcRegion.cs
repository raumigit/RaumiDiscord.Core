using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DiscordBot
{
    public class VC_RtcRegion
    {

        private static DiscordSocketClient? _client;

        public static void VCRtcRegion(DiscordSocketClient client)
        {
            //Client = client;

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

                await message.Channel.SendMessageAsync($"チャンネル `{channel.Name}` のリージョンを `{region}` に変更しました。");
            }
            catch (Exception ex)
            {
                await message.Channel.SendMessageAsync($"リージョン変更に失敗しました: {ex.Message}");
            }
        }
    }
}
