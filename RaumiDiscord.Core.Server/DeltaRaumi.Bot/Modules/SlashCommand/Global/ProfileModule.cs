﻿using Discord;
using Discord.Interactions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;

namespace RaumiDiscord.Core.Server.DiscordBot.Modules.SlashCommand.Global
{
    /// <summary>
    ///     ProfileModuleは、ユーザーのプロフィールに関する操作を提供します。
    /// </summary>
    public class ProfileModule 
    {
        /// <summary>
        ///     Profileは、ユーザーのプロフィールに関する操作を提供します。
        /// </summary>
        [Group("profile", "プロフィールに関する操作が行えます。")]
        public class Profile : InteractionModuleBase<SocketInteractionContext>
        {
            /// <summary>
            ///     プロフィールカードを生成するかどうかのフラグ
            /// </summary>
            public bool fileUpload = false;

            //[SlashCommand("generate", "名刺を出力します。")]
            //public async Task CardGenerate()
            //{
            //    await DeferAsync();
            //    string background = @".\Assets\Image\default_bg.png";
            //    //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
            //    string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
            //    string outputPath = @$".\Tests\namecard\{Context.User.Id}.png";
            //    string discordname = Context.User.Username;
            //    string profileImageFlame = @".\Assets\Image\sampleflame.png";

            //    string avatarUrl = Context.User.GetAvatarUrl(Discord.ImageFormat.Png);
            //    string avatarPath = $@".\Temp\Discordicon\128\{Context.User.Id}.png";

            //    Directory.CreateDirectory(@".\Temp\Discordicon\128");
            //    Directory.CreateDirectory(@".\Tests\namecard");

            //    //RaumiDiscord.Core.Server.DiscordBot.Services.ImageGenerator();



            //    using (HttpClient client = new HttpClient())
            //    {
            //        byte[] imageBytes = await client.GetByteArrayAsync(avatarUrl);
            //        await File.WriteAllBytesAsync(avatarPath, imageBytes);
            //    }

            //    string discordicon = avatarPath;

            //    using (Image bgImage = Image.Load(background))
            //    using (Image iconImage = Image.Load(discordicon))
            //    {
            //        int margin = 32;

            //        // アイコンを右上に配置
            //        int iconX = margin;
            //        int iconY = margin;
            //        int iconsize = 128;


            //        int picmargin = iconsize + margin;

            //        // フォントの設定
            //        FontCollection collection = new();
            //        FontFamily fontFamily = collection.Add(fontPath);
            //        Font font = fontFamily.CreateFont(48, FontStyle.Regular);



            //        using Image<Rgba32> backgroundimage = Image.Load<Rgba32>(background);

            //        backgroundimage.Mutate(x => x.Resize(1280, 720));

            //        // 画像を加工
            //        backgroundimage.Mutate(ctx =>
            //        {
            //            //ctx.DrawImage(profileImage,)
            //            // 背景にアイコンを貼り付け
            //            ctx.DrawImage(iconImage, new Point(iconX, iconY), 1.0f);

            //            // 名前をアイコン下に配置
            //            ctx.DrawText(discordname, font, Color.White, new PointF(margin, picmargin + margin / 2));
            //            ctx.DrawText("Welcome", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 2));
            //        });

            //        // 画像を保存
            //        backgroundimage.Save(outputPath);
            //        Console.WriteLine($"画像を保存しました: {outputPath}");
            //    }


            //    if (fileUpload == true)
            //    {
            //        await FollowupWithFileAsync(outputPath, text: "生成が完了しました。");
            //    }
            //    else
            //    {
            //        await FollowupAsync("GeneratedServer...", ephemeral: true);
            //    }

            //    //

            //    //await RespondAsync("実装されていないためしばらくお待ち下さい", ephemeral: true);
            //    //throw new NotImplementedException();
            //}

            /// <summary>
            /// /// プロフィールカードを生成します。
            /// </summary>
            /// <param name="comment"></param>
            /// <returns></returns>
            [SlashCommand("generate", "プロフィールカードを出力します。コメントをオプションで追加できます。")]
            public async Task Generate([Summary("Coment", "自由にコメントをいれることができます。")] string? comment = null)
            {
                
                string background = @".\Assets\Image\default_bg.png";
                //string discordicon = Context.User.GetAvatarUrl(Discord.ImageFormat.Png).ToString();
                string fontPath = @".\Assets\fonts\BizinGothic-Regular.ttf"; // フォントファイルを指定
                string outputPath = @$".\Tests\namecard\{Context.User.Id}.png";
                string discordname = Context.User.Username;
                string profileImageFlame = @".\Assets\Image\sampleflame.png";

                string avatarUrl = (Context.User as IGuildUser)?.GetGuildAvatarUrl(ImageFormat.Auto)
                ?? Context.User.GetAvatarUrl(ImageFormat.Auto)
                ?? Context.User.GetDefaultAvatarUrl();
                
                string avatarPath = $@".\Temp\Discordicon\128\{Context.User.Id}.png";

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
                    await DeferAsync(ephemeral: true);
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
                            ctx.DrawText(discordname, font, Color.White, new PointF(margin, picmargin + margin / 2));
                            ctx.DrawText($"{Context.Guild.Name}", font, Color.Cyan, new PointF(margin + 50, iconsize + 48 + margin * 2));
                            if (comment != null)
                            {
                                ctx.DrawText(comment, commentfont, Color.Black, new PointF(margin, backgroundimage.Height - (margin + commentfont.Size)));
                            }

                        });

                        // 画像を保存
                        backgroundimage.Save(outputPath);
                        Console.WriteLine($"画像を保存しました: {outputPath}");
                    }

                    if (fileUpload == true)
                    {
                        await FollowupWithFileAsync(outputPath, text: "生成が完了しました。");
                    }
                    else
                    {
                        await FollowupAsync("GeneratedServer...", ephemeral: true);
                    }
                }
                catch (Exception ex)
                {
                    //await DeleteOriginalResponseAsync();
                    await FollowupAsync($"生成エラー：\n" + $"```cmd\n{ex}\n```",ephemeral: true);
                }
            }

            /// <summary>
            /// プロフィールの背景をデフォルトの画像に変更します。
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            [SlashCommand("default-bg", "背景をデフォルトの画像へ変更します。")]
            public async Task DefaultBg() 
            {
                await FollowupAsync("実装までお待ち下さい");
                throw new NotImplementedException();
            }

            /// <summary>
            /// プロフィールの背景を指定したURLの画像に変更します。
            /// </summary>
            /// <param name="BackgraundUrl"></param>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            [SlashCommand("set-bg", "背景をコマンド経由で設定します。")]
            public async Task SetBg([Summary("BackgraundUrl", "Urlで背景画像を設定することができます。")] string BackgraundUrl)
            {

                await FollowupAsync("実装までお待ち下さい。");
                throw new NotImplementedException();
            }

            /// <summary>
            /// /// プロフィールの詳細な編集がWebから行えます。
            /// </summary>
            /// <returns></returns>
            /// <exception cref="NotImplementedException"></exception>
            [SlashCommand("edit-web", "Webからプロフィールの詳細な編集ができます。")]
            public async Task EditWeb()
            {
                await FollowupAsync("実装までお待ち下さい。");
                throw new NotImplementedException();
            }
        }
    }
}

/*
 
[SlashCommand("example", "hotcocoa")]
[Summary("mino")]
    public async Task exsample() 
    {
        await FollowupAsync("実装までお待ち下さい。");
        throw new NotImplementedException();
    }
 
 
 */
