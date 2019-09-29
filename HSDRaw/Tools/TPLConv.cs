/* This file is part of libWiiSharp
 * Copyright (C) 2009 Leathl
 * 
 * libWiiSharp is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * libWiiSharp is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

//TPL conversion based on Wii.py by Xuzz, SquidMan, megazig, Matt_P, Omega and The Lemon Man.
//Zetsubou by SquidMan was also a reference.
//Thanks to the authors!

// Adapted and simplified for use with HSDLib

using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Net;

namespace HSDRaw.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class TPLConv
    {
        public static byte[] EncodeTPL(byte[] rgba, int width, int height, GXTexFmt format, GXTlutFmt palformat, out byte[] paletteData)
        {
            return EncodeTPL(Shared.ByteArrayToUIntArray(rgba), width, height, format, palformat, out paletteData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgba"></param>
        /// <param name="format"></param>
        /// <param name="palformat"></param>
        /// <param name="paletteData"></param>
        /// <returns></returns>
        public static byte[] EncodeTPL(uint[] rgba, int width, int height, GXTexFmt format, GXTlutFmt palformat, out byte[] paletteData)
        {
            paletteData = null;

            if (IsPalettedFormat(format))
            {
                return FromImage(rgba, width, height, format, palformat, out paletteData);
            }
            else
            {
                return FromImage(rgba, width, height, format);
            }
        }

        public static byte[] DecodeTPL(GXTexFmt format, int width, int height, byte[] imageData)
        {
            return DecodeTPL(format, width, height, imageData, GXTlutFmt.RGB565, 0, new byte[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageData"></param>
        /// <param name="palformat"></param>
        /// <param name="colorCount"></param>
        /// <param name="paletteData"></param>
        /// <returns></returns>
        public static byte[] DecodeTPL(GXTexFmt format, int width, int height, byte[] imageData, GXTlutFmt palformat, int colorCount, byte[] paletteData)
        {
            var paletteDataRgba = new uint[0];

            if (IsPalettedFormat(format))
                paletteDataRgba = PaletteToRGBA(palformat, colorCount, paletteData);

            byte[] rgba;
            switch (format)
            {
                case GXTexFmt.I4:
                    rgba = fromI4(imageData, width, height);
                    break;
                case GXTexFmt.I8:
                    rgba = fromI8(imageData, width, height);
                    break;
                case GXTexFmt.IA4:
                    rgba = fromIA4(imageData, width, height);
                    break;
                case GXTexFmt.IA8:
                    rgba = fromIA8(imageData, width, height);
                    break;
                case GXTexFmt.RGB565:
                    rgba = fromRGB565(imageData, width, height);
                    break;
                case GXTexFmt.RGB5A3:
                    rgba = fromRGB5A3(imageData, width, height);
                    break;
                case GXTexFmt.RGBA8:
                    rgba = fromRGBA8(imageData, width, height);
                    break;
                case GXTexFmt.CI4:
                    rgba = new byte[0];
                    rgba = fromCI4(imageData, paletteDataRgba, width, height);
                    break;
                case GXTexFmt.CI8:
                    rgba = new byte[0];
                    rgba = fromCI8(imageData, paletteDataRgba, width, height);
                    break;
                case GXTexFmt.CI14X2:
                    rgba = new byte[0];
                    rgba = fromCI14X2(imageData, paletteDataRgba, width, height);
                    break;
                case GXTexFmt.CMP:
                    rgba = fromCMP(imageData, width, height);
                    break;
                default:
                    rgba = new byte[0];
                    break;
            }
            
            return rgba;
        }

        private static byte[] FromImage(uint[] rgba, int width, int height, GXTexFmt format)
        {
            byte[] b;
            return FromImage(rgba, width, height, format, GXTlutFmt.IA8, out b);
        }

        private static byte[] FromImage(uint[] rgba, int width, int height, GXTexFmt format, GXTlutFmt palFormat, out byte[] palData)
        {
            palData = new byte[0];
            if (IsPalettedFormat(format))
            {
                ColorIndexConverter cic = new ColorIndexConverter(rgba, width, height, format, palFormat);
                
                palData = cic.Palette;
                return cic.Data;
            }
            else
            {
                return ImageToTPL(rgba, width, height, format);
            }
        }
        
        private static byte[] ImageToTPL(uint[] rgba, int width, int height, GXTexFmt tplFormat)
        {
            switch (tplFormat)
            {
                case GXTexFmt.I4:
                    return toI4(rgba, width, height);
                case GXTexFmt.I8:
                    return toI8(rgba, width, height);
                case GXTexFmt.IA4:
                    return toIA4(rgba, width, height);
                case GXTexFmt.IA8:
                    return toIA8(rgba, width, height);
                case GXTexFmt.RGB565:
                    return toRGB565(rgba, width, height);
                case GXTexFmt.RGB5A3:
                    return toRGB5A3(rgba, width, height);
                case GXTexFmt.RGBA8:
                    return toRGBA8(rgba, width, height);
                case GXTexFmt.CI4:
                case GXTexFmt.CI8:
                case GXTexFmt.CI14X2:
                case GXTexFmt.CMP:
                    return new byte[0];
                default:
                    throw new FormatException(tplFormat.ToString() + " - Format not supported!\nCurrently, images can only be converted to the following formats:\nI4, I8, IA4, IA8, RGB565, RGB5A3, RGBA8, CI4, CI8 , CI14X2.");
            }
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
        
        private static int avg(int w0, int w1, int c0, int c1)
        {
            int a0 = c0 >> 11;
            int a1 = c1 >> 11;
            int a = (w0 * a0 + w1 * a1) / (w0 + w1);
            int c = (a << 11) & 0xffff;

            a0 = (c0 >> 5) & 63;
            a1 = (c1 >> 5) & 63;
            a = (w0 * a0 + w1 * a1) / (w0 + w1);
            c = c | ((a << 5) & 0xffff);

            a0 = c0 & 31;
            a1 = c1 & 31;
            a = (w0 * a0 + w1 * a1) / (w0 + w1);
            c = c | a;

            return c;
        }

        private static uint[] PaletteToRGBA(GXTlutFmt format, int count, byte[] data)
        {
            int itemcount = count;
            int r, g, b, a;

            uint[] output = new uint[itemcount];
            for (int i = 0; i < itemcount; i++)
            {
                if (i >= itemcount) continue;

                ushort pixel = BitConverter.ToUInt16(new byte[] { data[i * 2 + 1], data[i * 2] }, 0);

                if (format == GXTlutFmt.IA8) //IA8
                {
                    r = pixel & 0xff;
                    b = r;
                    g = r;
                    a = pixel >> 8;
                }
                else if (format == GXTlutFmt.RGB565) //RGB565
                {
                    b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                    g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                    r = (((pixel >> 0) & 0x1F) << 3) & 0xff;
                    a = 255;
                }
                else //RGB5A3
                {
                    if ((pixel & (1 << 15)) != 0) //RGB555
                    {
                        a = 255;
                        b = (((pixel >> 10) & 0x1F) * 255) / 31;
                        g = (((pixel >> 5) & 0x1F) * 255) / 31;
                        r = (((pixel >> 0) & 0x1F) * 255) / 31;
                    }
                    else //RGB4A3
                    {
                        a = (((pixel >> 12) & 0x07) * 255) / 7;
                        b = (((pixel >> 8) & 0x0F) * 255) / 15;
                        g = (((pixel >> 4) & 0x0F) * 255) / 15;
                        r = (((pixel >> 0) & 0x0F) * 255) / 15;
                    }
                }

                output[i] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
            }

            return output;
        }


        #region Conversions
        #region RGBA8
        private static byte[] fromRGBA8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int y1 = y; y1 < y + 4; y1++)
                        {
                            for (int x1 = x; x1 < x + 4; x1++)
                            {
                                ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                                if ((x1 >= width) || (y1 >= height))
                                    continue;

                                if (k == 0)
                                {
                                    int a = (pixel >> 8) & 0xff;
                                    int r = (pixel >> 0) & 0xff;
                                    output[x1 + (y1 * width)] |= (uint)((r << 16) | (a << 24));
                                }
                                else
                                {
                                    int g = (pixel >> 8) & 0xff;
                                    int b = (pixel >> 0) & 0xff;
                                    output[x1 + (y1 * width)] |= (uint)((g << 8) | (b << 0));
                                }
                            }
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }
        
        private static byte[] toRGBA8(uint[] pixeldata, int w, int h)
        {
            int z = 0, iv = 0;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 4];
            uint[] lr = new uint[32], lg = new uint[32], lb = new uint[32], la = new uint[32];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < (y1 + 4); y++)
                    {
                        for (int x = x1; x < (x1 + 4); x++)
                        {
                            uint rgba;

                            if (y >= h || x >= w)
                                rgba = 0;
                            else
                                rgba = pixeldata[x + (y * w)];

                            lr[z] = (uint)(rgba >> 16) & 0xff;
                            lg[z] = (uint)(rgba >> 8) & 0xff;
                            lb[z] = (uint)(rgba >> 0) & 0xff;
                            la[z] = (uint)(rgba >> 24) & 0xff;

                            z++;
                        }
                    }

                    if (z == 16)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            output[iv++] = (byte)(la[i]);
                            output[iv++] = (byte)(lr[i]);
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            output[iv++] = (byte)(lg[i]);
                            output[iv++] = (byte)(lb[i]);
                        }

                        z = 0;
                    }
                }
            }

            return output;
        }
        #endregion

        #region RGB5A3
        private static byte[] fromRGB5A3(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;
            int r, g, b;
            int a = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            if ((pixel & (1 << 15)) != 0)
                            {
                                b = (((pixel >> 10) & 0x1F) * 255) / 31;
                                g = (((pixel >> 5) & 0x1F) * 255) / 31;
                                r = (((pixel >> 0) & 0x1F) * 255) / 31;
                                a = 255;
                            }
                            else
                            {
                                a = (((pixel >> 12) & 0x07) * 255) / 7;
                                b = (((pixel >> 8) & 0x0F) * 255) / 15;
                                g = (((pixel >> 4) & 0x0F) * 255) / 15;
                                r = (((pixel >> 0) & 0x0F) * 255) / 15;
                            }

                            output[(y1 * width) + x1] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toRGB5A3(uint[] pixeldata, int w, int h)
        {
            int z = -1;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            int newpixel;

                            if (y >= h || x >= w)
                                newpixel = 0;
                            else
                            {
                                int rgba = (int)pixeldata[x + (y * w)];
                                newpixel = 0;

                                int r = (rgba >> 16) & 0xff;
                                int g = (rgba >> 8) & 0xff;
                                int b = (rgba >> 0) & 0xff;
                                int a = (rgba >> 24) & 0xff;

                                if (a <= 0xda) //RGB4A3
                                {
                                    newpixel &= ~(1 << 15);

                                    r = ((r * 15) / 255) & 0xf;
                                    g = ((g * 15) / 255) & 0xf;
                                    b = ((b * 15) / 255) & 0xf;
                                    a = ((a * 7) / 255) & 0x7;

                                    newpixel |= (a << 12) | (r << 8) | (g << 4) | b;
                                }
                                else //RGB5
                                {
                                    newpixel |= (1 << 15);

                                    r = ((r * 31) / 255) & 0x1f;
                                    g = ((g * 31) / 255) & 0x1f;
                                    b = ((b * 31) / 255) & 0x1f;

                                    newpixel |= (r << 10) | (g << 5) | b;
                                }
                            }

                            output[++z] = (byte)(newpixel >> 8);
                            output[++z] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region RGB565
        private static byte[] fromRGB565(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            int b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                            int g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                            int r = (((pixel >> 0) & 0x1F) << 3) & 0xff;

                            output[y1 * width + x1] = (uint)((r << 0) | (g << 8) | (b << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toRGB565(uint[] pixeldata, int w, int h)
        {
            int z = -1;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            ushort newpixel;

                            if (y >= h || x >= w)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint b = (rgba >> 16) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint r = (rgba >> 0) & 0xff;

                                newpixel = (ushort)(((b >> 3) << 11) | ((g >> 2) << 5) | ((r >> 3) << 0));
                            }

                            output[++z] = (byte)(newpixel >> 8);
                            output[++z] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region I4
        private static byte[] fromI4(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            int i = (pixel >> 4) * 255 / 15;
                            output[y1 * width + x1] = (uint)((i << 0) | (i << 8) | (i << 16) | (255 << 24));

                            i = (pixel & 0x0F) * 255 / 15;
                            if (y1 * width + x1 + 1 < output.Length) output[y1 * width + x1 + 1] = (uint)((i << 0) | (i << 8) | (i << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toI4(uint[] pixeldata, int w, int h)
        {
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 8) / 2];

            for (int y1 = 0; y1 < h; y1 += 8)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 8; y++)
                    {
                        for (int x = x1; x < x1 + 8; x += 2)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i1 = ((r + g + b) / 3) & 0xff;

                                if ((x + (y * w) + 1) >= pixeldata.Length) rgba = 0;
                                else rgba = pixeldata[x + (y * w) + 1];

                                r = (rgba >> 0) & 0xff;
                                g = (rgba >> 8) & 0xff;
                                b = (rgba >> 16) & 0xff;

                                uint i2 = ((r + g + b) / 3) & 0xff;

                                newpixel = (byte)((((i1 * 15) / 255) << 4) | (((i2 * 15) / 255) & 0xf));
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region I8
        private static byte[] fromI8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = (uint)((pixel << 0) | (pixel << 8) | (pixel << 16) | (255 << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toI8(uint[] pixeldata, int w, int h)
        {
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 4)];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 8; x++)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                newpixel = (byte)(((r + g + b) / 3) & 0xff);
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region IA4
        private static byte[] fromIA4(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            int pixel = tpl[inp++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            int i = ((pixel & 0x0F) * 255 / 15) & 0xff;
                            int a = (((pixel >> 4) * 255) / 15) & 0xff;

                            output[y1 * width + x1] = (uint)((i << 0) | (i << 8) | (i << 16) | (a << 24));
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toIA4(uint[] pixeldata, int w, int h)
        {
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 8) * Shared.AddPadding(h, 4)];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 8)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 8; x++)
                        {
                            byte newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i = ((r + g + b) / 3) & 0xff;
                                uint a = (rgba >> 24) & 0xff;

                                newpixel = (byte)((((i * 15) / 255) & 0xf) | (((a * 15) / 255) << 4));
                            }

                            output[inp++] = newpixel;
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region IA8
        private static byte[] fromIA8(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            int inp = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            int pixel = Shared.Swap(BitConverter.ToUInt16(tpl, inp++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            uint a = (uint)(pixel >> 8);
                            uint i = (uint)(pixel & 0xff);

                            output[y1 * width + x1] = (i << 0) | (i << 8) | (i << 16) | (a << 24);
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        private static byte[] toIA8(uint[] pixeldata, int w, int h)
        {
            int inp = 0;
            byte[] output = new byte[Shared.AddPadding(w, 4) * Shared.AddPadding(h, 4) * 2];

            for (int y1 = 0; y1 < h; y1 += 4)
            {
                for (int x1 = 0; x1 < w; x1 += 4)
                {
                    for (int y = y1; y < y1 + 4; y++)
                    {
                        for (int x = x1; x < x1 + 4; x++)
                        {
                            ushort newpixel;

                            if (x >= w || y >= h)
                                newpixel = 0;
                            else
                            {
                                uint rgba = pixeldata[x + (y * w)];

                                uint r = (rgba >> 0) & 0xff;
                                uint g = (rgba >> 8) & 0xff;
                                uint b = (rgba >> 16) & 0xff;

                                uint i = ((r + g + b) / 3) & 0xff;
                                uint a = (rgba >> 24) & 0xff;

                                newpixel = (ushort)((a << 8) | i);
                            }

                            byte[] temp = BitConverter.GetBytes(newpixel);
                            Array.Reverse(temp);

                            output[inp++] = (byte)(newpixel >> 8);
                            output[inp++] = (byte)(newpixel & 0xff);
                        }
                    }
                }
            }

            return output;
        }
        #endregion

        #region CI4
        private static byte[] fromCI4(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            byte pixel = tpl[i++];

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = paletteData[pixel >> 4]; ;
                            if (y1 * width + x1 + 1 < output.Length) output[y1 * width + x1 + 1] = paletteData[pixel & 0x0F];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI4 done in class ColorIndexConverter
        #endregion

        #region CI8
        private static byte[] fromCI8(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            ushort pixel = tpl[i++];

                            if (y1 >= height || x1 >= width)
                                continue;
                            if (pixel < paletteData.Length)
                                output[y1 * width + x1] = paletteData[pixel];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI8 done in class ColorIndexConverter
        #endregion

        #region CI14X2
        private static byte[] fromCI14X2(byte[] tpl, uint[] paletteData, int width, int height)
        {
            uint[] output = new uint[width * height];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            ushort pixel = Shared.Swap(BitConverter.ToUInt16(tpl, i++ * 2));

                            if (y1 >= height || x1 >= width)
                                continue;

                            output[y1 * width + x1] = paletteData[pixel & 0x3FFF];
                        }
                    }
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        //toCI14X2 done in class ColorIndexConverter
        #endregion

        #region CMP
        public static byte[] fromCMP(byte[] tpl, int width, int height)
        {
            uint[] output = new uint[width * height];
            ushort[] c = new ushort[4];
            int[] pix = new int[4];
            int inp = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int ww = Shared.AddPadding(width, 8);

                    int x0 = x & 0x03;
                    int x1 = (x >> 2) & 0x01;
                    int x2 = x >> 3;

                    int y0 = y & 0x03;
                    int y1 = (y >> 2) & 0x01;
                    int y2 = y >> 3;

                    int off = (8 * x1) + (16 * y1) + (32 * x2) + (4 * ww * y2);

                    c[0] = Shared.Swap(BitConverter.ToUInt16(tpl, off));
                    c[1] = Shared.Swap(BitConverter.ToUInt16(tpl, off + 2));

                    if (c[0] > c[1])
                    {
                        c[2] = (ushort)avg(2, 1, c[0], c[1]);
                        c[3] = (ushort)avg(1, 2, c[0], c[1]);
                    }
                    else
                    {
                        c[2] = (ushort)avg(1, 1, c[0], c[1]);
                        c[3] = 0;
                    }

                    uint pixel = Shared.Swap(BitConverter.ToUInt32(tpl, off + 4));

                    int ix = x0 + (4 * y0);
                    int raw = c[(pixel >> (30 - (2 * ix))) & 0x03];

                    pix[0] = (raw >> 8) & 0xf8;
                    pix[1] = (raw >> 3) & 0xf8;
                    pix[2] = (raw << 3) & 0xf8;
                    pix[3] = 0xff;
                    if (((pixel >> (30 - (2 * ix))) & 0x03) == 3 && c[0] <= c[1]) pix[3] = 0x00;

                    output[inp] = (uint)((pix[0] << 16) | (pix[1] << 8) | (pix[2] << 0) | (pix[3] << 24));
                    inp++;
                }
            }

            return Shared.UIntArrayToByteArray(output);
        }

        #endregion
        
        public static byte[] ToCMP(byte[] data, int width, int height)
        {
            byte[] output = new byte[width * height / 2];

            int off1 = 0;
            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    int ww = Shared.AddPadding(width, 8);

                    int x0 = x & 0x03;
                    int x1 = (x >> 2) & 0x01;
                    int x2 = x >> 3;

                    int y0 = y & 0x03;
                    int y1 = (y >> 2) & 0x01;
                    int y2 = y >> 3;

                    int off = (8 * x1) + (16 * y1) + (32 * x2) + (4 * ww * y2);

                    output[off + 0] = data[off1 + 1];
                    output[off + 1] = data[off1 + 0];
                    output[off + 2] = data[off1 + 3];
                    output[off + 3] = data[off1 + 2];

                    for (int i = 0; i < 4; i++)
                    {
                        output[off + 4 + i] = SwapBits(data[off1 + 4 + i]);
                    }
                    off1 += 8;
                }
            }
            return output;
        }

        private static byte SwapBits(byte b)
        {
            byte Y = 0;
            for (int i = 0; i < 8; i += 2)
            {
                Y |= (byte)(((b >> (8 - i - 2)) & 0x3) << i);
            }
            return Y;
        }



        #endregion

    }


    public static class Shared
    {
        /// <summary>
        /// Merges two string arrays into one without double entries.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string[] MergeStringArrays(string[] a, string[] b)
        {
            List<string> sList = new List<string>(a);

            foreach (string currentString in b)
                if (!sList.Contains(currentString)) sList.Add(currentString);

            sList.Sort();
            return sList.ToArray();
        }

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="firstIndex"></param>
        /// <param name="second"></param>
        /// <param name="secondIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] first, int firstIndex, byte[] second, int secondIndex, int length)
        {
            if (first.Length < length || second.Length < length) return false;

            for (int i = 0; i < length; i++)
                if (first[firstIndex + i] != second[secondIndex + i]) return false;

            return true;
        }

        /// <summary>
        /// Compares two byte arrays.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] first, byte[] second)
        {
            if (first.Length != second.Length) return false;
            else
                for (int i = 0; i < first.Length; i++)
                    if (first[i] != second[i]) return false;

            return true;
        }

        /// <summary>
        /// Turns a byte array into a string, default separator is a space.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] byteArray, char separator = ' ')
        {
            string res = string.Empty;

            foreach (byte b in byteArray)
                res += b.ToString("x2").ToUpper() + separator;

            return res.Remove(res.Length - 1);
        }

        /// <summary>
        /// Turns a hex string into a byte array.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] ba = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length / 2; i++)
                ba[i] = byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

            return ba;
        }

        /// <summary>
        /// Counts how often the given char exists in the given string.
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="theChar"></param>
        /// <returns></returns>
        public static int CountCharsInString(string theString, char theChar)
        {
            int count = 0;

            foreach (char thisChar in theString)
                if (thisChar == theChar)
                    count++;

            return count;
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long AddPadding(long value)
        {
            return AddPadding(value, 64);
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static long AddPadding(long value, int padding)
        {
            if (value % padding != 0)
            {
                value = value + (padding - (value % padding));
            }

            return value;
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int AddPadding(int value)
        {
            return AddPadding(value, 64);
        }

        /// <summary>
        /// Pads the given value to a multiple of the given padding value, default padding value is 64.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static int AddPadding(int value, int padding)
        {
            if (value % padding != 0)
            {
                value = value + (padding - (value % padding));
            }

            return value;
        }

        /// <summary>
        /// Swaps endianness.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort Swap(ushort value)
        {
            return (ushort)IPAddress.HostToNetworkOrder((short)value);
        }

        /// <summary>
        /// Swaps endianness.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Swap(uint value)
        {
            return (uint)IPAddress.HostToNetworkOrder((int)value);
        }

        /// <summary>
        /// Swaps endianness
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong Swap(ulong value)
        {
            return (ulong)IPAddress.HostToNetworkOrder((long)value);
        }

        /// <summary>
        /// Turns a ushort array into a byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] UShortArrayToByteArray(ushort[] array)
        {
            List<byte> results = new List<byte>();
            foreach (ushort value in array)
            {
                byte[] converted = BitConverter.GetBytes(value);
                results.AddRange(converted);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Turns a uint array into a byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte[] UIntArrayToByteArray(uint[] array)
        {
            List<byte> results = new List<byte>();
            foreach (uint value in array)
            {
                byte[] converted = BitConverter.GetBytes(value);
                results.AddRange(converted);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Turns a byte array into a uint array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static uint[] ByteArrayToUIntArray(byte[] array)
        {
            UInt32[] converted = new UInt32[array.Length / 4];
            int j = 0;

            for (int i = 0; i < array.Length; i += 4)
                converted[j++] = BitConverter.ToUInt32(array, i);

            return converted;
        }

        /// <summary>
        /// Turns a byte array into a ushort array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ushort[] ByteArrayToUShortArray(byte[] array)
        {
            ushort[] converted = new ushort[array.Length / 2];
            int j = 0;

            for (int i = 0; i < array.Length; i += 2)
                converted[j++] = BitConverter.ToUInt16(array, i);

            return converted;
        }
    }
}
