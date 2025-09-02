using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol;
using NUlid;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;

namespace RaumiDiscord.Core.Server.DiscordBot.Services
{
    internal class WelcomeMessageService
    {
        private bool fileUpload;
        private readonly SocketMessage _message;
        private readonly DiscordWebhookClient _webhook;
        private readonly ImprovedLoggingService _logger;
        private readonly DeltaRaumiDbContext _raumiDB;

        public WelcomeMessageService(SocketMessage message, ImprovedLoggingService loggingService, DeltaRaumiDbContext dbContext)
        {
            _message = message;
            _logger = loggingService;
            _raumiDB = dbContext;
        }
        internal async Task welcomeCardGenerator(SocketGuildUser socketUser)
        {
            string background = @".\Assets\Image\default_bg.png";
            //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            string outputPath = @$".\Tests\namecard\{socketUser.Id}.png";
            string discordname = socketUser.Username;
            string profileImageFlame = @".\Assets\Image\sampleflame.png";

            string avatarUrl = (socketUser as IGuildUser)?.GetGuildAvatarUrl(ImageFormat.Auto)
            ?? socketUser.GetAvatarUrl(ImageFormat.Auto)
            ?? socketUser.GetDefaultAvatarUrl();

            string avatarPath = $@".\Temp\Discordicon\128\{socketUser.Id}.png";

            var guildBaseData = await _raumiDB.GuildBases.Where(g => g.GuildId == socketUser.Guild.Id.ToString()).FirstOrDefaultAsync();

            var sendwelcomechannel = string.IsNullOrEmpty(guildBaseData?.WelcomeChannnelID)
                ? socketUser.Guild.DefaultChannel ?? socketUser.Guild.TextChannels.FirstOrDefault()
                : (ulong.TryParse(guildBaseData.WelcomeChannnelID, out var channelId) ? socketUser.Guild.GetTextChannel(channelId) : null);
            //正しい挙動であればDBのWelcomeChannelIDを探し、あればUlongへ変換。なければデフォルトチャンネルとしてチャンネルの最上位チャンネルを代入　

            Directory.CreateDirectory(@".\Temp\Discordicon\128");
            Directory.CreateDirectory(@".\Tests\namecard");

            //RaumiDiscord.Core.Server.DiscordBot.Services.ImageGenerator();


            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(avatarUrl);
                await File.WriteAllBytesAsync(avatarPath, imageBytes);
            }

            string discordicon = avatarPath;

            try
            {
                using (Image bgImage = Image.Load(background))
                using (Image iconImage = Image.Load(discordicon))
                {
                    int margin = 32;

                    // アイコンを右上に配置
                    int iconX = margin;
                    int iconY = margin;
                    int iconsize = 128;

                    int picmargin = iconsize + margin;

                    FontCollection collection = new();
                    FontFamily fontFamily = collection.Add(fontPath);
                    Font font = fontFamily.CreateFont(48, FontStyle.Regular);
                    Font commentfont = fontFamily.CreateFont(32f, FontStyle.Regular);

                    // テキストの描画位置を計算（中央配置）
                    //TextOptions CommentTextOptions = new(font)
                    //{
                    //    Origin = new PointF(bgImage.Width / 2, bgImage.Height-font.Size),
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    VerticalAlignment = VerticalAlignment.Center
                    //};

                    using Image<Rgba32> backgroundimage = Image.Load<Rgba32>(background);

                    backgroundimage.Mutate(x => x.Resize(1280, 720));

                    // 画像を加工
                    backgroundimage.Mutate(ctx =>
                    {
                        // 背景にアイコンを貼り付け
                        ctx.DrawImage(iconImage, new Point(iconX, iconY), 1.0f);

                        // 名前をアイコン下に配置
                        ctx.DrawText(discordname, font, Color.White, new PointF(margin, picmargin + margin / 2));
                        ctx.DrawText("Welcome", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 2));


                    });

                    // 画像を保存
                    backgroundimage.Save(outputPath);
                    //Console.WriteLine($"画像を保存しました: {outputPath}");
                    await _logger.Log($"画像を保存しました: {outputPath}", $"WelcomeMessageService", ImprovedLoggingService.LogLevel.Info);
                }
                try
                {
                    if (fileUpload == true)
                    {
                        await sendwelcomechannel.SendFileAsync(outputPath);
                    }
                    else
                    {
                        await sendwelcomechannel.SendMessageAsync($"Welcome {socketUser.Nickname}");
                    }
                }
                catch (Exception)
                {
                    await _logger.Log($"カードの送信中にエラーが発生しました。welcomechannnelの値が不正の可能性があります", $"WelcomeMessageService", ImprovedLoggingService.LogLevel.Error);
                }

            }
            catch (Exception ex)
            {
                await _logger.Log($"カード生成にエラーが発生しました: {ex.Message}", $"WelcomeMessageService", ImprovedLoggingService.LogLevel.Error);
            }
        }
    }
}