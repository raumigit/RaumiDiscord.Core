﻿using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using System.Threading.Tasks;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    public class VoiceChannelLeaveModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("leave", "BOTをVCから切断します")]
        public async Task BotLeaveAsync()
        {
            var guild = Context.Guild;
            if (guild==null)
            {
                await RespondAsync("⚠️このコマンドはサーバー内でのみ使用できます。", ephemeral: true);
            }

            var channel = Context.Guild.CurrentUser.VoiceChannel;
            if (channel == null)
            {
                await RespondAsync("VCにBotがいないと思われます。", ephemeral: true);
                return;
            }
            var voiceState = Context.Guild.CurrentUser.VoiceChannel;
            if (voiceState != null)
            {
                await voiceState.DisconnectAsync();
                await RespondAsync("正常に切断しました。");
            }
            else
            {
                await RespondAsync("⚠️VCから退出できませんでした。",ephemeral: true);
            }
            
            
        }
    }
}
