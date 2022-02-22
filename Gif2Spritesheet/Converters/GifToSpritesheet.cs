using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gif2Spritesheet.Converters
{
    public static class GifToSpritesheet
    {
        public static async Task<Image> Convert(Image gif)
        {
            var size = gif.Size();
            var nbFrames = gif.Frames.Count;

            var resultWidth = size.Width * nbFrames;
            var resultHeight = size.Height;

            var result = new Image<Rgba32>(resultWidth, resultHeight);
            
            int xOffset = 0;
            
            foreach (var frame in gif.Frames.Cast<ImageFrame<Rgba32>>())
            {
                await Task.Run(() => WriteFrameToImage(frame, result, xOffset));
                xOffset += frame.Width;
            }

            return result;
        }

        // Src : https://khalidabuhakmeh.com/gifs-in-console-output-imagesharp-and-spectreconsole
        private static async Task<byte[]> GetBytesFromFrameAsync(ImageFrame<Rgba32> imageFrame, CancellationTokenSource cancellationTokenSource = null)
        {
            using var image = new Image<Rgba32>(imageFrame.Width, imageFrame.Height);
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    image[x, y] = imageFrame[x, y];
                }
            }

            await using var memoryStream = new MemoryStream();

            if (cancellationTokenSource != null)
                await image.SaveAsBmpAsync(memoryStream, cancellationTokenSource.Token);
            else
                await image.SaveAsBmpAsync(memoryStream);

            return memoryStream.ToArray();
        }

        private static void WriteFrameToImage(ImageFrame<Rgba32> imageFrame, Image<Rgba32> result, int xOffset = 0, int yOffset = 0)
        {
            Parallel.For(0, imageFrame.Height, (y, state) =>
            {
                Parallel.For(0, imageFrame.Width, (x, stateCol) =>
                {
                    result[x + xOffset, y + yOffset] = imageFrame[x, y];
                });
            });
        }
    }
}
