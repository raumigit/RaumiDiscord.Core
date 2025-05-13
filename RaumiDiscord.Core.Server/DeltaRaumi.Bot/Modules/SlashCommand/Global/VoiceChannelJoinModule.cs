using Discord;
using Discord.Interactions;
using System.Diagnostics;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    /// <summary>
    /// VoiceChannelJoinModuleは、ボイスチャンネルに参加するためのモジュールです。
    /// </summary>
    public class VoiceChannelJoinModule : InteractionModuleBase<SocketInteractionContext>
    {
        /// <summary>
        /// 音声チャンネルに参加します。
        /// </summary>
        /// <returns>非同期操作を表すタスク</returns>
        [SlashCommand("join", "BOTをボイスチャンネルに呼び出す", runMode: Discord.Interactions.RunMode.Async)]
        public async Task BotJoinAsync(IVoiceChannel? channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await RespondAsync("VCに入る、またはチャンネルを指定する必要があります。", ephemeral: true);
                return;
            }
            var audioClient = await channel.ConnectAsync();
            await RespondAsync("VCに接続しました。\n⚠️実装されていないためしばらくお待ち下さい", ephemeral: true);
            //throw new NotImplementedException();
        }

        private static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
    }
}
