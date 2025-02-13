using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Local
{
    //public class VoiceChangeRegionModule

    public class VoiceChangeRegionModule : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly Dictionary<string, string?> RegionOptions = new()
        {
            { "auto", null},
            { "brazil", "brazil"},
            { "hongkong", "hongkong"},
            { "india", "india"},
            { "japan", "japan"},
            { "rotterdam", "rotterdam"},
            { "russia", "russia"},
            { "singapore", "singapore"},
            { "southafrica", "southafrica"},
            { "us-central", "us-central"},
            { "us-east", "us-east"},
            { "us-south", "us-south"},
            { "us-west", "us-west"}
        };

        [SlashCommand("vc-region", "VCのリージョンを変更します。")]
        public async Task SetVoiceCommand(
            [Summary("region", "リージョンを選択")]
            [Choice("Auto", "auto")]
            [Choice("Brazil", "brazil")]
            [Choice("HongKong", "hongkong")]
            [Choice("India", "india")]
            [Choice("Japan", "japan")]
            [Choice("Rotterdam", "rotterdam")]
            [Choice("Russia", "russia")]
            [Choice("Singapore", "singapore")]
            [Choice("SouthAfrica", "southafrica")]
            [Choice("US Central", "us-central")]
            [Choice("US East", "us-east")]
            [Choice("US South", "us-south")]
            [Choice("US West", "us-west")]
            string region,

            [Summary("voice_channel", "VCを選択(VCに入っていれば省略可)")]
            [Autocomplete(typeof(VoiceChannelAutocompleteHandler))]
            SocketVoiceChannel? voiceChannel = null)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null || (!user.GuildPermissions.ManageChannels && !user.Roles.Any(r => r.Permissions.ManageChannels)))
            {
                await RespondAsync("このコマンドを使用するのに必要な権限がありません。", ephemeral: true);
                return;
            }

            var userVoiceChannel = (user.VoiceChannel as SocketVoiceChannel);
            if (voiceChannel == null)
            {
                voiceChannel = userVoiceChannel;
            }

            if (voiceChannel == null)
            {
                await RespondAsync("変更できるボイスチャネルが見つかりません。", ephemeral: true);
                return;
            }

            string selectedRegion = RegionOptions[region];
            await voiceChannel.ModifyAsync(vc => vc.RTCRegion = selectedRegion);
            await RespondAsync($"チャンネル `{voiceChannel.Name}` のリージョンを `{selectedRegion ??"Auto"}`に変更しました。");
        }
    }

    public class VoiceChannelAutocompleteHandler : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var guild = (context.Guild as SocketGuild);
            if (guild == null)
                return Task.FromResult(AutocompletionResult.FromSuccess());

            var voiceChannels = guild.VoiceChannels
                .Select(vc => new AutocompleteResult(vc.Name, vc.Id.ToString()))
                .ToList();

            return Task.FromResult(AutocompletionResult.FromSuccess(voiceChannels));
        }
    }
}
