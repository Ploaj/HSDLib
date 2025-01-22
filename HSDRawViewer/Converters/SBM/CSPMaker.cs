using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;

namespace HSDRawViewer.Converters.SBM
{
    public class CSPMaker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        private static byte Blend(byte c1, byte c2, float alpha)
        {
            var col = c1 * alpha + c2 * (1 - alpha);

            if (col > byte.MaxValue)
                col = byte.MaxValue;

            return (byte)col;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public static void MakeCSP(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;

            // Create a shadow image to store shadow data
            Image<Rgba32> shadow = image.Clone();

            int sx = 10; // Shadow offset X
            int sy = 10; // Shadow offset Y

            // Apply Outline
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rgba32 pixel = image[x, y];
                    float alpha = pixel.A / 255f;

                    // Calculate shadow position
                    int shadowX = x - sx;
                    int shadowY = y + sy;

                    if (shadowX >= 0 && shadowX < width && shadowY >= 0 && shadowY < height)
                    {
                        shadow[shadowX, shadowY] = new Rgba32(
                            0, 0, 0, (byte)(109 * alpha) // Apply shadow opacity
                        );
                    }

                    // Blend pixel colors
                    image[x, y] = new Rgba32(
                        Blend(pixel.R, 0, alpha),
                        Blend(pixel.G, 0, alpha),
                        Blend(pixel.B, 0, alpha),
                        pixel.A // Retain original alpha
                    );
                }
            }

            // Apply Shadow
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rgba32 pixel = image[x, y];
                    Rgba32 shadowPixel = shadow[x, y];

                    float shadowAlpha = shadowPixel.A / 255f;
                    float alpha = pixel.A / 255f;

                    // Blend shadow and image colors
                    image[x, y] = new Rgba32(
                        Blend(pixel.R, shadowPixel.R, alpha),
                        Blend(pixel.G, shadowPixel.G, alpha),
                        Blend(pixel.B, shadowPixel.B, alpha),
                        (byte)Math.Min(255, pixel.A + shadowPixel.A) // Combine alpha channels
                    );
                }
            }
        }
    }
}
