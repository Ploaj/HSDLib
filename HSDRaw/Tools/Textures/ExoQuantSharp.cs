/*
ExoQuantSharp (ExoQuant v0.7)

Copyright (c) 2019 David Benepe
Copyright (c) 2004 Dennis Ranke

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/******************************************************************************
* Usage:
* ------
*
* ExoQuant exq = new ExoQuant(); // init quantizer (per image)
* exq.Feed(<byte array of rgba32 data>); // feed pixel data (32bpp)
* exq.Quantize(<num of colors>); // find palette
* exq.GetPalette(<output rgba32 palette>, <num of colors>); // get palette
* exq.MapImage(<num of pixels>, <byte array of rgba32 data>, <output index data>);
* or:
* exq.MapImageOrdered(<width>, <height>, <byte array of rgba32 data>, <output index data>); 
* // map image to palette
*
* Notes:
* ------
*
* All 32bpp data (input data and palette data) is considered a byte stream
* of the format:
* R0 G0 B0 A0 R1 G1 B1 A1 ...
* If you want to use a different order, the easiest way to do this is to
* change the SCALE_x constants in expquant.h, as those are the only differences
* between the channels.
*
******************************************************************************/

using System;

namespace ExoQuantSharp
{
    using ExqFloat = Double; // C#'s equivalent of C's typedef

    public class ExoQuant
    {
        public class ExqColor
        {
            public ExqFloat r, g, b, a;
        }

        public class ExqHistogramEntry
        {
            public ExqColor color = new ExqColor();
            public byte ored, ogreen, oblue, oalpha;
            public int palIndex;
            public ExqColor ditherScale = new ExqColor();
            public int[] ditherIndex = new int[4];
            public int num = 0;
            public ExqHistogramEntry pNext = null;
            public ExqHistogramEntry pNextInHash = null;
        }

        static readonly int EXQ_HASH_BITS = 16;
        static readonly int EXQ_HASH_SIZE = (1 << (EXQ_HASH_BITS));

        static readonly float SCALE_R = 1.0f;
        static readonly float SCALE_G = 1.2f;
        static readonly float SCALE_B = 0.8f;
        static readonly float SCALE_A = 1.0f;

        public class ExqNode
        {
            public ExqColor dir = new ExqColor(), avg = new ExqColor();
            public ExqFloat vdif;
            public ExqFloat err;
            public int num;
            public ExqHistogramEntry pHistogram = null;
            public ExqHistogramEntry pSplit = null;
        }

        public class ExqData
        {
            public ExqHistogramEntry[] pHash = new ExqHistogramEntry[EXQ_HASH_SIZE];
            public ExqNode[] node = new ExqNode[256];
            public int numColors;
            public int numBitsPerChannel;
            public bool optimized;
            public bool transparency;
        }

        // Don't need to pass data through the functions like in C, just keep it private within the class.
        private ExqData pExq;

        public ExoQuant()
        {
            pExq = new ExqData();

            for (int i = 0; i < 256; i++)
            {
                pExq.node[i] = new ExqNode();
            }

            for (int i = 0; i < EXQ_HASH_SIZE; i++)
            {
                pExq.pHash[i] = null;
            }

            pExq.numColors = 0;
            pExq.optimized = false;
            pExq.transparency = true;
            pExq.numBitsPerChannel = 8;
        }

        public void NoTransparency()
        {
            pExq.transparency = false;
        }

        private uint MakeHash(uint rgba)
        {
            rgba -= (rgba >> 13) | (rgba << 19);
            rgba -= (rgba >> 13) | (rgba << 19);
            rgba -= (rgba >> 13) | (rgba << 19);
            rgba -= (rgba >> 13) | (rgba << 19);
            rgba -= (rgba >> 13) | (rgba << 19);
            rgba &= (uint)(EXQ_HASH_SIZE - 1);
            return rgba;
        }

        private uint ToRGBA(uint r, uint g, uint b, uint a)
        {
            return r | (g << 8) | (b << 16) | (a << 24);
        }

        public void Feed(byte[] pData)
        {
            byte channelMask = (byte)(0xFF00 >> pExq.numBitsPerChannel);

            int nPixels = pData.Length / 4;

            for (int i = 0; i < nPixels; i++)
            {
                byte r = pData[i * 4 + 0], g = pData[i * 4 + 1],
                     b = pData[i * 4 + 2], a = pData[i * 4 + 3];

                uint hash = MakeHash(ToRGBA(r, g, b, a));

                ExqHistogramEntry pCur = pExq.pHash[hash];

                while (pCur != null && (pCur.ored != r || pCur.ogreen != g || pCur.oblue != b || pCur.oalpha != a))
                    pCur = pCur.pNextInHash;

                if (pCur != null)
                    pCur.num++;
                else
                {
                    pCur = new ExqHistogramEntry();
                    pCur.pNextInHash = pExq.pHash[hash];
                    pExq.pHash[hash] = pCur;
                    pCur.ored = r; pCur.ogreen = g; pCur.oblue = b; pCur.oalpha = a;
                    r &= channelMask; g &= channelMask; b &= channelMask;
                    pCur.color.r = r / 255.0f * SCALE_R;
                    pCur.color.g = g / 255.0f * SCALE_G;
                    pCur.color.b = b / 255.0f * SCALE_B;
                    pCur.color.a = a / 255.0f * SCALE_A;

                    if (pExq.transparency)
                    {
                        pCur.color.r *= pCur.color.a;
                        pCur.color.g *= pCur.color.a;
                        pCur.color.b *= pCur.color.a;
                    }

                    pCur.num = 1;
                    pCur.palIndex = -1;
                    pCur.ditherScale.r = pCur.ditherScale.g = pCur.ditherScale.b =
                        pCur.ditherScale.a = -1;
                    pCur.ditherIndex[0] = pCur.ditherIndex[1] = pCur.ditherIndex[2] =
                        pCur.ditherIndex[3] = -1;
                }
            }
        }

        public void Quantize(int nColors)
        {
            QuantizeEx(nColors, false);
        }

        public void QuantizeHq(int nColors)
        {
            QuantizeEx(nColors, true);
        }

        public void QuantizeEx(int nColors, bool hq)
        {
            int besti;
            ExqFloat beste;
            ExqHistogramEntry pCur, pNext;
            int i, j;

            if (nColors > 256)
                nColors = 256;

            if (pExq.numColors == 0)
            {
                pExq.node[0].pHistogram = null;
                for (i = 0; i < EXQ_HASH_SIZE; i++)
                    for (pCur = pExq.pHash[i]; pCur != null; pCur = pCur.pNextInHash)
                    {
                        pCur.pNext = pExq.node[0].pHistogram;
                        pExq.node[0].pHistogram = pCur;
                    }
                SumNode(pExq.node[0]);
                pExq.numColors = 1;
            }

            for (i = pExq.numColors; i < nColors; i++)
            {
                beste = 0;
                besti = 0;
                for (j = 0; j < i; j++)
                    if (pExq.node[j].vdif >= beste)
                    {
                        beste = pExq.node[j].vdif;
                        besti = j;
                    }

                pCur = pExq.node[besti].pHistogram;

                pExq.node[besti].pHistogram = null;
                pExq.node[i].pHistogram = null;
                while (pCur != null && pCur != pExq.node[besti].pSplit)
                {
                    pNext = pCur.pNext;
                    pCur.pNext = pExq.node[i].pHistogram;
                    pExq.node[i].pHistogram = pCur;
                    pCur = pNext;
                }

                while (pCur != null)
                {
                    pNext = pCur.pNext;
                    pCur.pNext = pExq.node[besti].pHistogram;
                    pExq.node[besti].pHistogram = pCur;
                    pCur = pNext;
                }

                SumNode(pExq.node[besti]);
                SumNode(pExq.node[i]);

                pExq.numColors = i + 1;
                if (hq)
                    OptimizePalette(1);
            }

            pExq.optimized = false;
        }

        public ExqFloat GetMeanError()
        {
            int n = 0;
            ExqFloat err = 0;

            for (int i = 0; i < pExq.numColors; i++)
            {
                n += pExq.node[i].num;
                err += pExq.node[i].err;
            }

            return Math.Sqrt(err / n) * 256;
        }

        public void GetPalette(out byte[] pPal, int nColors)
        {
            ExqFloat r, g, b, a;
            byte channelMask = (byte)(0xff00 >> pExq.numBitsPerChannel);

            pPal = new byte[nColors * 4];

            if (nColors > pExq.numColors)
                nColors = pExq.numColors;

            if (!pExq.optimized)
                OptimizePalette(4);

            for (int i = 0; i < nColors; i++)
            {
                r = pExq.node[i].avg.r;
                g = pExq.node[i].avg.g;
                b = pExq.node[i].avg.b;
                a = pExq.node[i].avg.a;

                if (pExq.transparency && a != 0)
                {
                    r /= a; g /= a; b /= a;
                }

                int pPalIndex = i * 4;

                pPal[pPalIndex + 0] = (byte)(r / SCALE_R * 255.9f);
                pPal[pPalIndex + 1] = (byte)(g / SCALE_G * 255.9f);
                pPal[pPalIndex + 2] = (byte)(b / SCALE_B * 255.9f);
                pPal[pPalIndex + 3] = (byte)(a / SCALE_A * 255.9f);

                for (int j = 0; j < 3; j++)
                    pPal[pPalIndex + j] = (byte)((pPal[pPalIndex + j] + (1 << (8 - pExq.numBitsPerChannel)) / 2) & channelMask);
            }
        }

        public void SetPalette(byte[] pPal, int nColors)
        {
            pExq.numColors = nColors;

            for (int i = 0; i < nColors; i++)
            {
                pExq.node[i].avg.r = pPal[i * 4 + 0] * SCALE_R / 255.9f;
                pExq.node[i].avg.g = pPal[i * 4 + 1] * SCALE_G / 255.9f;
                pExq.node[i].avg.b = pPal[i * 4 + 2] * SCALE_B / 255.9f;
                pExq.node[i].avg.a = pPal[i * 4 + 3] * SCALE_A / 255.9f;
            }

            pExq.optimized = true;
        }

        public void MapImage(int nPixels, byte[] pIn, out byte[] pOut)
        {
            int i;
            ExqColor c = new ExqColor();
            ExqHistogramEntry pHist;

            pOut = new byte[nPixels];

            if (!pExq.optimized)
                OptimizePalette(4);

            for (i = 0; i < nPixels; i++)
            {
                pHist = FindHistogram(pIn, i);
                if (pHist != null && pHist.palIndex != -1)
                {
                    pOut[i] = (byte)pHist.palIndex;
                }
                else
                {
                    c.r = pIn[i * 4 + 0] / 255.0f * SCALE_R;
                    c.g = pIn[i * 4 + 1] / 255.0f * SCALE_G;
                    c.b = pIn[i * 4 + 2] / 255.0f * SCALE_B;
                    c.a = pIn[i * 4 + 3] / 255.0f * SCALE_A;

                    if (pExq.transparency)
                    {
                        c.r *= c.a; c.g *= c.a; c.b *= c.a;
                    }
                    pOut[i] = FindNearestColor(c);
                    if (pHist != null)
                        pHist.palIndex = i;
                }
            }
        }

        public void MapImageOrdered(int width, int height, byte[] pIn, out byte[] pOut)
        {
            MapImageDither(width, height, pIn, out pOut, true);
        }

        public void MapImageRandom(int nPixels, byte[] pIn, out byte[] pOut)
        {
            MapImageDither(nPixels, 1, pIn, out pOut, false);
        }

        private readonly Random random = new Random();
        private void MapImageDither(int width, int height, byte[] pIn, out byte[] pOut, bool ordered)
        {
            ExqFloat[] ditherMatrix = { -0.375, 0.125, 0.375, -0.125 };

            int i, j, d;
            ExqColor p = new ExqColor(),
                      scale = new ExqColor(),
                      tmp = new ExqColor();
            ExqHistogramEntry pHist;

            pOut = new byte[width * height];

            if (!pExq.optimized)
                OptimizePalette(4);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    if (ordered)
                        d = (x & 1) + (y & 1) * 2;
                    else
                        d = random.Next() & 3;

                    pHist = FindHistogram(pIn, index);

                    p.r = pIn[index * 4 + 0] / 255.0f * SCALE_R;
                    p.g = pIn[index * 4 + 1] / 255.0f * SCALE_G;
                    p.b = pIn[index * 4 + 2] / 255.0f * SCALE_B;
                    p.a = pIn[index * 4 + 3] / 255.0f * SCALE_A;

                    if (pExq.transparency)
                    {
                        p.r *= p.a; p.g *= p.a; p.b *= p.a;
                    }

                    if (pHist == null || pHist.ditherScale.r < 0)
                    {
                        i = FindNearestColor(p);
                        scale.r = pExq.node[i].avg.r - p.r;
                        scale.g = pExq.node[i].avg.g - p.g;
                        scale.b = pExq.node[i].avg.b - p.b;
                        scale.a = pExq.node[i].avg.a - p.a;
                        tmp.r = p.r - scale.r / 3;
                        tmp.g = p.g - scale.g / 3;
                        tmp.b = p.b - scale.b / 3;
                        tmp.a = p.a - scale.a / 3;
                        j = FindNearestColor(tmp);
                        if (i == j)
                        {
                            tmp.r = p.r - scale.r * 3;
                            tmp.g = p.g - scale.g * 3;
                            tmp.b = p.b - scale.b * 3;
                            tmp.a = p.a - scale.a * 3;
                            j = FindNearestColor(tmp);
                        }
                        if (i != j)
                        {
                            scale.r = (pExq.node[j].avg.r - pExq.node[i].avg.r) * 0.8f;
                            scale.g = (pExq.node[j].avg.g - pExq.node[i].avg.g) * 0.8f;
                            scale.b = (pExq.node[j].avg.b - pExq.node[i].avg.b) * 0.8f;
                            scale.a = (pExq.node[j].avg.a - pExq.node[i].avg.a) * 0.8f;
                            if (scale.r < 0) scale.r = -scale.r;
                            if (scale.g < 0) scale.g = -scale.g;
                            if (scale.b < 0) scale.b = -scale.b;
                            if (scale.a < 0) scale.a = -scale.a;
                        }
                        else
                            scale.r = scale.g = scale.b = scale.a = 0;

                        if (pHist != null)
                        {
                            pHist.ditherScale.r = scale.r;
                            pHist.ditherScale.g = scale.g;
                            pHist.ditherScale.b = scale.b;
                            pHist.ditherScale.a = scale.a;
                        }
                    }
                    else
                    {
                        scale.r = pHist.ditherScale.r;
                        scale.g = pHist.ditherScale.g;
                        scale.b = pHist.ditherScale.b;
                        scale.a = pHist.ditherScale.a;
                    }

                    if (pHist != null && pHist.ditherIndex[d] >= 0)
                    {
                        pOut[index] = (byte)pHist.ditherIndex[d];
                    }
                    else
                    {
                        tmp.r = p.r + scale.r * ditherMatrix[d];
                        tmp.g = p.g + scale.g * ditherMatrix[d];
                        tmp.b = p.b + scale.b * ditherMatrix[d];
                        tmp.a = p.a + scale.a * ditherMatrix[d];
                        pOut[index] = FindNearestColor(tmp);
                        if (pHist != null)
                        {
                            pHist.ditherIndex[d] = pOut[index];
                        }
                    }
                }
        }

        private void SumNode(ExqNode pNode)
        {
            int n, n2;
            ExqColor fsum = new ExqColor(), fsum2 = new ExqColor(), vc = new ExqColor(),
                      tmp = new ExqColor(), tmp2 = new ExqColor(),
                      sum = new ExqColor(), sum2 = new ExqColor();
            ExqHistogramEntry pCur;
            ExqFloat isqrt, nv, v;

            n = 0;
            fsum.r = fsum.g = fsum.b = fsum.a = 0;
            fsum2.r = fsum2.g = fsum2.b = fsum2.a = 0;

            for (pCur = pNode.pHistogram; pCur != null; pCur = pCur.pNext)
            {
                n += pCur.num;
                fsum.r += pCur.color.r * pCur.num;
                fsum.g += pCur.color.g * pCur.num;
                fsum.b += pCur.color.b * pCur.num;
                fsum.a += pCur.color.a * pCur.num;
                fsum2.r += pCur.color.r * pCur.color.r * pCur.num;
                fsum2.g += pCur.color.g * pCur.color.g * pCur.num;
                fsum2.b += pCur.color.b * pCur.color.b * pCur.num;
                fsum2.a += pCur.color.a * pCur.color.a * pCur.num;
            }
            pNode.num = n;
            if (n == 0)
            {
                pNode.vdif = 0;
                pNode.err = 0;
                return;
            }

            pNode.avg.r = fsum.r / n;
            pNode.avg.g = fsum.g / n;
            pNode.avg.b = fsum.b / n;
            pNode.avg.a = fsum.a / n;

            vc.r = fsum2.r - fsum.r * pNode.avg.r;
            vc.g = fsum2.g - fsum.g * pNode.avg.g;
            vc.b = fsum2.b - fsum.b * pNode.avg.b;
            vc.a = fsum2.a - fsum.a * pNode.avg.a;

            v = vc.r + vc.g + vc.b + vc.a;
            pNode.err = v;
            pNode.vdif = -v;

            if (vc.r > vc.g && vc.r > vc.b && vc.r > vc.a)
                Sort(ref pNode.pHistogram, SortByRed);
            else if (vc.g > vc.b && vc.g > vc.a)
                Sort(ref pNode.pHistogram, SortByGreen);
            else if (vc.b > vc.a)
                Sort(ref pNode.pHistogram, SortByBlue);
            else
                Sort(ref pNode.pHistogram, SortByAlpha);

            pNode.dir.r = pNode.dir.g = pNode.dir.b = pNode.dir.a = 0;
            for (pCur = pNode.pHistogram; pCur != null; pCur = pCur.pNext)
            {
                tmp.r = (pCur.color.r - pNode.avg.r) * pCur.num;
                tmp.g = (pCur.color.g - pNode.avg.g) * pCur.num;
                tmp.b = (pCur.color.b - pNode.avg.b) * pCur.num;
                tmp.a = (pCur.color.a - pNode.avg.a) * pCur.num;
                if (tmp.r * pNode.dir.r + tmp.g * pNode.dir.g +
                    tmp.b * pNode.dir.b + tmp.a * pNode.dir.a < 0)
                {
                    tmp.r = -tmp.r;
                    tmp.g = -tmp.g;
                    tmp.b = -tmp.b;
                    tmp.a = -tmp.a;
                }
                pNode.dir.r += tmp.r;
                pNode.dir.g += tmp.g;
                pNode.dir.b += tmp.b;
                pNode.dir.a += tmp.a;
            }
            isqrt = 1 / Math.Sqrt(pNode.dir.r * pNode.dir.r +
                pNode.dir.g * pNode.dir.g + pNode.dir.b * pNode.dir.b +
                pNode.dir.a * pNode.dir.a);
            pNode.dir.r *= isqrt;
            pNode.dir.g *= isqrt;
            pNode.dir.b *= isqrt;
            pNode.dir.a *= isqrt;

            sortDir = pNode.dir;
            Sort(ref pNode.pHistogram, SortByDir);

            sum.r = sum.g = sum.b = sum.a = 0;
            sum2.r = sum2.g = sum2.b = sum2.a = 0;
            n2 = 0;
            pNode.pSplit = pNode.pHistogram;
            for (pCur = pNode.pHistogram; pCur != null; pCur = pCur.pNext)
            {
                if (pNode.pSplit == null)
                    pNode.pSplit = pCur;

                n2 += pCur.num;
                sum.r += pCur.color.r * pCur.num;
                sum.g += pCur.color.g * pCur.num;
                sum.b += pCur.color.b * pCur.num;
                sum.a += pCur.color.a * pCur.num;
                sum2.r += pCur.color.r * pCur.color.r * pCur.num;
                sum2.g += pCur.color.g * pCur.color.g * pCur.num;
                sum2.b += pCur.color.b * pCur.color.b * pCur.num;
                sum2.a += pCur.color.a * pCur.color.a * pCur.num;

                if (n == n2)
                    break;

                tmp.r = sum2.r - sum.r * sum.r / n2;
                tmp.g = sum2.g - sum.g * sum.g / n2;
                tmp.b = sum2.b - sum.b * sum.b / n2;
                tmp.a = sum2.a - sum.a * sum.a / n2;
                tmp2.r = (fsum2.r - sum2.r) - (fsum.r - sum.r) * (fsum.r - sum.r) / (n - n2);
                tmp2.g = (fsum2.g - sum2.g) - (fsum.g - sum.g) * (fsum.g - sum.g) / (n - n2);
                tmp2.b = (fsum2.b - sum2.b) - (fsum.b - sum.b) * (fsum.b - sum.b) / (n - n2);
                tmp2.a = (fsum2.a - sum2.a) - (fsum.a - sum.a) * (fsum.a - sum.a) / (n - n2);

                nv = tmp.r + tmp.g + tmp.b + tmp.a + tmp2.r + tmp2.g + tmp2.b + tmp2.a;
                if (-nv > pNode.vdif)
                {
                    pNode.vdif = -nv;
                    pNode.pSplit = null;
                }
            }

            if (pNode.pSplit == pNode.pHistogram)
                pNode.pSplit = pNode.pSplit.pNext;

            pNode.vdif += v;
        }

        private void OptimizePalette(int iter)
        {
            ExqHistogramEntry pCur;

            pExq.optimized = true;

            for (int n = 0; n < iter; n++)
            {
                for (int i = 0; i < pExq.numColors; i++)
                    pExq.node[i].pHistogram = null;

                for (int i = 0; i < EXQ_HASH_SIZE; i++)
                    for (pCur = pExq.pHash[i]; pCur != null; pCur = pCur.pNextInHash)
                    {
                        byte j = FindNearestColor(pCur.color);
                        pCur.pNext = pExq.node[j].pHistogram;
                        pExq.node[j].pHistogram = pCur;
                    }

                for (int i = 0; i < pExq.numColors; i++)
                    SumNode(pExq.node[i]);
            }
        }

        private byte FindNearestColor(ExqColor pColor)
        {
            ExqColor dif = new ExqColor();
            ExqFloat bestv = 16;
            int besti = 0;

            for (int i = 0; i < pExq.numColors; i++)
            {
                dif.r = pColor.r - pExq.node[i].avg.r;
                dif.g = pColor.g - pExq.node[i].avg.g;
                dif.b = pColor.b - pExq.node[i].avg.b;
                dif.a = pColor.a - pExq.node[i].avg.a;
                if (dif.r * dif.r + dif.g * dif.g + dif.b * dif.b + dif.a * dif.a < bestv)
                {
                    bestv = dif.r * dif.r + dif.g * dif.g + dif.b * dif.b + dif.a * dif.a;
                    besti = i;
                }
            }
            return (byte)besti;
        }

        private ExqHistogramEntry FindHistogram(byte[] pCol, int index)
        {
            uint hash;
            ExqHistogramEntry pCur;

            uint r = pCol[index * 4 + 0],
                 g = pCol[index * 4 + 1],
                 b = pCol[index * 4 + 2],
                 a = pCol[index * 4 + 3];

            hash = MakeHash(ToRGBA(r, g, b, a));

            pCur = pExq.pHash[hash];
            while (pCur != null && (pCur.ored != r || pCur.ogreen != g || pCur.oblue != b || pCur.oalpha != a))
            {
                pCur = pCur.pNextInHash;
            }

            return pCur;
        }

        public delegate ExqFloat SortFunction(ExqHistogramEntry pHist);
        private void Sort(ref ExqHistogramEntry ppHist, SortFunction sortfunc)
        {
            ExqHistogramEntry pLow, pHigh, pCur, pNext;
            int n = 0;
            ExqFloat sum = 0;

            for (pCur = ppHist; pCur != null; pCur = pCur.pNext)
            {
                n++;
                sum += sortfunc(pCur);
            }

            if (n < 2)
                return;

            sum /= n;

            pLow = pHigh = null;
            for (pCur = ppHist; pCur != null; pCur = pNext)
            {
                pNext = pCur.pNext;
                if (sortfunc(pCur) < sum)
                {
                    pCur.pNext = pLow;
                    pLow = pCur;
                }
                else
                {
                    pCur.pNext = pHigh;
                    pHigh = pCur;
                }
            }

            if (pLow == null)
            {
                ppHist = pHigh;
                return;
            }
            if (pHigh == null)
            {
                ppHist = pLow;
                return;
            }

            Sort(ref pLow, sortfunc);
            Sort(ref pHigh, sortfunc);

            ppHist = pLow;
            while (pLow.pNext != null)
                pLow = pLow.pNext;

            pLow.pNext = pHigh;
        }

        private ExqFloat SortByRed(ExqHistogramEntry pHist)
        {
            return pHist.color.r;
        }

        private ExqFloat SortByGreen(ExqHistogramEntry pHist)
        {
            return pHist.color.g;
        }

        private ExqFloat SortByBlue(ExqHistogramEntry pHist)
        {
            return pHist.color.b;
        }

        private ExqFloat SortByAlpha(ExqHistogramEntry pHist)
        {
            return pHist.color.a;
        }

        private ExqColor sortDir = new ExqColor();
        private ExqFloat SortByDir(ExqHistogramEntry pHist)
        {
            return pHist.color.r * sortDir.r +
                pHist.color.g * sortDir.g +
                pHist.color.b * sortDir.b +
                pHist.color.a * sortDir.a;
        }

    }
}