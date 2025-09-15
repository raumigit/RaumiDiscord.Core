using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        private bool _fileUpload;

        /// <summary>
        /// ImageGeneratorのコンストラクタ
        /// </summary>
        public ImageGenerator()
        {
            //_deltaRaumiDb = deltaRaumiDb;
            //_logger = logger;
        }
        /// <summary>
        /// WelcomeCardGeneraterは、ウェルカムカードを生成します。
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="avatarPath"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="comment"></param>
        public static async Task WelcomeCardGenerater(string guildName,string userId, string userName,string avatarPath,string avatarUrl,string? comment)
        {
            string background = @".\Assets\Image\default_bg.png";
            //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            string outputPath = @$".\Tests\welcomecard\{userId}.png";
            //string discordname = Context.User.Username;
            // string profileImageFlame = @".\Assets\Image\sampleflame.png";

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
                    await backgroundimage.SaveAsync(outputPath);
                    Console.WriteLine($"画像を保存しました: {outputPath}");
                }

            }
            catch (Exception ex)
            {
                // await DeleteOriginalResponseAsync();
                //await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```", ephemeral: true);
            }
        }
        /// <summary>
        /// UserCardGeneraterは、ユーザーカードを生成します。
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="avatarPath"></param>
        /// <param name="avatarUrl"></param>
        /// <param name="comment"></param>
        /// <param name="exp"></param>
        public static async Task UserCardGenerater(string guildName, string userId, string userName, string avatarPath, string avatarUrl, string? comment,int exp)
        {
            string background = @".\Assets\Image\default_bg.png";
            //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            string outputPath = @$".\Tests\namecard\{userId}.png";
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
                        ctx.DrawText($"Exp:{exp}", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 3+2));
                        if (comment != null)
                        {
                            ctx.DrawText(comment, commentfont, Color.Black, new PointF(margin, backgroundimage.Height - (margin + commentfont.Size)));
                        }
                    });

                    // 画像を保存
                    await backgroundimage.SaveAsync(outputPath);
                    Console.WriteLine($"画像を保存しました: {outputPath}");
                }

               
            }
            catch (Exception ex)
            {
                
                //await DeleteOriginalResponseAsync();
                //await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```", ephemeral: true);
            }
        }

        /// <summary>
        /// ディレクトリを作成します。
        /// </summary>
        public void CreateDirectory()
        {
            Directory.CreateDirectory(@".\Temp\Discordicon\128");
            Directory.CreateDirectory(@".\Tests\namecard");
        }
    }
}
