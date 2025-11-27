using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using Newtonsoft.Json;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services
{
    public class TemplateConfig
    {
        public CanvasConfig Canvas { get; set; }
        public List<ElementConfig> Elements { get; set; }
    }

    public class CanvasConfig
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundColor { get; set; }
    }

    public class ElementConfig
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public string Color { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class NameCardRenderer
    {
        private readonly string _templatePath;
        private readonly string _customDir;
        private readonly string _outputDir;

        public NameCardRenderer(string templatePath, string? customDir = null)
        {
            _templatePath = templatePath;
            _customDir = customDir ?? AppDomain.CurrentDomain.BaseDirectory;
            _outputDir = Path.Combine(_customDir, "namecard");

            // 出力ディレクトリが存在しない場合は作成
            if (!Directory.Exists(_outputDir))
            {
                Directory.CreateDirectory(_outputDir);
            }
        }

        /// <summary>
        /// ネームカードを生成してファイルパスを返す
        /// </summary>
        /// <param name="userId">DiscordユーザーID</param>
        /// <param name="data">テンプレートに埋め込むデータ（username, avatarPath など）</param>
        /// <returns>生成されたPNGファイルのフルパス</returns>
        public async Task<string> GenerateNameCardAsync(ulong userId, Dictionary<string, string> data)
        {
            // テンプレート読み込み
            var templateJson = await File.ReadAllTextAsync(_templatePath);
            var template = JsonConvert.DeserializeObject<TemplateConfig>(templateJson);

            // 画像生成
            var bgColor = ParseColor(template.Canvas.BackgroundColor);
            using var image = new Image<Rgba32>(template.Canvas.Width, template.Canvas.Height, bgColor);

            // 要素をID順にソートして描画
            var sortedElements = template.Elements.OrderBy(e => e.Id).ToList();

            foreach (var element in sortedElements)
            {
                await RenderElementAsync(image, element, data);
            }

            // ファイル保存
            var outputPath = Path.Combine(_outputDir, $"{userId}.png");
            await image.SaveAsPngAsync(outputPath);

            return outputPath;
        }

        /// <summary>
        /// 個別要素をレンダリング
        /// </summary>
        private async Task RenderElementAsync(Image<Rgba32> image, ElementConfig element, Dictionary<string, string> data)
        {
            switch (element.Type.ToLower())
            {
                case "backgrounds":
                    await RenderBackgroundAsync(image, element, data);
                    break;
                case "label":
                    RenderLabel(image, element, data);
                    break;
                case "icon":
                    await RenderIconAsync(image, element, data);
                    break;
            }
        }

        /// <summary>
        /// 背景画像を描画
        /// </summary>
        private async Task RenderBackgroundAsync(Image<Rgba32> image, ElementConfig element, Dictionary<string, string> data)
        {
            var path = ReplacePlaceholders(element.Path, data);
            var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_customDir, path);

            if (File.Exists(fullPath))
            {
                using var bgImage = await Image.LoadAsync<Rgba32>(fullPath);

                // キャンバスサイズに合わせてリサイズ
                bgImage.Mutate(ctx => ctx.Resize(image.Width, image.Height));

                image.Mutate(ctx => ctx.DrawImage(bgImage, new Point(0, 0), 1f));
            }
        }

        /// <summary>
        /// テキストラベルを描画
        /// </summary>
        private void RenderLabel(Image<Rgba32> image, ElementConfig element, Dictionary<string, string> data)
        {
            var text = ReplacePlaceholders(element.Text, data);
            var color = ParseColor(element.Color);

            // フォント設定
            var fontCollection = new FontCollection();
            FontFamily fontFamily;

            try
            {
                fontFamily = fontCollection.Families.FirstOrDefault(f =>
                    f.Name.Equals(element.FontFamily, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                // システムフォントが取得できない場合はデフォルトを使用
                fontFamily = SystemFonts.Families.First();
            }

            var font = fontFamily.CreateFont(element.FontSize, FontStyle.Regular);

            image.Mutate(ctx => ctx.DrawText(
                text,
                font,
                color,
                new PointF(element.X, element.Y)
            ));
        }

        /// <summary>
        /// アイコン画像を描画
        /// </summary>
        private async Task RenderIconAsync(Image<Rgba32> image, ElementConfig element, Dictionary<string, string> data)
        {
            var path = ReplacePlaceholders(element.Path, data);

            // URLの場合はダウンロード、それ以外はローカルファイルとして扱う
            Image<Rgba32> iconImage;

            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                using var httpClient = new System.Net.Http.HttpClient();
                var imageData = await httpClient.GetByteArrayAsync(path);
                iconImage = Image.Load<Rgba32>(imageData);
            }
            else
            {
                var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_customDir, path);

                if (!File.Exists(fullPath))
                    return;

                iconImage = await Image.LoadAsync<Rgba32>(fullPath);
            }

            using (iconImage)
            {
                // リサイズ
                iconImage.Mutate(ctx => ctx.Resize(element.Width, element.Height));

                image.Mutate(ctx => ctx.DrawImage(iconImage, new Point(element.X, element.Y), 1f));
            }
        }

        /// <summary>
        /// プレースホルダーを実際の値で置換
        /// </summary>
        private string ReplacePlaceholders(string input, Dictionary<string, string> data)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(input, @"\{\{(\w+)\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return data.ContainsKey(key) ? data[key] : match.Value;
            });
        }

        /// <summary>
        /// HEXカラーコードをImageSharpのColorオブジェクトに変換
        /// </summary>
        private Color ParseColor(string colorCode)
        {
            colorCode = colorCode.TrimStart('#');

            if (colorCode.Length == 6)
            {
                var r = Convert.ToByte(colorCode.Substring(0, 2), 16);
                var g = Convert.ToByte(colorCode.Substring(2, 2), 16);
                var b = Convert.ToByte(colorCode.Substring(4, 2), 16);
                return Color.FromRgb(r, g, b);
            }

            return Color.White;
        }
    }

    // 使用例
    public class Example
    {
        public async Task SendNameCardToDiscord()
        {
            // レンダラー初期化
            var renderer = new NameCardRenderer("template.json");
            // カスタムディレクトリを指定する場合
            // var renderer = new NameCardRenderer("template.json", "/path/to/custom/dir");

            // Discord.Netから取得したユーザー情報
            ulong userId = 123456789012345678;
            string username = "山田太郎";
            string avatarUrl = "https://cdn.discordapp.com/avatars/123456789012345678/abc123.png";

            // ネームカード生成
            var filePath = await renderer.GenerateNameCardAsync(userId, new Dictionary<string, string>
            {
                { "username", username },
                { "avatarPath", avatarUrl }
            });

            // Discord.Netでファイル送信
            // await channel.SendFileAsync(filePath, "namecard.png");

            Console.WriteLine($"生成されたファイル: {filePath}");
        }
    }
}
