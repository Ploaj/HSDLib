using OpenTK;
using System;
using System.Linq;
using VGAudio.Codecs.GcAdpcm;

namespace HSDRawViewer.Tools
{
    // code adapted from https://github.com/jackoalan/gc-dspadpcm-encode/blob/039712baa1291fbd77a1390e0496757122efd81b/grok.c
    /// <summary>
    /// 
    /// </summary>
    public class DSPEncoder
    {
        /// <summary>
        /// Takes 16 bps sound sample
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodeDSP(short[] data, out short[] coefs)
        {
            var coeffs = DSPCorrelateCoefs(data, data.Length / 16);

            coefs = coeffs;

            return GcAdpcmEncoder.Encode(data, coeffs);
        }

        private static short[] DSPCorrelateCoefs(short[] source, int samples)
        {
            int srcIndex = 0;
            var coefsOut = new short[0x10];
            int numFrames = (samples + 13) / 14;
            int frameSamples;

            short[] blockBuffer = new short[0x3800];
            short[,] pcmHistBuffer = new short[2, 14];

            Vector3 vec1 = Vector3.Zero;
            Vector3 vec2 = Vector3.Zero;

            Vector3[] mtx = new Vector3[3];
            int[] vecIdxs = new int[3];

            Vector3[] records = new Vector3[numFrames * 2];
            int recordCount = 0;

            Vector3[] vecBest = new Vector3[8];

            /* Iterate though 1024-block frames */
            for (int x = samples; x > 0;)
            {
                if (x > 0x3800) /* Full 1024-block frame */
                {
                    frameSamples = 0x3800;
                    x -= 0x3800;
                }
                else /* Partial frame */
                {
                    /* Zero lingering block samples */
                    frameSamples = x;
                    for (int z = 0; z < 14 && z + frameSamples < 0x3800; z++)
                        blockBuffer[frameSamples + z] = 0;
                    x = 0;
                }

                /* Copy (potentially non-frame-aligned PCM samples into aligned buffer) */
                //memcpy(blockBuffer, source, frameSamples* sizeof(short));
                //source += frameSamples;
                blockBuffer = new short[frameSamples];
                Array.Copy(source, srcIndex, blockBuffer, 0, frameSamples);
                srcIndex += frameSamples;

                for (int i = 0; i < frameSamples;)
                {
                    for (int z = 0; z < 14; z++)
                        pcmHistBuffer[0, z] = pcmHistBuffer[1, z];
                    for (int z = 0; z < 14; z++)
                    {
                        if (i >= frameSamples)
                            break;
                        pcmHistBuffer[1, z] = blockBuffer[i++];
                    }

                    InnerProductMerge(GetColumn(pcmHistBuffer, 1), out vec1);
                    if (Math.Abs(vec1[0]) > 10.0)
                    {
                        OuterProductMerge(GetColumn(pcmHistBuffer, 1), out mtx);
                        if (!AnalyzeRanges(mtx, ref vecIdxs))
                        {
                            BidirectionalFilter(ref mtx, ref vecIdxs, out vec1);
                            if (!QuadraticMerge(ref vec1))
                            {
                                FinishRecord(vec1, out records[recordCount]);
                                recordCount++;
                            }
                        }
                    }
                }
            }

            vec1[0] = 1.0f;
            vec1[1] = 0.0f;
            vec1[2] = 0.0f;

            for (int z = 0; z < recordCount; z++)
            {
                MatrixFilter(ref records[z], ref vecBest[0]);
                for (int y = 1; y <= 2; y++)
                    vec1[y] += vecBest[0][y];
            }
            for (int y = 1; y <= 2; y++)
                vec1[y] /= recordCount;

            MergeFinishRecord(ref vec1, ref vecBest[0]);


            int exp = 1;
            for (int w = 0; w < 3;)
            {
                vec2[0] = 0.0f;
                vec2[1] = -1.0f;
                vec2[2] = 0.0f;
                for (int i = 0; i < exp; i++)
                    for (int y = 0; y <= 2; y++)
                        vecBest[exp + i][y] = (0.01f * vec2[y]) + vecBest[i][y];
                ++w;
                exp = 1 << w;
                FilterRecords(ref vecBest, exp, ref records, recordCount);
            }

            /* Write output */
            for (int z = 0; z < 8; z++)
            {
                double d;
                d = -vecBest[z][1] * 2048.0;
                if (d > 0.0)
                    coefsOut[z * 2] = (d > 32767.0) ? (short)32767 : (short)Math.Round(d);
                else
                    coefsOut[z * 2] = (d < -32768.0) ? (short)-32768 : (short)Math.Round(d);

                d = -vecBest[z][2] * 2048.0;
                if (d > 0.0)
                    coefsOut[z * 2 + 1] = (d > 32767.0) ? (short)32767 : (short)Math.Round(d);
                else
                    coefsOut[z * 2 + 1] = (d < -32768.0) ? (short)-32768 : (short)Math.Round(d);
            }

            return coefsOut;
        }


        private static T[] GetColumn<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }


        private static void InnerProductMerge(short[] pcmBuf, out Vector3 vecOut)
        {
            vecOut = Vector3.Zero;
            for (int i = 0; i <= 2; i++)
            {
                vecOut[i] = 0.0f;
                for (int x = 0; x < 14; x++)
                    if (x - i >= 0)
                        vecOut[i] -= pcmBuf[x - i] * pcmBuf[x];
            }
        }

        private static void OuterProductMerge(short[] pcmBuf, out Vector3[] mtxOut)
        {
            mtxOut = new Vector3[3];
            for (int x = 1; x <= 2; x++)
                for (int y = 1; y <= 2; y++)
                {
                    mtxOut[x][y] = 0.0f;
                    for (int z = 0; z < 14; z++)
                        if (z - x >= 0 && z - y >= 0)
                            mtxOut[x][y] += pcmBuf[z - x] * pcmBuf[z - y];
                }
        }

        private static double DBL_EPSILON = 4.94065645841247e-324;

        private static bool AnalyzeRanges(Vector3[] mtx, ref int[] vecIdxsOut)
        {
            double[] recips = new double[3];
            double val, tmp, min, max;

            /* Get greatest distance from zero */
            for (int x = 1; x <= 2; x++)
            {
                val = Math.Max(Math.Abs(mtx[x][1]), Math.Abs(mtx[x][2]));
                if (val < DBL_EPSILON)
                    return true;

                recips[x] = 1.0 / val;
            }

            int maxIndex = 0;
            for (int i = 1; i <= 2; i++)
            {
                for (int x = 1; x < i; x++)
                {
                    tmp = mtx[x][i];
                    for (int y = 1; y < x; y++)
                        tmp -= mtx[x][y] * mtx[y][i];
                    mtx[x][i] = (float)tmp;
                }

                val = 0.0;
                for (int x = i; x <= 2; x++)
                {
                    tmp = mtx[x][i];
                    for (int y = 1; y < i; y++)
                        tmp -= mtx[x][y] * mtx[y][i];

                    mtx[x][i] = (float)tmp;
                    tmp = Math.Abs(tmp) * recips[x];
                    if (tmp >= val)
                    {
                        val = tmp;
                        maxIndex = x;
                    }
                }

                if (maxIndex != i)
                {
                    for (int y = 1; y <= 2; y++)
                    {
                        tmp = mtx[maxIndex][y];
                        mtx[maxIndex][y] = mtx[i][y];
                        mtx[i][y] = (float)tmp;
                    }
                    recips[maxIndex] = recips[i];
                }

                vecIdxsOut[i] = maxIndex;

                if (mtx[i][i] == 0.0)
                    return true;

                if (i != 2)
                {
                    tmp = 1.0 / mtx[i][i];
                    for (int x = i + 1; x <= 2; x++)
                        mtx[x][i] *= (float)tmp;
                }
            }

            /* Get range */
            min = 1.0e10;
            max = 0.0;
            for (int i = 1; i <= 2; i++)
            {
                tmp = Math.Abs(mtx[i][i]);
                if (tmp < min)
                    min = tmp;
                if (tmp > max)
                    max = tmp;
            }

            if (min / max < 1.0e-10)
                return true;

            return false;
        }

        private static void BidirectionalFilter(ref Vector3[] mtx, ref int[] vecIdxs, out Vector3 vecOut)
        {
            double tmp;

            vecOut = Vector3.Zero;

            for (int i = 1, x = 0; i <= 2; i++)
            {
                int index = vecIdxs[i];
                tmp = vecOut[index];
                vecOut[index] = vecOut[i];
                if (x != 0)
                    for (int y = x; y <= i - 1; y++)
                        tmp -= vecOut[y] * mtx[i][y];
                else if (tmp != 0.0)
                    x = i;
                vecOut[i] = (float)tmp;
            }

            for (int i = 2; i > 0; i--)
            {
                tmp = vecOut[i];
                for (int y = i + 1; y <= 2; y++)
                    tmp -= vecOut[y] * mtx[i][y];
                vecOut[i] = (float)tmp / mtx[i][i];
            }

            vecOut[0] = 1.0f;
        }


        private static bool QuadraticMerge(ref Vector3 inOutVec)
        {
            double v0, v1, v2 = inOutVec[2];
            double tmp = 1.0 - (v2 * v2);

            if (tmp == 0.0)
                return true;

            v0 = (inOutVec[0] - (v2 * v2)) / tmp;
            v1 = (inOutVec[1] - (inOutVec[1] * v2)) / tmp;

            inOutVec[0] = (float)v0;
            inOutVec[1] = (float)v1;

            return Math.Abs(v1) > 1.0;
        }


        private static void FinishRecord(Vector3 vin, out Vector3 vout)
        {
            vout = Vector3.Zero;
            for (int z = 1; z <= 2; z++)
            {
                if (vin[z] >= 1.0)
                    vin[z] = 0.9999999999f;
                else if (vin[z] <= -1.0)
                    vin[z] = -0.9999999999f;
            }
            vout[0] = 1.0f;
            vout[1] = (vin[2] * vin[1]) + vin[1];
            vout[2] = vin[2];
        }

        private static void MatrixFilter(ref Vector3 src, ref Vector3 dst)
        {
            Vector3[] mtx = new Vector3[3];

            mtx[2][0] = 1.0f;
            for (int i = 1; i <= 2; i++)
                mtx[2][i] = -src[i];

            for (int i = 2; i > 0; i--)
            {
                double val = 1.0 - (mtx[i][i] * mtx[i][i]);
                for (int y = 1; y <= i; y++)
                    mtx[i - 1][y] = (float)(((mtx[i][i] * mtx[i][y]) + mtx[i][y]) / val);
            }

            dst[0] = 1.0f;
            for (int i = 1; i <= 2; i++)
            {
                dst[i] = 0.0f;
                for (int y = 1; y <= i; y++)
                    dst[i] += mtx[i][y] * dst[i - y];
            }
        }

        private static void MergeFinishRecord(ref Vector3 src, ref Vector3 dst)
        {
            Vector3 tmp = Vector3.Zero;
            double val = src[0];

            dst[0] = 1.0f;
            for (int i = 1; i <= 2; i++)
            {
                double v2 = 0.0;
                for (int y = 1; y < i; y++)
                    v2 += dst[y] * src[i - y];

                if (val > 0.0)
                    dst[i] = (float)(-(v2 + src[i]) / val);
                else
                    dst[i] = 0.0f;

                tmp[i] = dst[i];

                for (int y = 1; y < i; y++)
                    dst[y] += dst[i] * dst[i - y];

                val *= 1.0 - (dst[i] * dst[i]);
            }

            FinishRecord(tmp, out dst);
        }


        private static double ContrastVectors(ref Vector3 source1, ref Vector3 source2)
        {
            double val = (source2[2] * source2[1] + -source2[1]) / (1.0 - source2[2] * source2[2]);
            double val1 = (source1[0] * source1[0]) + (source1[1] * source1[1]) + (source1[2] * source1[2]);
            double val2 = (source1[0] * source1[1]) + (source1[1] * source1[2]);
            double val3 = source1[0] * source1[2];
            return val1 + (2.0 * val * val2) + (2.0 * (-source2[1] * val + -source2[2]) * val3);
        }

        private static void FilterRecords(ref Vector3[] vecBest, int exp, ref Vector3[] records, int recordCount)
        {
            Vector3[] bufferList = new Vector3[8];

            int[] buffer1 = new int[8];
            Vector3 buffer2 = Vector3.Zero;

            int index;
            double value, tempVal = 0;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < exp; y++)
                {
                    buffer1[y] = 0;
                    for (int i = 0; i <= 2; i++)
                        bufferList[y][i] = 0.0f;
                }
                for (int z = 0; z < recordCount; z++)
                {
                    index = 0;
                    value = 1.0e30;
                    for (int i = 0; i < exp; i++)
                    {
                        tempVal = ContrastVectors(ref vecBest[i], ref records[z]);
                        if (tempVal < value)
                        {
                            value = tempVal;
                            index = i;
                        }
                    }
                    buffer1[index]++;
                    MatrixFilter(ref records[z], ref buffer2);
                    for (int i = 0; i <= 2; i++)
                        bufferList[index][i] += buffer2[i];
                }

                for (int i = 0; i < exp; i++)
                    if (buffer1[i] > 0)
                        for (int y = 0; y <= 2; y++)
                            bufferList[i][y] /= buffer1[i];

                for (int i = 0; i < exp; i++)
                    MergeFinishRecord(ref bufferList[i], ref vecBest[i]);
            }
        }

    }
}
