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

using HSDRaw.GX;
using System;
using System.Collections.Generic;

namespace HSDRaw.Tools
{
    internal class ColorIndexConverter
    {
        private uint[] rgbaPalette;
        private byte[] tplPalette;
        private uint[] rgbaData;
        private byte[] tplData;
        private GXTexFmt tplFormat;
        private GXTlutFmt paletteFormat;
        private int width;
        private int height;

        public byte[] Palette { get { return tplPalette; } }
        public byte[] Data { get { return tplData; } }

        public ColorIndexConverter(uint[] rgbaData, int width, int height, GXTexFmt tplFormat, GXTlutFmt paletteFormat)
        {
            if (tplFormat != GXTexFmt.CI4 && tplFormat != GXTexFmt.CI8) // && tplFormat != TPL_TextureFormat.CI14X2)
                throw new Exception("Texture format must be either CI4 or CI8"); // or CI14X2!");
            if (paletteFormat != GXTlutFmt.IA8 && paletteFormat != GXTlutFmt.RGB565 && paletteFormat != GXTlutFmt.RGB5A3)
                throw new Exception("Palette format must be either IA8, RGB565 or RGB5A3!");

            this.rgbaData = rgbaData;
            this.width = width;
            this.height = height;
            this.tplFormat = tplFormat;
            this.paletteFormat = paletteFormat;

            buildPalette();

            if (tplFormat == GXTexFmt.CI4) toCI4();
            else if (tplFormat == GXTexFmt.CI8) toCI8();
            else toCI14X2();
        }


        #region Private Functions
        private void toCI4()
        {
            byte[] indexData = new byte[Shared.AddPadding(width, 8) * Shared.AddPadding(height, 8) / 2];
            int i = 0;

            for (int y = 0; y < height; y += 8)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 8; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1 += 2)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            uint index1 = getColorIndex(pixel);

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else if (y1 * width + x1 + 1 >= rgbaData.Length)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1 + 1];

                            uint index2 = getColorIndex(pixel);

                            indexData[i++] = (byte)(((byte)index1 << 4) | (byte)index2);
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void toCI8()
        {
            byte[] indexData = new byte[Shared.AddPadding(width, 8) * Shared.AddPadding(height, 4)];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 8)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 8; x1++)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            indexData[i++] = (byte)getColorIndex(pixel);
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void toCI14X2()
        {
            byte[] indexData = new byte[Shared.AddPadding(width, 4) * Shared.AddPadding(height, 4) * 2];
            int i = 0;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    for (int y1 = y; y1 < y + 4; y1++)
                    {
                        for (int x1 = x; x1 < x + 4; x1++)
                        {
                            uint pixel;

                            if (y1 >= height || x1 >= width)
                                pixel = 0;
                            else
                                pixel = rgbaData[y1 * width + x1];

                            byte[] temp = BitConverter.GetBytes((ushort)getColorIndex(pixel));
                            indexData[i++] = temp[1];
                            indexData[i++] = temp[0];
                        }
                    }
                }
            }

            this.tplData = indexData;
        }

        private void buildPalette()
        {
            int palLength = 256;
            if (tplFormat == GXTexFmt.CI4) palLength = 16;
            else if (tplFormat == GXTexFmt.CI14X2) palLength = 16384;

            List<uint> palette = new List<uint>();
            List<ushort> tPalette = new List<ushort>();

            palette.Add(0);
            tPalette.Add(0);

            for (int i = 1; i < rgbaData.Length; i++)
            {
                if (palette.Count == palLength) break;
                if (((rgbaData[i] >> 24) & 0xff) < ((tplFormat == GXTexFmt.CI14X2) ? 1 : 25)) continue;

                ushort tplValue = Shared.Swap(convertToPaletteValue((int)rgbaData[i]));

                if (!palette.Contains(rgbaData[i]) && !tPalette.Contains(tplValue))
                {
                    palette.Add(rgbaData[i]);
                    tPalette.Add(tplValue);
                }
            }

            while (palette.Count % 16 != 0)
            { palette.Add(0xffffffff); tPalette.Add(0xffff); }

            tplPalette = Shared.UShortArrayToByteArray(tPalette.ToArray());
            rgbaPalette = palette.ToArray();
        }

        private ushort convertToPaletteValue(int rgba)
        {
            int newpixel = 0, r, g, b, a;

            if (paletteFormat == GXTlutFmt.IA8)
            {
                int intensity = ((((rgba >> 0) & 0xff) + ((rgba >> 8) & 0xff) + ((rgba >> 16) & 0xff)) / 3) & 0xff;
                int alpha = (rgba >> 24) & 0xff;

                newpixel = (ushort)((alpha << 8) | intensity);
            }
            else if (paletteFormat == GXTlutFmt.RGB565)
            {
                newpixel = (ushort)(((((rgba >> 16) & 0xff) >> 3) << 11) | ((((rgba >> 8) & 0xff) >> 2) << 5) | ((((rgba >> 0) & 0xff) >> 3) << 0));
            }
            else
            {
                r = (rgba >> 16) & 0xff;
                g = (rgba >> 8) & 0xff;
                b = (rgba >> 0) & 0xff;
                a = (rgba >> 24) & 0xff;

                if (a <= 0xda) //RGB4A3
                {
                    newpixel &= ~(1 << 15);

                    r = ((r * 15) / 255) & 0xf;
                    g = ((g * 15) / 255) & 0xf;
                    b = ((b * 15) / 255) & 0xf;
                    a = ((a * 7) / 255) & 0x7;

                    newpixel |= a << 12;
                    newpixel |= b << 0;
                    newpixel |= g << 4;
                    newpixel |= r << 8;
                }
                else //RGB5
                {
                    newpixel |= (1 << 15);

                    r = ((r * 31) / 255) & 0x1f;
                    g = ((g * 31) / 255) & 0x1f;
                    b = ((b * 31) / 255) & 0x1f;

                    newpixel |= b << 0;
                    newpixel |= g << 5;
                    newpixel |= r << 10;
                }
            }

            return (ushort)newpixel;
        }

        private uint getColorIndex(uint value)
        {
            uint minDistance = 0x7FFFFFFF;
            uint colorIndex = 0;

            if (((value >> 24) & 0xFF) < ((tplFormat == GXTexFmt.CI14X2) ? 1 : 25)) return 0;
            ushort color = convertToPaletteValue((int)value);

            for (int i = 0; i < rgbaPalette.Length; i++)
            {
                ushort curPal = convertToPaletteValue((int)rgbaPalette[i]);

                if (color == curPal) return (uint)i;
                uint curDistance = getDistance(color, curPal); //(uint)Math.Abs(Math.Abs(color) - Math.Abs(curVal));

                if (curDistance < minDistance)
                {
                    minDistance = curDistance;
                    colorIndex = (uint)i;
                }
            }

            return colorIndex;
        }

        private uint getDistance(ushort color, ushort paletteColor)
        {
            uint curCol = convertToRgbaValue(color);
            uint palCol = convertToRgbaValue(paletteColor);

            uint curA = (curCol >> 24) & 0xFF;
            uint curR = (curCol >> 16) & 0xFF;
            uint curG = (curCol >> 8) & 0xFF;
            uint curB = (curCol >> 0) & 0xFF;

            uint palA = (palCol >> 24) & 0xFF;
            uint palR = (palCol >> 16) & 0xFF;
            uint palG = (palCol >> 8) & 0xFF;
            uint palB = (palCol >> 0) & 0xFF;

            uint distA = Math.Max(curA, palA) - Math.Min(curA, palA);
            uint distR = Math.Max(curR, palR) - Math.Min(curR, palR);
            uint distG = Math.Max(curG, palG) - Math.Min(curG, palG);
            uint distB = Math.Max(curB, palB) - Math.Min(curB, palB);

            return distA + distR + distG + distB;
        }

        private uint convertToRgbaValue(ushort pixel)
        {
            int rgba = 0, r, g, b, a;

            if (paletteFormat == GXTlutFmt.IA8)
            {
                int i = (pixel >> 8);
                a = pixel & 0xff;

                rgba = (i << 0) | (i << 8) | (i << 16) | (a << 24);
            }
            else if (paletteFormat == GXTlutFmt.RGB565)
            {
                b = (((pixel >> 11) & 0x1F) << 3) & 0xff;
                g = (((pixel >> 5) & 0x3F) << 2) & 0xff;
                r = (((pixel >> 0) & 0x1F) << 3) & 0xff;
                a = 255;

                rgba = (r << 0) | (g << 8) | (b << 16) | (a << 24);
            }
            else
            {
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

                rgba = (r << 0) | (g << 8) | (b << 16) | (a << 24);
            }

            return (uint)rgba;
        }
        #endregion
    }
}
