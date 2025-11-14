using Flurl.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;


namespace TestCaptcha
{
    public class Response
    {
        public required List<ResponseImage> Images { get; set; }
    }

    public class ResponseImage {
        public required Representations Representations { get; set; }
    }

    public class Representations
    {
        public required string Medium { get; set; }
    }

    internal class FurryAPI
    {
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri("https://furbooru.org"),
        };

        public static async Task<Response?> GetImages(string query, int page)
        {
            var uri = new Flurl.Url("/api/v1/json/search/images").SetQueryParams(new
            {
                q = query,
                page,
            }).ToUri();
            return await client.GetFromJsonAsync<Response>(uri, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        }


        public static async Task<bool> GetRandomImagesAsync(PictureBox[] boxes)
        {
            try
            {
                Random random = new();
                Response images = await FurryAPI.GetImages("upvotes.gte:10", random.Next(20)); //"safe, male, dog, -hazbin hotel, -breasts, -transformation, -pride flag, -intersex, -bare butt, -femboy, -bikini, -swimsuit, upvotes.gte:10"
                string url = images.Images[random.Next(images.Images.Count)].Representations.Medium;
                using var image = SixLabors.ImageSharp.Image.Load(await url.GetStreamAsync());
                SixLabors.ImageSharp.Size size = new() { Width = 512, Height = 512 };
                var tile = size / 2;
                image.Mutate(x => x.Resize(size));
                var t1 = image.Clone(x => x.Crop(new() { Width = tile.Width, Height = tile.Height, X = 0, Y = 0 }));
                var t2 = image.Clone(x => x.Crop(new() { Width = tile.Width, Height = tile.Height, X = tile.Width, Y = 0 }));
                var t3 = image.Clone(x => x.Crop(new() { Width = tile.Width, Height = tile.Height, X = 0, Y = tile.Height }));
                var t4 = image.Clone(x => x.Crop(new() { Width = tile.Width, Height = tile.Height, X = tile.Width, Y = tile.Height }));
                using MemoryStream ms = new();

                t1.SaveAsBmp(ms);
                boxes[0].Image = Image.FromStream(ms);
                ms.Seek(0, SeekOrigin.Begin);

                t2.SaveAsBmp(ms);
                boxes[1].Image = Image.FromStream(ms);
                ms.Seek(0, SeekOrigin.Begin);

                t3.SaveAsBmp(ms);
                boxes[2].Image = Image.FromStream(ms);
                ms.Seek(0, SeekOrigin.Begin);

                t4.SaveAsBmp(ms);
                boxes[3].Image = Image.FromStream(ms);

                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }

}
