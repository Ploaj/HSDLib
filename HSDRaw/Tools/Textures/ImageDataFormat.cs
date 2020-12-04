// CTools library - Library functions for CTools
// Copyright (C) 2010 Chadderz

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using BCn;
using HSDRaw;
using HSDRaw.GX;
using SimplePaletteQuantizer.Quantizers.XiaolinWu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Chadsoft.CTools.Image
{
    public sealed class ImageDataFormat
    {
        private static ImageDataFormat _i4, _i8, _ia4, _ia8, _rgb565, _rgb5a3, _rgba32, _c4, _c8, _c14x2, _cmpr;

        public static ImageDataFormat I4 { get { if (_i4 == null) _i4 = new ImageDataFormat("I4", "I4", 4, 0, 8, 8, 32, false, false, false, false, 0, 0, ConvertBlockToI4, ConvertBlockFromI4); return _i4; } }
        public static ImageDataFormat I8 { get { if (_i8 == null) _i8 = new ImageDataFormat("I8", "I8", 8, 0, 8, 4, 32, false, false, false, false, 0, 0, ConvertBlockToI8, ConvertBlockFromI8); return _i8; } }
        public static ImageDataFormat IA4 { get { if (_ia4 == null) _ia4 = new ImageDataFormat("IA4", "IA4", 8, 4, 8, 4, 32, false, false, false, false, 0, 0, ConvertBlockToIa4, ConvertBlockFromIa4); return _ia4; } }
        public static ImageDataFormat IA8 { get { if (_ia8 == null) _ia8 = new ImageDataFormat("IA8", "IA8", 16, 8, 4, 4, 32, false, false, false, false, 0, 0, ConvertBlockToIa8, ConvertBlockFromIa8); return _ia8; } }
        public static ImageDataFormat RGB565 { get { if (_rgb565 == null) _rgb565 = new ImageDataFormat("RGB565", "RGB565", 16, 0, 4, 4, 32, true, false, false, false, 0, 0, ConvertBlockToRgb565, ConvertBlockFromRgb565); return _rgb565; } }
        public static ImageDataFormat RGB5A3 { get { if (_rgb5a3 == null) _rgb5a3 = new ImageDataFormat("RGB5A3", "RGB5A3", 16, 3, 4, 4, 32, true, false, false, false, 0, 0, ConvertBlockToRgb5a3, ConvertBlockFromRgb5a3); return _rgb5a3; } }
        public static ImageDataFormat Rgba32 { get { if (_rgba32 == null) _rgba32 = new ImageDataFormat("RGBA32", "RGBA32", 32, 8, 4, 4, 64, true, false, false, false, 0, 0, ConvertBlockToRgba32, ConvertBlockFromRgba32); return _rgba32; } }
        public static ImageDataFormat Cmpr { get { if (_cmpr == null) _cmpr = new ImageDataFormat("CMPR", "CMPR", 4, 1, 8, 8, 32, true, true, true, false, 0, 0, ConvertBlockToCmpr, ConvertBlockFromCmpr); return _cmpr; } }

        private static Dictionary<GXTexFmt, ImageDataFormat> typeToConverter = new Dictionary<GXTexFmt, ImageDataFormat>()
        {
            { GXTexFmt.I4, I4 },
            { GXTexFmt.I8, I8 },
            { GXTexFmt.IA4, IA4 },
            { GXTexFmt.IA8, IA8 },
            { GXTexFmt.RGB565, RGB565 },
            { GXTexFmt.RGB5A3, RGB5A3 },
            { GXTexFmt.RGBA8, Rgba32 },
            { GXTexFmt.CMP, Cmpr },
        };

        public string Name { get; private set; }
        public string Description { get; private set; }
        public int BitsPerPixel { get; private set; }
        public int AlphaDepth { get; private set; }
        public int BlockWidth { get; private set; }
        public int BlockHeight { get; private set; }
        public int BlockStride { get; private set; }
        public bool HasColour { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool LossyCompression { get; private set; }
        public bool Palette { get; private set; }
        public int PaletteSize { get; private set; }
        public int PaletteBitsPerEntry { get; private set; }

        private ConvertBlockDelegate _convertTo, _convertFrom;

        public ImageDataFormat(string name, string description, int bitsPerPixel, int alphaDepth, int blockWidth, int blockHeight, int blockStride, bool hasColour, bool isCompressed, bool lossyCompression, bool palette, int paletteSize, int paletteBitsPerEntry, ConvertBlockDelegate convertTo, ConvertBlockDelegate convertFrom)
        {
            Name = name;
            Description = description;
            BitsPerPixel = bitsPerPixel;
            AlphaDepth = alphaDepth;
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
            BlockStride = blockStride;
            HasColour = hasColour;
            IsCompressed = isCompressed;
            Palette = palette;
            PaletteSize = paletteSize;
            PaletteBitsPerEntry = paletteBitsPerEntry;
            _convertTo = convertTo;
            _convertFrom = convertFrom;
        }


        public static bool IsPalettedFormat(GXTexFmt format)
        {
            if (format == GXTexFmt.CI4
                || format == GXTexFmt.CI8
                || format == GXTexFmt.CI14X2)
                return true;

            return false;
        }

        public static int GetImageSize(GXTexFmt format, int width, int height)
        {
            if (width % 4 != 0)
                width += 4 - (width % 4);

            //if (height % 4 != 0)
            //    height += 4 - (height % 4);

            int size = width * height;
            switch (format)
            {
                case GXTexFmt.CI4:
                case GXTexFmt.I4:
                    return size / 2;
                case GXTexFmt.IA4:
                case GXTexFmt.I8:
                case GXTexFmt.CI14X2:
                case GXTexFmt.CMP:
                case GXTexFmt.CI8:
                    return size;
                case GXTexFmt.IA8:
                case GXTexFmt.RGB565:
                case GXTexFmt.RGB5A3:
                    return size * 2;
                case GXTexFmt.RGBA8:
                    return size * 4;
                default:
                    return size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        /// <returns></returns>
        public static byte[] Encode(byte[] rgba, int width, int height, GXTexFmt imgFormat, GXTlutFmt palFormat, out byte[] palette)
        {
            palette = null;

            if (typeToConverter.ContainsKey(imgFormat))
                return typeToConverter[imgFormat].ConvertTo(rgba, width, height, null);

            switch(imgFormat)
            {
                case GXTexFmt.CI4:
                    {
                        var encode = EncodeC4(rgba, width, height, palFormat);

                        palette = new byte[encode.Item2.Length * sizeof(short)];
                        Buffer.BlockCopy(encode.Item2, 0, palette, 0, palette.Length);

                        return encode.Item1;
                    }
                case GXTexFmt.CI8:
                    {
                        var encode = EncodeC8(rgba, width, height, palFormat);

                        palette = new byte[encode.Item2.Length * sizeof(short)];
                        Buffer.BlockCopy(encode.Item2, 0, palette, 0, palette.Length);

                        return encode.Item1;
                    }
                case GXTexFmt.CI14X2:
                    {
                        // TODO:
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        /// <returns></returns>
        public static byte[] Decode(byte[] data, int width, int height, GXTexFmt imgFormat)
        {
            return Decode(data, null, width, height, imgFormat, GXTlutFmt.IA8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imgFormat"></param>
        /// <param name="palFormat"></param>
        /// <returns></returns>
        public static byte[] Decode(byte[] data, byte[] palData, int width, int height, GXTexFmt imgFormat, GXTlutFmt palFormat)
        {
            if (typeToConverter.ContainsKey(imgFormat))
                return typeToConverter[imgFormat].ConvertFrom(data, width, height, null);

            switch (imgFormat)
            {
                case GXTexFmt.CI4:
                    return DecodeC4(data, (uint)width, (uint)height, palData, palFormat);
                case GXTexFmt.CI8:
                    return DecodeC8(data, (uint)width, (uint)height, palData, palFormat);
                case GXTexFmt.CI14X2:
                    // TODO:
                    break;
            }
            
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public int RoundWidth(int width)
        {
            return width + ((BlockWidth - (width % BlockWidth)) % BlockWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public int RoundHeight(int height)
        {
            return height + ((BlockHeight - (height % BlockHeight)) % BlockHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public byte[] ConvertFrom(byte[] data, int width, int height, ProgressChangedEventHandler progress)
        {
            byte[] result, blockResult, block;
            int step;

            step = Math.Max(width * height / BlockHeight / BlockWidth / 100, 1024);
            result = new byte[width * height << 2];
            block = new byte[BlockStride];

            for (int y = 0, i = 0; y < height; y += BlockHeight)
            {
                for (int x = 0; x < width; x += BlockWidth, i++)
                {
                    Array.Copy(data, i * block.Length, block, 0, block.Length);
                    blockResult = _convertFrom(block);

                    for (int dy = 0; dy < Math.Min(BlockHeight, height - y); dy++)
                    {
                        Array.Copy(blockResult, dy * BlockWidth << 2, result, ((dy + y) * width + x) << 2, Math.Min(BlockWidth, width - x) << 2);
                    }

                    if (i % step == 0 && progress != null)
                        progress(this, new ProgressChangedEventArgs((x + y * width * 100) / (result.Length / 4), null));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public byte[] ConvertTo(byte[] data, int width, int height, ProgressChangedEventHandler progress)
        {
            byte[] result, block, blockResult;
            int step;

            step = Math.Max(width * height / BlockHeight / BlockWidth / 100, 2048);
            result = new byte[RoundWidth(width) / BlockWidth * RoundHeight(height) / BlockHeight * BlockStride];
            block = new byte[BlockWidth * BlockHeight << 2];

            for (int y = 0, i = 0; y < height; y += BlockHeight)
            {
                for (int x = 0; x < width; x += BlockWidth, i++)
                {
                    Array.Clear(block, 0, block.Length);

                    for (int dy = 0; dy < Math.Min(BlockHeight, height - y); dy++)
                    {
                        Array.Copy(data, ((y + dy) * width + x) << 2, block, dy * BlockWidth << 2, Math.Min(BlockWidth, width - x) << 2);
                    }

                    blockResult = _convertTo(block);
                    blockResult.CopyTo(result, i * BlockStride);

                    //if (i % step == 0 && progress != null)
                    //progress(this, new ProgressChangedEventArgs((x + y * width * 100) / (width * height), null));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromI4(byte[] block)
        {
            byte[] result;

            result = new byte[256];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 8 + 0] = result[i * 8 + 1] = result[i * 8 + 2] = result[i * 8 + 3] = (byte)((block[i] >> 4) * 0x11);
                result[i * 8 + 4] = result[i * 8 + 5] = result[i * 8 + 6] = result[i * 8 + 7] = (byte)((block[i] & 0xF) * 0x11);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromI8(byte[] block)
        {
            byte[] result;

            result = new byte[128];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = result[i * 4 + 3] = block[i];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromIa4(byte[] block)
        {
            byte[] result;

            result = new byte[128];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = (byte)((block[i] & 0xF) * 0x11);
                result[i * 4 + 3] = (byte)((block[i] >> 4) * 0x11);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromIa8(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = block[i * 2 + 1];
                result[i * 4 + 3] = block[i * 2 + 0];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromRgb565(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 3 & 0xf8 | block[i * 2 + 1] >> 2 & 0x07);
                result[i * 4 + 1] = (byte)(block[i * 2 + 0] << 5 & 0xe0 | block[i * 2 + 1] >> 3 & 0x1c | block[i * 2 + 0] >> 1 & 0x03);
                result[i * 4 + 2] = (byte)(block[i * 2 + 0] & 0xf8 | block[i * 2 + 0] >> 5);
                result[i * 4 + 3] = 255;
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromRgb5a3(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                if ((block[i * 2 + 0] & 0x80) == 0)
                {
                    result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 4 & 0xf0 | block[i * 2 + 1] & 0xf);
                    result[i * 4 + 1] = (byte)(block[i * 2 + 1] & 0xf0 | block[i * 2 + 1] >> 4 & 0xf);
                    result[i * 4 + 2] = (byte)(block[i * 2 + 0] << 4 & 0xf0 | block[i * 2 + 0] & 0xf);
                    result[i * 4 + 3] = (byte)(block[i * 2 + 0] << 1 & 0xe0 | block[i * 2 + 0] >> 2 & 0x1C | block[i * 2 + 0] >> 5 & 0x03);
                }
                else
                {
                    result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 3 & 0xf8 | block[i * 2 + 1] >> 2 & 0x07);
                    result[i * 4 + 1] = (byte)(block[i * 2 + 0] << 6 & 0xc0 | block[i * 2 + 1] >> 2 & 0x38 | block[i * 2 + 0] & 0x06 | block[i * 2 + 1] >> 7 & 0x01);
                    result[i * 4 + 2] = (byte)(block[i * 2 + 0] << 1 & 0xf8 | block[i * 2 + 0] >> 4 & 0x07);
                    result[i * 4 + 3] = 0xff;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromRgba32(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 4; i++)
            {
                result[i * 4 + 0] = block[i * 2 + 33];
                result[i * 4 + 1] = block[i * 2 + 32];
                result[i * 4 + 2] = block[i * 2 + 1];
                result[i * 4 + 3] = block[i * 2 + 0];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromCmpr(byte[] block)
        {
            byte[] result;
            byte[][] results;

            result = new byte[256];
            results = new byte[4][];

            for (int i = 0, x = 0, y = 0; i < block.Length / 8; i++)
            {
                results[i] = ConvertBlockFromQuaterCmpr(block, i << 3);

                Array.Copy(results[i], 0, result, x + y + 0, 16);
                Array.Copy(results[i], 16, result, x + y + 32, 16);
                Array.Copy(results[i], 32, result, x + y + 64, 16);
                Array.Copy(results[i], 48, result, x + y + 96, 16);

                x = 16 - x;
                if (x == 0)
                    y = 128;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockFromQuaterCmpr(byte[] block, int offset)
        {
            byte[][] palette;
            byte[] result;

            result = new byte[64];
            palette = new byte[4][];

            palette[0] = new byte[] { (byte)(block[offset + 1] << 3 & 0xf8), (byte)(block[offset + 0] << 5 & 0xe0 | block[offset + 1] >> 3 & 0x1c), (byte)(block[offset + 0] & 0xf8), 0xff };
            palette[1] = new byte[] { (byte)(block[offset + 3] << 3 & 0xf8), (byte)(block[offset + 2] << 5 & 0xe0 | block[offset + 3] >> 3 & 0x1c), (byte)(block[offset + 2] & 0xf8), 0xff };

            if (block[offset + 0] > block[offset + 2] || (block[offset + 0] == block[offset + 2] && block[offset + 1] > block[offset + 3]))
            {
                palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
            }
            else
            {
                palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                palette[3] = new byte[] { 0, 0, 0, 0 };
            }

            for (int i = 0; i < 4; i++)
            {
                palette[block[offset + i + 4] >> 6].CopyTo(result, i * 16 + 0);
                palette[block[offset + i + 4] >> 4 & 0x3].CopyTo(result, i * 16 + 4);
                palette[block[offset + i + 4] >> 2 & 0x3].CopyTo(result, i * 16 + 8);
                palette[block[offset + i + 4] & 0x3].CopyTo(result, i * 16 + 12);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToI4(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(((block[i * 8 + 0] * 11 + block[i * 8 + 1] * 59 + block[i * 8 + 2] * 30) / 1700) << 4 | (block[i * 8 + 4] * 11 + block[i * 8 + 5] * 59 + block[i * 8 + 6] * 30) / 1700);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToI8(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 100);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToIa4(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 1700 | block[i * 4 + 3] / 0x11 << 4);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToIa8(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i * 2 + 1] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 100);
                result[i * 2 + 0] = block[i * 4 + 3];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToRgb565(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i * 2 + 0] = (byte)(block[i * 4 + 2] & 0xf8 | block[i * 4 + 1] >> 5);
                result[i * 2 + 1] = (byte)(block[i * 4 + 0] >> 3 | block[i * 4 + 1] << 3 & 0xe0);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToRgb5a3(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                if (block[i * 4 + 3] < 0xe0)
                {
                    result[i * 2 + 0] = (byte)(block[i * 4 + 3] >> 1 & 0x70 | block[i * 4 + 2] >> 4);
                    result[i * 2 + 1] = (byte)(block[i * 4 + 1] & 0xf0 | block[i * 4 + 0] >> 4);
                }
                else
                {
                    result[i * 2 + 0] = (byte)(0x80 | block[i * 4 + 2] >> 1 & 0x7c | block[i * 4 + 1] >> 6);
                    result[i * 2 + 1] = (byte)(block[i * 4 + 0] >> 3 | block[i * 4 + 1] << 2 & 0xe0);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static byte[] ConvertBlockToRgba32(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < result.Length / 4; i++)
            {
                result[i * 2 + 33] = block[i * 4 + 0];
                result[i * 2 + 32] = block[i * 4 + 1];
                result[i * 2 + 1] = block[i * 4 + 2];
                result[i * 2 + 0] = block[i * 4 + 3];
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
        /// <param name="stream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imagePalette"></param>
        /// <param name="paletteFormat"></param>
        /// <returns></returns>
        private static byte[] DecodeC4(byte[] imgData, uint width, uint height, byte[] imagePalette, GXTlutFmt paletteFormat)
        {
            //4 bpp, 8 block width/height, block size 32 bytes, possible palettes (IA8, RGB565, RGB5A3)
            uint numBlocksW = width / 8;
            uint numBlocksH = height / 8;

            int i = 0;

            byte[] decodedData = new byte[width * height * 8];

            //Read the indexes from the file
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //Inner Loop for pixels
                    for (int pY = 0; pY < 8; pY++)
                    {
                        for (int pX = 0; pX < 8; pX += 2)
                        {
                            //Ensure we're not reading past the end of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 8 + pY >= height))
                                continue;

                            byte data = imgData[i++];
                            byte t = (byte)(data & 0xF0);
                            byte t2 = (byte)(data & 0x0F);

                            decodedData[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 0] = (byte)(t >> 4);
                            decodedData[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 1] = t2;
                        }
                    }
                }
            }

            //Now look them up in the palette and turn them into actual colors.
            byte[] finalDest = new byte[decodedData.Length / 2];

            int pixelSize = paletteFormat == GXTlutFmt.IA8 ? 2 : 4;
            int destOffset = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UnpackPixelFromPalette(decodedData[y * width + x], ref finalDest, destOffset, imagePalette, paletteFormat);
                    destOffset += pixelSize;
                }
            }

            return finalDest;
        }

        private static byte[] DecodeC8(byte[] imgData, uint width, uint height, byte[] imagePalette, GXTlutFmt paletteFormat)
        {
            //4 bpp, 8 block width/4 block height, block size 32 bytes, possible palettes (IA8, RGB565, RGB5A3)
            uint numBlocksW = width / 8;
            uint numBlocksH = height / 4;

            int i = 0;

            byte[] decodedData = new byte[width * height * 8];

            //Read the indexes from the file
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //Inner Loop for pixels
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            //Ensure we're not reading past the end of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;
                            
                            byte data = imgData[i++];
                            decodedData[width * ((yBlock * 4) + pY) + (xBlock * 8) + pX] = data;
                        }
                    }
                }
            }

            //Now look them up in the palette and turn them into actual colors.
            byte[] finalDest = new byte[decodedData.Length / 2];

            int pixelSize = paletteFormat == GXTlutFmt.IA8 ? 2 : 4;
            int destOffset = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UnpackPixelFromPalette(decodedData[y * width + x], ref finalDest, destOffset, imagePalette, paletteFormat);
                    destOffset += pixelSize;
                }
            }

            return finalDest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paletteIndex"></param>
        /// <param name="dest"></param>
        /// <param name="offset"></param>
        /// <param name="paletteData"></param>
        /// <param name="format"></param>
        private static void UnpackPixelFromPalette(int paletteIndex, ref byte[] dest, int offset, byte[] paletteData, GXTlutFmt format)
        {
            switch (format)
            {
                case GXTlutFmt.IA8:
                    dest[0] = paletteData[2 * paletteIndex + 1];
                    dest[1] = paletteData[2 * paletteIndex + 0];
                    break;
                case GXTlutFmt.RGB565:
                    {
                        ushort palettePixelData = (ushort)((Buffer.GetByte(paletteData, 2 * paletteIndex) << 8) | Buffer.GetByte(paletteData, 2 * paletteIndex + 1));
                        RGB565ToRGBA8(palettePixelData, ref dest, offset);
                    }
                    break;
                case GXTlutFmt.RGB5A3:
                    {
                        ushort palettePixelData = (ushort)((Buffer.GetByte(paletteData, 2 * paletteIndex) << 8) | Buffer.GetByte(paletteData, 2 * paletteIndex + 1));
                        RGB5A3ToRGBA8(palettePixelData, ref dest, offset);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Convert a RGB565 encoded pixel (two bytes in length) to a RGBA (4 byte in length)
        /// pixel.
        /// </summary>
        /// <param name="sourcePixel">RGB565 encoded pixel.</param>
        /// <param name="dest">Destination array for RGBA pixel.</param>
        /// <param name="destOffset">Offset into destination array to write RGBA pixel.</param>
        private static void RGB565ToRGBA8(ushort sourcePixel, ref byte[] dest, int destOffset)
        {
            byte r, g, b;
            r = (byte)((sourcePixel & 0xF100) >> 11);
            g = (byte)((sourcePixel & 0x7E0) >> 5);
            b = (byte)((sourcePixel & 0x1F));

            r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));
            g = (byte)((g << (8 - 6)) | (g >> (12 - 8)));
            b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));

            dest[destOffset] = b;
            dest[destOffset + 1] = g;
            dest[destOffset + 2] = r;
            dest[destOffset + 3] = 0xFF; //Set alpha to 1
        }

        /// <summary>
        /// Convert a RGB5A3 encoded pixel (two bytes in length) to an RGBA (4 byte in length)
        /// pixel.
        /// </summary>
        /// <param name="sourcePixel">RGB5A3 encoded pixel.</param>
        /// <param name="dest">Destination array for RGBA pixel.</param>
        /// <param name="destOffset">Offset into destination array to write RGBA pixel.</param>
        private static void RGB5A3ToRGBA8(ushort sourcePixel, ref byte[] dest, int destOffset)
        {
            byte r, g, b, a;

            //No alpha bits
            if ((sourcePixel & 0x8000) == 0x8000)
            {
                a = 0xFF;
                r = (byte)((sourcePixel & 0x7C00) >> 10);
                g = (byte)((sourcePixel & 0x3E0) >> 5);
                b = (byte)(sourcePixel & 0x1F);

                r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));
                g = (byte)((g << (8 - 5)) | (g >> (10 - 8)));
                b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));
            }
            //Alpha bits
            else
            {
                a = (byte)((sourcePixel & 0x7000) >> 12);
                r = (byte)((sourcePixel & 0xF00) >> 8);
                g = (byte)((sourcePixel & 0xF0) >> 4);
                b = (byte)(sourcePixel & 0xF);

                a = (byte)((a << (8 - 3)) | (a << (8 - 6)) | (a >> (9 - 8)));
                r = (byte)((r << (8 - 4)) | r);
                g = (byte)((g << (8 - 4)) | g);
                b = (byte)((b << (8 - 4)) | b);
            }

            dest[destOffset + 0] = b;
            dest[destOffset + 1] = g;
            dest[destOffset + 2] = r;
            dest[destOffset + 3] = a;
        }

        /// <summary>
        /// 
        /// </summary>
        private class Color32
        {
            public byte R, G, B, A;

            public Color32(byte r, byte g, byte b, byte a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public override string ToString()
            {
                return string.Format("[Color32] (r: {0} g: {1} b: {2} a: {3})", R, G, B, A);
            }

            public byte this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return R;
                        case 1:
                            return G;
                        case 2:
                            return B;
                        case 3:
                            return A;

                        default:
                            throw new ArgumentOutOfRangeException("index");
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            R = value;
                            break;

                        case 1:
                            G = value;
                            break;

                        case 2:
                            B = value;
                            break;

                        case 3:
                            A = value;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("index");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Tuple<byte[], ushort[]> EncodeC4(byte[] rgba, int width, int height, GXTlutFmt paletteFormat)
        {
            List<Color32> palColors = new List<Color32>();

            uint numBlocksW = (uint)width / 8;
            uint numBlocksH = (uint)height / 8;

            byte[] pixIndices = new byte[numBlocksH * numBlocksW * 8 * 8];

            for (int i = 0; i < (width * height) * 4; i += 4)
                palColors.Add(new Color32(rgba[i + 2], rgba[i + 1], rgba[i + 0], rgba[i + 3]));

            SortedList<ushort, Color32> rawColorData = new SortedList<ushort, Color32>();
            foreach (Color32 col in palColors)
            {
                EncodeColor(paletteFormat, col, rawColorData);
            }

            int pixIndex = 0;
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    for (int pY = 0; pY < 8; pY++)
                    {
                        for (int pX = 0; pX < 8; pX += 2)
                        {
                            pixIndices[pixIndex] = (byte)(rawColorData.IndexOfValue(palColors[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX]) << 4);
                            pixIndices[pixIndex++] |= (byte)(rawColorData.IndexOfValue(palColors[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 1]));
                        }
                    }
                }
            }

            return new Tuple<byte[], ushort[]>(pixIndices, rawColorData.Keys.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Tuple<byte[], ushort[]> EncodeC8(byte[] rgba, int width, int height, GXTlutFmt paletteFormat)
        {
            List<Color32> palColors = new List<Color32>();

            uint numBlocksW = (uint)width / 8;
            uint numBlocksH = (uint)height / 4;

            byte[] pixIndices = new byte[numBlocksH * numBlocksW * 8 * 4];

            for (int i = 0; i < (width * height) * 4; i += 4)
                palColors.Add(new Color32(rgba[i + 2], rgba[i + 1], rgba[i + 0], rgba[i + 3]));

            SortedList<ushort, Color32> rawColorData = new SortedList<ushort, Color32>();
            foreach (Color32 col in palColors)
            {
                EncodeColor(paletteFormat, col, rawColorData);
            }

            int pixIndex = 0;
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            pixIndices[pixIndex++] = (byte)rawColorData.IndexOfValue(palColors[width * ((yBlock * 4) + pY) + (xBlock * 8) + pX]);
                        }
                    }
                }
            }

            //PaletteCount = (ushort)rawColorData.Count;

            return new Tuple<byte[], ushort[]>(pixIndices, rawColorData.Keys.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paletteFormat"></param>
        /// <param name="col"></param>
        /// <param name="rawColorData"></param>
        private static void EncodeColor(GXTlutFmt paletteFormat, Color32 col, SortedList<ushort, Color32> rawColorData)
        {
            switch (paletteFormat)
            {
                case GXTlutFmt.IA8:
                    byte i = (byte)((col.R * 0.2126) + (col.G * 0.7152) + (col.B * 0.0722));

                    ushort fullIA8 = (ushort)((i << 8) | (col.A));
                    if (!rawColorData.ContainsKey(fullIA8))
                        rawColorData.Add((ushort)(fullIA8), col);
                    break;
                case GXTlutFmt.RGB565:
                    ushort r_565 = (ushort)(col.R >> 3);
                    ushort g_565 = (ushort)(col.G >> 2);
                    ushort b_565 = (ushort)(col.B >> 3);

                    ushort fullColor565 = 0;
                    fullColor565 |= b_565;
                    fullColor565 |= (ushort)(g_565 << 5);
                    fullColor565 |= (ushort)(r_565 << 11);

                    if (!rawColorData.ContainsKey(fullColor565))
                        rawColorData.Add(fullColor565, col);
                    break;
                case GXTlutFmt.RGB5A3:
                    ushort r_53 = (ushort)(col.R >> 4);
                    ushort g_53 = (ushort)(col.G >> 4);
                    ushort b_53 = (ushort)(col.B >> 4);
                    ushort a_53 = (ushort)(col.A >> 5);

                    ushort fullColor53 = 0;
                    fullColor53 |= b_53;
                    fullColor53 |= (ushort)(g_53 << 4);
                    fullColor53 |= (ushort)(r_53 << 8);
                    fullColor53 |= (ushort)(a_53 << 12);

                    if (!rawColorData.ContainsKey(fullColor53))
                        rawColorData.Add(fullColor53, col);
                    break;
            }
        }
    }

    public delegate byte[] ConvertBlockDelegate(byte[] block);
}