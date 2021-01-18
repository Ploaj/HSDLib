using System;

namespace HSDRaw.Tools.Textures
{
    public class CMPREncode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static byte[] ConvertTo(byte[] data, int width, int height)
        {
            byte[] result, block, blockResult;

            var BlockHeight = 8;
            var BlockWidth = 8;
            var BlockStride = 32;

            result = new byte[RoundWidth(width) / BlockWidth * RoundHeight(height) / BlockHeight * BlockStride];
            block = new byte[BlockWidth * BlockHeight << 2];

            for (int y = 0, i = 0; y < height; y += BlockHeight)
            {
                for (int x = 0; x < width; x += BlockWidth, i++)
                {
                    Array.Clear(block, 0, block.Length);

                    for (int dy = 0; dy < Math.Min(BlockHeight, height - y); dy++)
                        Array.Copy(data, ((y + dy) * width + x) << 2, block, dy * BlockWidth << 2, Math.Min(BlockWidth, width - x) << 2);

                    blockResult = ConvertBlockToCmpr(block);
                    blockResult.CopyTo(result, i * BlockStride);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToCmpr(byte[] block)
        {
            byte[] subBlock;
            byte[] result;

            result = new byte[32];
            subBlock = new byte[64];

            for (int i = 0, x = 0, y = 0; i < block.Length / 64; i++)
            {
                Array.Copy(block, x + y + 0, subBlock, 0, 16);
                Array.Copy(block, x + y + 32, subBlock, 16, 16);
                Array.Copy(block, x + y + 64, subBlock, 32, 16);
                Array.Copy(block, x + y + 96, subBlock, 48, 16);

                x = 16 - x;
                if (x == 0)
                    y = 128;

                ConvertBlockToQuaterCmpr(subBlock).CopyTo(result, i << 3);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToQuaterCmpr(byte[] block)
        {
            /*BC1BlockEncoder encode = new BC1BlockEncoder();

            float[] rValues = new float[16];
            float[] gValues = new float[16];
            float[] bValues = new float[16];

            for(int i = 0; i < block.Length; i+=4)
            {
                bValues[i / 4] = block[i + 0] / 255f;
                gValues[i / 4] = block[i + 1] / 255f;
                rValues[i / 4] = block[i + 2] / 255f;
            }

            encode.LoadBlock(rValues, gValues, bValues);
            //encode.DitherRgb = true;

            //encode.LoadAlphaMask();

            var encoded = BitConverter.GetBytes(encode.Encode().PackedValue);

            var swap = encoded[0];
            encoded[0] = encoded[1];
            encoded[1] = swap;

            swap = encoded[2];
            encoded[2] = encoded[3];
            encoded[3] = swap;

            for (int i = 0; i < 4; i++)
                encoded[4 + i] = SwapBits(encoded[4 + i]);

            return encoded;*/

            int col1, col2, dist, temp;
            bool alpha;
            byte[][] palette;
            byte[] result;

            dist = col1 = col2 = -1;
            alpha = false;
            result = new byte[8];

            for (int i = 0; i < 15; i++)
            {
                if (block[i * 4 + 3] < 16)
                    alpha = true;
                else
                {
                    for (int j = i + 1; j < 16; j++)
                    {
                        temp = Distance(block, i * 4, block, j * 4);

                        if (temp > dist)
                        {
                            dist = temp;
                            col1 = i;
                            col2 = j;
                        }
                    }
                }
            }

            if (dist == -1)
            {
                palette = new byte[][] { new byte[] { 0, 0, 0, 0xff }, new byte[] { 0xff, 0xff, 0xff, 0xff }, null, null };
            }
            else
            {
                palette = new byte[4][];
                palette[0] = new byte[4];
                palette[1] = new byte[4];

                Array.Copy(block, col1 * 4, palette[0], 0, 3);
                palette[0][3] = 0xff;
                Array.Copy(block, col2 * 4, palette[1], 0, 3);
                palette[1][3] = 0xff;

                if (palette[0][0] >> 3 == palette[1][0] >> 3 && palette[0][1] >> 2 == palette[1][1] >> 2 && palette[0][2] >> 3 == palette[1][2] >> 3)
                    if (palette[0][0] >> 3 == 0 && palette[0][1] >> 2 == 0 && palette[0][2] >> 3 == 0)
                        palette[1][0] = palette[1][1] = palette[1][2] = 0xff;
                    else
                        palette[1][0] = palette[1][1] = palette[1][2] = 0x0;
            }

            result[0] = (byte)(palette[0][2] & 0xf8 | palette[0][1] >> 5);
            result[1] = (byte)(palette[0][1] << 3 & 0xe0 | palette[0][0] >> 3);
            result[2] = (byte)(palette[1][2] & 0xf8 | palette[1][1] >> 5);
            result[3] = (byte)(palette[1][1] << 3 & 0xe0 | palette[1][0] >> 3);

            if ((result[0] > result[2] || (result[0] == result[2] && result[1] >= result[3])) == alpha)
            {
                Array.Copy(result, 0, result, 4, 2);
                Array.Copy(result, 2, result, 0, 2);
                Array.Copy(result, 4, result, 2, 2);

                palette[2] = palette[0];
                palette[0] = palette[1];
                palette[1] = palette[2];
            }

            if (!alpha)
            {
                palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
            }
            else
            {
                palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                palette[3] = new byte[] { 0, 0, 0, 0 };
            }

            for (int i = 0; i < block.Length >> 4; i++)
            {
                result[4 + i] = (byte)(LeastDistance(palette, block, i * 16 + 0) << 6 | LeastDistance(palette, block, i * 16 + 4) << 4 | LeastDistance(palette, block, i * 16 + 8) << 2 | LeastDistance(palette, block, i * 16 + 12));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte SwapBits(byte b)
        {
            byte Y = 0;
            for (int i = 0; i < 8; i += 2)
            {
                Y |= (byte)(((b >> (8 - i - 2)) & 0x3) << i);
            }
            return Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="palette"></param>
        /// <param name="colour"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static int LeastDistance(byte[][] palette, byte[] colour, int offset)
        {
            int dist, best, temp;

            if (colour[offset + 3] < 8)
                return 3;

            dist = int.MaxValue;
            best = 0;

            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i][3] != 0xff)
                    break;

                temp = Distance(palette[i], 0, colour, offset);

                if (temp < dist)
                {
                    if (temp == 0)
                        return i;

                    dist = temp;
                    best = i;
                }
            }

            return best;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colour1"></param>
        /// <param name="offset1"></param>
        /// <param name="colour2"></param>
        /// <param name="offset2"></param>
        /// <returns></returns>
        private static int Distance(byte[] colour1, int offset1, byte[] colour2, int offset2)
        {
            int temp, val;

            temp = 0;

            for (int i = 0; i < 3; i++)
            {
                val = colour1[offset1 + i] - colour2[offset2 + i];
                temp += val * val;
            }

            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int RoundWidth(int width)
        {
            return width + ((8 - (width % 8)) % 8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public static int RoundHeight(int height)
        {
            return height + ((8 - (height % 8)) % 8);
        }
    }
}
