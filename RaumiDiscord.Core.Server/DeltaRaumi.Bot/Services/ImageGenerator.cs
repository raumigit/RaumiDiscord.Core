using Discord;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.CompilerServices;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    /// <summary>
    /// ImageGeneratorは、画像を生成するためのクラスです。
    /// </summary>
    public class ImageGenerator
    {
        private readonly DeltaRaumiDbContext _deltaRaumiDb;
        private readonly ImprovedLoggingService _logger;
        private bool fileUpload;

        /// <summary>
        /// ImageGeneratorのコンストラクタ
        /// </summary>
        public ImageGenerator()
        {
            //_deltaRaumiDb = deltaRaumiDb;
            //_logger = logger;
        }
        public static async Task WelcomeCardGenerater(string guildName,string userID, string userName,string avatarPath,string avatarUrl,string comment)
        {
            string background = @".\Assets\Image\default_bg.png";
            //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            string outputPath = @$".\Tests\welcomecard\{userID}.png";
            //string discordname = Context.User.Username;
            string profileImageFlame = @".\Assets\Image\sampleflame.png";

            //string avatarUrl = (Context.User as IGuildUser)?.GetGuildAvatarUrl(ImageFormat.Auto)
            //?? Context.User.GetAvatarUrl(ImageFormat.Auto)
            //?? Context.User.GetDefaultAvatarUrl();

            //string avatarPath = $@".\Temp\Discordicon\128\{Context.User.Id}.png";

            

            //RaumiDiscord.Core.Server.DiscordBot.Services.ImageGenerator();


            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(avatarUrl);
                await File.WriteAllBytesAsync(avatarPath, imageBytes);
            }

            string discordicon = avatarPath;

            try
            {
                //await DeferAsync(ephemeral: true);
                using (Image bgImage = Image.Load(background))
                using (Image iconImage = Image.Load(discordicon))
                {
                    int margin = 32;

                    // アイコンを右上に配置
                    int iconX = margin;
                    int iconY = margin;
                    int iconsize = 128;


                    int picmargin = iconsize + margin;

                    // フォントの設定
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
                        ctx.DrawText(userName, font, Color.White, new PointF(margin, picmargin + margin / 2));
                        ctx.DrawText($"Welcome", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 2));
                        if (comment != null)
                        {
                            ctx.DrawText(comment, commentfont, Color.Black, new PointF(margin, backgroundimage.Height - (margin + commentfont.Size)));
                        }
                    });

                    // 画像を保存
                    backgroundimage.Save(outputPath);
                    Console.WriteLine($"画像を保存しました: {outputPath}");
                }

            }
            catch (Exception ex)
            {
                //await DeleteOriginalResponseAsync();
                //await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```", ephemeral: true);
            }
        }
        public static async Task UserCardGenerater(string guildName, string userID, string userName, string avatarPath, string avatarUrl, string comment,int exp)
        {
            string background = @".\Assets\Image\default_bg.png";
            //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            string outputPath = @$".\Tests\namecard\{userID}.png";
            //string discordname = userName;
            string profileImageFlame = @".\Assets\Image\sampleflame.png";

            //string avatarUrl = (Context.User as IGuildUser)?.GetGuildAvatarUrl(ImageFormat.Auto)
            //?? Context.User.GetAvatarUrl(ImageFormat.Auto)
            //?? Context.User.GetDefaultAvatarUrl();

            //string avatarPath = $@".\Temp\Discordicon\128\{userName}.png";



            //RaumiDiscord.Core.Server.DiscordBot.Services.ImageGenerator();


            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(avatarUrl);
                await File.WriteAllBytesAsync(avatarPath, imageBytes);
            }

            string discordicon = avatarPath;

            try
            {
                //await DeferAsync(ephemeral: true);
                using (Image bgImage = Image.Load(background))
                using (Image iconImage = Image.Load(discordicon))
                {
                    int margin = 32;

                    // アイコンを右上に配置
                    int iconX = margin;
                    int iconY = margin;
                    int iconsize = 128;


                    int picmargin = iconsize + margin;

                    // フォントの設定
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
                        ctx.DrawText(userName, font, Color.White, new PointF(margin, picmargin + margin / 2));
                        ctx.DrawText($"{guildName}", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 2));
                        ctx.DrawText($"exp:{exp}", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 3));
                        if (comment != null)
                        {
                            ctx.DrawText(comment, commentfont, Color.Black, new PointF(margin, backgroundimage.Height - (margin + commentfont.Size)));
                        }
                    });

                    // 画像を保存
                    backgroundimage.Save(outputPath);
                    Console.WriteLine($"画像を保存しました: {outputPath}");
                }

               
            }
            catch (Exception ex)
            {
                
                //await DeleteOriginalResponseAsync();
                //await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```", ephemeral: true);
            }
        }

        public void CreateDirectory()
        {
            Directory.CreateDirectory(@".\Temp\Discordicon\128");
            Directory.CreateDirectory(@".\Tests\namecard");
        }
    }
}
