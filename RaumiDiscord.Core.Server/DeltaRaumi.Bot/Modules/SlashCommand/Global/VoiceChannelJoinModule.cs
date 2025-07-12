using Discord;
using Discord.Audio;
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
            await voicesendasync(audioClient);
            //throw new NotImplementedException();
        }

        private async Task voicesendasync(IAudioClient audioClient)
        {
            // 音声ファイルのパス
            string filePath = @".\Assets\Fail.wav";

            // ファイルが存在するか確認
            if (!File.Exists(filePath))
            {
                await ModifyOriginalResponseAsync(msg => msg.Content = "音声ファイルが見つかりませんでした。");
                return;
            }
            // 音声を再生
            await SendAudioAsync(audioClient, filePath);
        }

        private async Task SendAudioAsync(IAudioClient audioClient, string filePath)
        {
            // FFmpegを使用してオーディオストリームを作成
            using (var ffmpeg = CreateStream(filePath))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                try
                {
                    // バッファサイズを設定
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    // オーディオストリームをDiscordに送信
                    while ((bytesRead = await output.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await discord.WriteAsync(buffer, 0, bytesRead);
                    }

                    // ストリームをフラッシュして終了
                    await discord.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"音声の送信中にエラーが発生しました: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// FFmpegを使用してオーディオストリームを作成します。
        /// </summary>
        /// <param name="path">音声ファイルのパス</param>
        /// <returns>プロセス</returns>
        private static Process CreateStream(string path)
        {
#pragma warning disable CS8603 // Null 参照戻り値である可能性があります。
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
#pragma warning restore CS8603 // Null 参照戻り値である可能性があります。
        }
    }
}

