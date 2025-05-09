﻿using System;

namespace BCn
{
    public class BC1BlockEncoder
    {
        /// <summary>
        /// Gets or sets whether RGB data will be dithered.
        /// </summary>
        public bool DitherRgb { get; set; }
        public bool UseUniformWeighting { get; set; }

        /// <summary>
        /// Loads a block of color data.
        /// </summary>
        /// <param name="rValues">The array to load red values from.</param>
        /// <param name="rIndex">The index in <paramref name="rValues"/> to start loading from.</param>
        /// <param name="gValues">The array to load green values from.</param>
        /// <param name="gIndex">The index in <paramref name="rValues"/> to start loading from.</param>
        /// <param name="bValues">The array to load blue values from.</param>
        /// <param name="bIndex">The index in <paramref name="rValues"/> to start loading from.</param>
        /// <param name="rowPitch">The number of array elements between successive rows.</param>
        /// <param name="colPitch">The number of array elements between successive pixels.</param>
        public void LoadBlock(
            float[] rValues, int rIndex,
            float[] gValues, int gIndex,
            float[] bValues, int bIndex,
            int rowPitch = 4, int colPitch = 1)
        {
            var target = this.values;

            int rIdx = rIndex;
            int gIdx = gIndex;
            int bIdx = bIndex;

            target[0].R = rValues[rIdx];
            target[1].R = rValues[rIdx += colPitch];
            target[2].R = rValues[rIdx += colPitch];
            target[3].R = rValues[rIdx += colPitch];

            target[0].G = gValues[gIdx];
            target[1].G = gValues[gIdx += colPitch];
            target[2].G = gValues[gIdx += colPitch];
            target[3].G = gValues[gIdx += colPitch];

            target[0].B = bValues[bIdx];
            target[1].B = bValues[bIdx += colPitch];
            target[2].B = bValues[bIdx += colPitch];
            target[3].B = bValues[bIdx += colPitch];

            rIdx = rIndex += rowPitch;
            gIdx = gIndex += rowPitch;
            bIdx = bIndex += rowPitch;

            target[4].R = rValues[rIdx];
            target[5].R = rValues[rIdx += colPitch];
            target[6].R = rValues[rIdx += colPitch];
            target[7].R = rValues[rIdx += colPitch];

            target[4].G = gValues[gIdx];
            target[5].G = gValues[gIdx += colPitch];
            target[6].G = gValues[gIdx += colPitch];
            target[7].G = gValues[gIdx += colPitch];

            target[4].B = bValues[bIdx];
            target[5].B = bValues[bIdx += colPitch];
            target[6].B = bValues[bIdx += colPitch];
            target[7].B = bValues[bIdx += colPitch];

            rIdx = rIndex += rowPitch;
            gIdx = gIndex += rowPitch;
            bIdx = bIndex += rowPitch;

            target[8].R = rValues[rIdx];
            target[9].R = rValues[rIdx += colPitch];
            target[10].R = rValues[rIdx += colPitch];
            target[11].R = rValues[rIdx += colPitch];

            target[8].G = gValues[gIdx];
            target[9].G = gValues[gIdx += colPitch];
            target[10].G = gValues[gIdx += colPitch];
            target[11].G = gValues[gIdx += colPitch];

            target[8].B = bValues[bIdx];
            target[9].B = bValues[bIdx += colPitch];
            target[10].B = bValues[bIdx += colPitch];
            target[11].B = bValues[bIdx += colPitch];

            rIdx = rIndex + rowPitch;
            gIdx = gIndex + rowPitch;
            bIdx = bIndex + rowPitch;

            target[12].R = rValues[rIdx];
            target[13].R = rValues[rIdx += colPitch];
            target[14].R = rValues[rIdx += colPitch];
            target[15].R = rValues[rIdx += colPitch];

            target[12].G = gValues[gIdx];
            target[13].G = gValues[gIdx += colPitch];
            target[14].G = gValues[gIdx += colPitch];
            target[15].G = gValues[gIdx += colPitch];

            target[12].B = bValues[bIdx];
            target[13].B = bValues[bIdx += colPitch];
            target[14].B = bValues[bIdx += colPitch];
            target[15].B = bValues[bIdx += colPitch];
        }

        /// <summary>
        /// Loads a block of color data.
        /// </summary>
        /// <param name="rValues">The array to load red values from.</param>
        /// <param name="gValues">The array to load green values from.</param>
        /// <param name="bValues">The array to load blue values from.</param>
        /// <param name="rowPitch">The number of array elements between successive rows.</param>
        /// <param name="colPitch">The number of array elements between successive pixels.</param>
        public void LoadBlock(
            float[] rValues, float[] gValues, float[] bValues,
            int rowPitch = 4, int colPitch = 1)
        {
            LoadBlock(rValues, 0, gValues, 0, bValues, 0, rowPitch, colPitch);
        }

        /// <summary>
        /// Sets the alpha mask to fully solid.
        /// </summary>
        public void ClearAlphaMask()
        {
            alphaMask = 0;
        }

        /// <summary>
        /// Loads an alpha bitmask.
        /// </summary>
        /// <param name="mask">The bitmask to load.</param>
        /// <remarks>
        /// Set bits denote transparent pixels.
        /// </remarks>
        public void LoadAlphaMask(ushort mask)
        {
            alphaMask = mask;
        }

        public bool DitherAlpha { get; set; }

        /// <summary>
        /// Loads the alpha mask from floating-point alpha values.
        /// </summary>
        /// <param name="aValues">The array to read alpha values from.</param>
        /// <param name="aIndex">The index at which to start reading <paramref name="aValues"/>.</param>
        /// <param name="alphaRef">The alpha reference value. Alpha values smaller than this will be considered transparent.</param>
        /// <param name="rowPitch">The number of array elements between rows.</param>
        /// <param name="colPitch">The number of array elements between pixels within a row.</param>
        public void LoadAlphaMask(
            float[] aValues, int aIndex, float alphaRef = 0.5F,
            int rowPitch = 4, int colPitch = 1)
        {
            alphaMask = 0;

            int aIdx;

            if (DitherAlpha)
            {
                //when dithering, we load into a temporary array,
                //apply dithering, and then continue loading from
                //our temporary storage

                //create the temporary storage the first time it's neeed

                if (alphaValues == null)
                {
                    alphaValues = new float[16];
                    alphaErrors = new float[16];
                }
                else
                {
                    Array.Clear(alphaErrors, 0, 16);
                }

                //load the values

                if (rowPitch == 4 && colPitch == 1)
                {
                    Array.Copy(aValues, aIndex, alphaValues, 0, 16);
                }
                else
                {
                    aIdx = aIndex;

                    alphaValues[0] = aValues[aIdx];
                    alphaValues[1] = aValues[aIdx += colPitch];
                    alphaValues[2] = aValues[aIdx += colPitch];
                    alphaValues[3] = aValues[aIdx += colPitch];

                    aIdx = aIndex += rowPitch;

                    alphaValues[4] = aValues[aIdx];
                    alphaValues[5] = aValues[aIdx += colPitch];
                    alphaValues[6] = aValues[aIdx += colPitch];
                    alphaValues[7] = aValues[aIdx += colPitch];

                    aIdx = aIndex += rowPitch;

                    alphaValues[8] = aValues[aIdx];
                    alphaValues[9] = aValues[aIdx += colPitch];
                    alphaValues[10] = aValues[aIdx += colPitch];
                    alphaValues[11] = aValues[aIdx += colPitch];

                    aIdx = aIndex + rowPitch;

                    alphaValues[12] = aValues[aIdx];
                    alphaValues[13] = aValues[aIdx += colPitch];
                    alphaValues[14] = aValues[aIdx += colPitch];
                    alphaValues[15] = aValues[aIdx += colPitch];
                }

                //apply the dithering

                for (int i = 0; i < alphaValues.Length; i++)
                {
                    var v = alphaValues[i];
                    var e = alphaErrors[i];

                    float a = (int)(v + e);
                    float b = (int)(a + 0.5F);
                    float d = a - b;

                    if ((i & 3) != 3)
                        alphaErrors[i + 1] += d * (7F / 16F);

                    if (i < 12)
                    {
                        if ((i & 3) != 0)
                            alphaErrors[i + 3] += d * (3F / 16F);

                        alphaErrors[i + 4] += d * (5F / 16F);

                        if ((i & 3) != 3)
                            alphaErrors[i + 5] += d * (1F / 16F);
                    }

                    alphaValues[i] = b;
                }

                //and continue loading from our temporary storage

                aValues = alphaValues;
                aIndex = 0;

                rowPitch = 4;
                colPitch = 1;
            }

            aIdx = aIndex;

            if (aValues[aIdx] < alphaRef) alphaMask |= 0x0001;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0002;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0004;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0008;

            aIdx = aIndex += rowPitch;

            if (aValues[aIdx] < alphaRef) alphaMask |= 0x0010;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0020;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0040;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0080;

            aIdx = aIndex += rowPitch;

            if (aValues[aIdx] < alphaRef) alphaMask |= 0x0100;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0200;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0400;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x0800;

            aIdx = aIndex + rowPitch;

            if (aValues[aIdx] < alphaRef) alphaMask |= 0x1000;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x2000;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x4000;
            if (aValues[aIdx += colPitch] < alphaRef) alphaMask |= 0x8000;
        }

        public BC1Block Encode()
        {
            BC1Block ret;

            if (alphaMask == 0xFFFF)
            {
                ret.PackedValue = BC1Block.TransparentValue;
                return ret;
            }

            QuantizeValues();
            
            SpanValues(out RgbF32 r0, out RgbF32 r1);

            //quantize the endpoints

            bool weightValues = !UseUniformWeighting;

            if (weightValues)
            {
                r0.R *= RInvWeight;
                r0.G *= GInvWeight;
                r0.B *= BInvWeight;

                r1.R *= RInvWeight;
                r1.G *= GInvWeight;
                r1.B *= BInvWeight;
            }

            var pr0 = Rgb565.Pack(r0);
            var pr1 = Rgb565.Pack(r1);

            if (alphaMask == 0 && pr0.PackedValue == pr1.PackedValue)
                return new BC1Block(pr0, pr1);

            pr0.Unpack(out r0);
            pr1.Unpack(out r1);

            if (weightValues)
            {
                r0.R *= RWeight;
                r0.G *= GWeight;
                r0.B *= BWeight;

                r1.R *= RWeight;
                r1.G *= GWeight;
                r1.B *= BWeight;
            }

            //interp out the steps

            RgbF32 s0;

            if ((alphaMask != 0) == (pr0.PackedValue <= pr1.PackedValue))
            {
                ret = new BC1Block(pr0, pr1);
                interpValues[0] = s0 = r0;
                interpValues[1] = r1;
            }
            else
            {
                ret = new BC1Block(pr1, pr0);
                interpValues[0] = s0 = r1;
                interpValues[1] = r0;
            }

            uint[] pSteps;

            if (alphaMask != 0)
            {
                pSteps = pSteps3;

                RgbF32.Lerp(out interpValues[2], interpValues[0], interpValues[1], 0.5F);
            }
            else
            {
                pSteps = pSteps4;

                RgbF32.Lerp(out interpValues[2], interpValues[0], interpValues[1], 1.0F / 3.0F);
                RgbF32.Lerp(out interpValues[3], interpValues[0], interpValues[1], 2.0F / 3.0F);
            }

            //find the best values

            RgbF32 dir;

            dir.R = interpValues[1].R - s0.R;
            dir.G = interpValues[1].G - s0.G;
            dir.B = interpValues[1].B - s0.B;

            float fSteps = alphaMask != 0 ? 2 : 3;
            float fScale = (pr0.PackedValue != pr1.PackedValue) ?
                (fSteps / (dir.R * dir.R + dir.G * dir.G + dir.B * dir.B)) : 0.0F;

            dir.R *= fScale;
            dir.G *= fScale;
            dir.B *= fScale;

            bool dither = DitherRgb;

            if (dither)
            {
                if (error == null)
                    error = new RgbF32[16];
                else
                    Array.Clear(error, 0, 16);
            }

            for (int i = 0; i < values.Length; i++)
            {
                if ((alphaMask & (1 << i)) != 0)
                {
                    ret.PackedValue |= 3U << (32 + i * 2);
                }
                else
                {
                    var cl = values[i];

                    if (weightValues)
                    {
                        cl.R *= RWeight;
                        cl.G *= GWeight;
                        cl.B *= BWeight;
                    }

                    if (dither)
                    {
                        var e = error[i];

                        cl.R += e.R;
                        cl.G += e.G;
                        cl.B += e.B;
                    }

                    float fDot =
                        (cl.R - s0.R) * dir.R +
                        (cl.G - s0.G) * dir.G +
                        (cl.B - s0.B) * dir.B;

                    uint iStep;

                    if (fDot <= 0)
                        iStep = 0;
                    else if (fDot >= fSteps)
                        iStep = 1;
                    else
                        iStep = pSteps[(int)(fDot + 0.5F)];

                    ret.PackedValue |= (ulong)iStep << (32 + i * 2);

                    if (dither)
                    {
                        RgbF32 e, d, interp = interpValues[iStep];

                        d.R = cl.R - interp.R;
                        d.G = cl.G - interp.G;
                        d.B = cl.B - interp.B;

                        if ((i & 3) != 3)
                        {
                            e = error[i + 1];

                            e.R += d.R * (7.0F / 16.0F);
                            e.G += d.G * (7.0F / 16.0F);
                            e.B += d.B * (7.0F / 16.0F);

                            error[i + 1] = e;
                        }

                        if (i < 12)
                        {
                            if ((i & 3) != 0)
                            {
                                e = error[i + 3];

                                e.R += d.R * (3.0F / 16.0F);
                                e.G += d.G * (3.0F / 16.0F);
                                e.B += d.B * (3.0F / 16.0F);

                                error[i + 3] = e;
                            }

                            e = error[i + 4];

                            e.R += d.R * (5.0F / 16.0F);
                            e.G += d.G * (5.0F / 16.0F);
                            e.B += d.B * (5.0F / 16.0F);

                            error[i + 4] = e;

                            if (3 != (i & 3))
                            {
                                e = error[i + 5];

                                e.R += d.R * (1.0F / 16.0F);
                                e.G += d.G * (1.0F / 16.0F);
                                e.B += d.B * (1.0F / 16.0F);

                                error[i + 5] = e;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        private RgbF32[] values = new RgbF32[16];
        private RgbF32[] qvalues = new RgbF32[16];
        private RgbF32[] error = new RgbF32[16];

        private float[] alphaValues;
        private float[] alphaErrors;
        private uint alphaMask;

        private float[] fDir = new float[4];

        private RgbF32[] interpValues = new RgbF32[4];

        private void QuantizeValues()
        {
            bool dither = DitherRgb;
            bool weightColors = !UseUniformWeighting;

            if (dither)
                Array.Clear(error, 0, 16);

            for (int i = 0; i < values.Length; i++)
            {
                var cl = values[i];

                if (dither)
                {
                    var e = error[i];

                    cl.R += e.R;
                    cl.G += e.G;
                    cl.B += e.B;
                }

                RgbF32 qcl;

                qcl.R = (int)(cl.R * 31F + 0.5F) * (1F / 31F);
                qcl.G = (int)(cl.G * 63F + 0.5F) * (1F / 63F);
                qcl.B = (int)(cl.B * 31F + 0.5F) * (1F / 31F);

                if (dither)
                {
                    RgbF32 e, d;

                    d.R = cl.R - qcl.R;
                    d.G = cl.G - qcl.G;
                    d.B = cl.B - qcl.B;

                    if ((i & 3) != 3)
                    {
                        e = error[i + 1];

                        e.R += d.R * (7F / 16F);
                        e.G += d.G * (7F / 16F);
                        e.B += d.B * (7F / 16F);

                        error[i + 1] = e;
                    }

                    if (i < 12)
                    {
                        if ((i & 3) != 0)
                        {
                            e = error[i + 3];

                            e.R += d.R * (3F / 16F);
                            e.G += d.G * (3F / 16F);
                            e.B += d.B * (3F / 16F);

                            error[i + 3] = e;
                        }

                        e = error[i + 4];

                        e.R += d.R * (5F / 16F);
                        e.G += d.G * (5F / 16F);
                        e.B += d.B * (5F / 16F);

                        error[i + 4] = e;

                        if ((i & 3) != 3)
                        {
                            e = error[i + 5];

                            e.R += d.R * (1F / 16F);
                            e.G += d.G * (1F / 16F);
                            e.B += d.B * (1F / 16F);

                            error[i + 5] = e;
                        }
                    }
                }

                if (weightColors)
                {
                    qcl.R *= RWeight;
                    qcl.G *= GWeight;
                    qcl.B *= BWeight;
                }

                qvalues[i] = qcl;
            }
        }

        private const float RWeight = 0.2125F / 0.7154F;
        private const float GWeight = 1.0F;
        private const float BWeight = 0.0721F / 0.7154F;

        private const float RInvWeight = 0.7154F / 0.2125F;
        private const float GInvWeight = 1.0F;
        private const float BInvWeight = 0.7154F / 0.0721F;

        private const float fEpsilon = (0.25F / 64.0F) * (0.25F / 64.0F);
        private static readonly float[] pC3 = { 2.0F / 2.0F, 1.0F / 2.0F, 0.0F / 2.0F };
        private static readonly float[] pD3 = { 0.0F / 2.0F, 1.0F / 2.0F, 2.0F / 2.0F };
        private static readonly float[] pC4 = { 3.0F / 3.0F, 2.0F / 3.0F, 1.0F / 3.0F, 0.0F / 3.0F };
        private static readonly float[] pD4 = { 0.0F / 3.0F, 1.0F / 3.0F, 2.0F / 3.0F, 3.0F / 3.0F };
        private static readonly uint[] pSteps3 = { 0, 2, 1 };
        private static readonly uint[] pSteps4 = { 0, 2, 3, 1 };

        private void SpanValues(out RgbF32 r0, out RgbF32 r1)
        {
            bool hasAlpha = alphaMask != 0;

            int cSteps = hasAlpha ? 3 : 4;
            var pC = hasAlpha ? pC3 : pC4;
            var pD = hasAlpha ? pD3 : pD4;

            var values = this.qvalues;

            // Find Min and Max points, as starting point
            RgbF32 X = UseUniformWeighting ? new RgbF32(1, 1, 1) :
                new RgbF32(RWeight, GWeight, BWeight);
            RgbF32 Y = new RgbF32(0, 0, 0);

            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];

#if COLOR_WEIGHTS
				if( (alphaMask & (1 << i)) != 0 )
#endif
                {
                    if (v.R < X.R) X.R = v.R;
                    if (v.G < X.G) X.G = v.G;

                    if (v.B < X.B) X.B = v.B;
                    if (v.R > Y.R) Y.R = v.R;

                    if (v.G > Y.G) Y.G = v.G;
                    if (v.B > Y.B) Y.B = v.B;
                }
            }

            // Diagonal axis
            RgbF32 AB;

            AB.R = Y.R - X.R;
            AB.G = Y.G - X.G;
            AB.B = Y.B - X.B;

            float fAB = AB.R * AB.R + AB.G * AB.G + AB.B * AB.B;

            // Single color block.. no need to root-find
            if (fAB < float.Epsilon)
            {
                r0 = X;
                r1 = Y;
                return;
            }

            // Try all four axis directions, to determine which diagonal best fits data
            float fABInv = 1.0f / fAB;

            RgbF32 Dir;
            Dir.R = AB.R * fABInv;
            Dir.G = AB.G * fABInv;
            Dir.B = AB.B * fABInv;

            RgbF32 Mid;
            Mid.R = (X.R + Y.R) * 0.5f;
            Mid.G = (X.G + Y.G) * 0.5f;
            Mid.B = (X.B + Y.B) * 0.5f;

            fDir[0] = fDir[1] = fDir[2] = fDir[3] = 0.0F;

            for (int i = 0; i < values.Length; i++)
            {
                var v = values[i];

                RgbF32 Pt;
                Pt.R = (v.R - Mid.R) * Dir.R;
                Pt.G = (v.G - Mid.G) * Dir.G;
                Pt.B = (v.B - Mid.B) * Dir.B;

                float f;

#if COLOR_WEIGHTS
				f = Pt.R + Pt.G + Pt.B;
				fDir[0] += v.a * f * f;

				f = Pt.R + Pt.G - Pt.B;
				fDir[1] += v.a * f * f;

				f = Pt.R - Pt.G + Pt.B;
				fDir[2] += v.a * f * f;

				f = Pt.R - Pt.G - Pt.B;
				fDir[3] += v.a * f * f;
#else
                f = Pt.R + Pt.G + Pt.B;
                fDir[0] += f * f;

                f = Pt.R + Pt.G - Pt.B;
                fDir[1] += f * f;

                f = Pt.R - Pt.G + Pt.B;
                fDir[2] += f * f;

                f = Pt.R - Pt.G - Pt.B;
                fDir[3] += f * f;
#endif
            }

            float fDirMax = fDir[0];
            int iDirMax = 0;

            for (int iDir = 1; iDir < fDir.Length; iDir++)
            {
                var d = fDir[iDir];
                if (d > fDirMax)
                {
                    fDirMax = d;
                    iDirMax = iDir;
                }
            }

            if ((iDirMax & 2) != 0)
            {
                float f = X.G; X.G = Y.G; Y.G = f;
            }

            if ((iDirMax & 1) != 0)
            {
                float f = X.B; X.B = Y.B; Y.B = f;
            }


            // Two color block.. no need to root-find
            if (fAB < 1.0f / 4096.0f)
            {
                r0 = X;
                r1 = Y;
                return;
            }

            // Use Newton's Method to find local minima of sum-of-squares error.
            float fSteps = (float)(cSteps - 1);

            for (int iIteration = 0; iIteration < 8; iIteration++)
            {
                // Calculate new steps

                for (int iStep = 0; iStep < cSteps; iStep++)
                {
                    interpValues[iStep].R = X.R * pC[iStep] + Y.R * pD[iStep];
                    interpValues[iStep].G = X.G * pC[iStep] + Y.G * pD[iStep];
                    interpValues[iStep].B = X.B * pC[iStep] + Y.B * pD[iStep];
                }

                // Calculate color direction
                Dir.R = Y.R - X.R;
                Dir.G = Y.G - X.G;
                Dir.B = Y.B - X.B;

                float fLen = (Dir.R * Dir.R + Dir.G * Dir.G + Dir.B * Dir.B);

                if (fLen < (1.0f / 4096.0f))
                    break;

                float fScale = fSteps / fLen;

                Dir.R *= fScale;
                Dir.G *= fScale;
                Dir.B *= fScale;


                // Evaluate function, and derivatives
                float d2X, d2Y;
                RgbF32 dX, dY;
                d2X = d2Y = dX.R = dX.G = dX.B = dY.R = dY.G = dY.B = 0.0f;

                for (int i = 0; i < values.Length; i++)
                {
                    var v = values[i];

                    float fDot = (v.R - X.R) * Dir.R +
                                 (v.G - X.G) * Dir.G +
                                 (v.B - X.B) * Dir.B;

                    int iStep;
                    if (fDot <= 0.0f)
                        iStep = 0;
                    else if (fDot >= fSteps)
                        iStep = cSteps - 1;
                    else
                        iStep = (int)(fDot + 0.5f);


                    RgbF32 Diff;
                    Diff.R = interpValues[iStep].R - v.R;
                    Diff.G = interpValues[iStep].G - v.G;
                    Diff.B = interpValues[iStep].B - v.B;

#if COLOR_WEIGHTS
					float fC = pC[iStep] * v.a * (1.0f / 8.0f);
					float fD = pD[iStep] * v.a * (1.0f / 8.0f);
#else
                    float fC = pC[iStep] * (1.0f / 8.0f);
                    float fD = pD[iStep] * (1.0f / 8.0f);
#endif // COLOR_WEIGHTS

                    d2X += fC * pC[iStep];
                    dX.R += fC * Diff.R;
                    dX.G += fC * Diff.G;
                    dX.B += fC * Diff.B;

                    d2Y += fD * pD[iStep];
                    dY.R += fD * Diff.R;
                    dY.G += fD * Diff.G;
                    dY.B += fD * Diff.B;
                }


                // Move endpoints
                if (d2X > 0.0f)
                {
                    float f = -1.0f / d2X;

                    X.R += dX.R * f;
                    X.G += dX.G * f;
                    X.B += dX.B * f;
                }

                if (d2Y > 0.0f)
                {
                    float f = -1.0f / d2Y;

                    Y.R += dY.R * f;
                    Y.G += dY.G * f;
                    Y.B += dY.B * f;
                }

                if ((dX.R * dX.R < fEpsilon) && (dX.G * dX.G < fEpsilon) && (dX.B * dX.B < fEpsilon) &&
                   (dY.R * dY.R < fEpsilon) && (dY.G * dY.G < fEpsilon) && (dY.B * dY.B < fEpsilon))
                {
                    break;
                }
            }

            r0 = X;
            r1 = Y;
        }
    }


    /// <summary>
    /// The BC1 format.
    /// </summary>
    /// <remarks>
    /// This is also used by the BC2 and BC3 formats.
    /// </remarks>
    [Serializable]
    public struct BC1Block
    {
        public ulong PackedValue;

        public const ulong TransparentValue = 0xFFFFFFFFFFFF0000UL;

        public Rgb565 Rgb0
        {
            get { return new Rgb565((ushort)PackedValue); }
            set { PackedValue = (PackedValue & ~0xFFFFUL) | value.PackedValue; }
        }

        public Rgb565 Rgb1
        {
            get { return new Rgb565((ushort)(PackedValue >> 16)); }
            set { PackedValue = (PackedValue & ~0xFFFF0000UL) | ((ulong)value.PackedValue << 16); }
        }

        public bool HasTransparentValues
        {
            get { return (ushort)PackedValue <= (ushort)(PackedValue >> 16); }
        }

        public BC1Block(Rgb565 r0, Rgb565 r1)
        {
            PackedValue = ((ulong)r1.PackedValue << 16) | r0.PackedValue;
        }

        public void GetPalette(Rgb565[] palette, int index = 0)
        {
            if (palette == null)
                throw new ArgumentNullException("palette");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (palette.Length - index < 4)
                throw new ArgumentOutOfRangeException("index");

            var c0 = Rgb0;
            var c1 = Rgb1;

            palette[index + 0] = c0;
            palette[index + 1] = c1;

            if (HasTransparentValues)
            {
                var interp = new Rgb565();

                interp.R = (c0.R + c1.R) / 2;
                interp.G = (c0.G + c1.G) / 2;
                interp.B = (c0.B + c1.B) / 2;

                palette[index + 2] = interp;

                palette[index + 3] = new Rgb565(0);
            }
            else
            {
                var interp = new Rgb565();

                interp.R = (c0.R * 2 + c1.R) / 3;
                interp.G = (c0.G * 2 + c1.G) / 3;
                interp.B = (c0.B * 2 + c1.B) / 3;

                palette[index + 2] = interp;

                interp.R = (c0.R + c1.R * 2) / 3;
                interp.G = (c0.G + c1.G * 2) / 3;
                interp.B = (c0.B + c1.B * 2) / 3;

                palette[index + 3] = interp;
            }
        }

        public void GetPalette(float[] rPalette, float[] gPalette,
            float[] bPalette, float[] aPalette = null, int index = 0)
        {
            if (rPalette == null)
                throw new ArgumentNullException("rPalette");
            if (gPalette == null)
                throw new ArgumentNullException("gPalette");
            if (bPalette == null)
                throw new ArgumentNullException("bPalette");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (rPalette.Length - index < 4 || gPalette.Length - index < 4 ||
                bPalette.Length - index < 4 || (aPalette != null && aPalette.Length - index < 4))
                throw new ArgumentOutOfRangeException("index");

            var c0 = Rgb0;
            var c1 = Rgb1;

            var r0 = c0.RF;
            var g0 = c0.GF;
            var b0 = c0.BF;

            var r1 = c1.RF;
            var g1 = c1.GF;
            var b1 = c1.BF;

            rPalette[index + 0] = r0;
            gPalette[index + 0] = g0;
            bPalette[index + 0] = b0;

            rPalette[index + 1] = r1;
            gPalette[index + 1] = g1;
            bPalette[index + 1] = b1;

            if (HasTransparentValues)
            {
                rPalette[index + 2] = (r0 + r1) / 2;
                gPalette[index + 2] = (g0 + g1) / 2;
                bPalette[index + 2] = (b0 + b1) / 2;

                rPalette[index + 3] = 0;
                gPalette[index + 3] = 0;
                bPalette[index + 3] = 0;
            }
            else
            {
                rPalette[index + 2] = (r0 * 2 + r1) / 3;
                gPalette[index + 2] = (g0 * 2 + g1) / 3;
                bPalette[index + 2] = (b0 * 2 + b1) / 3;

                rPalette[index + 3] = (r0 + r1 * 2) / 3;
                gPalette[index + 3] = (g0 + g1 * 2) / 3;
                bPalette[index + 3] = (b0 + b1 * 2) / 3;
            }

            if (aPalette != null)
            {
                aPalette[index + 0] = 1;
                aPalette[index + 1] = 1;
                aPalette[index + 2] = 1;
                aPalette[index + 3] = HasTransparentValues ? 0 : 1;
            }
        }

        public void GetIndices(int[] indices, int index = 0)
        {
            if (indices == null)
                throw new ArgumentNullException("indices");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (indices.Length - index < 16)
                throw new ArgumentOutOfRangeException("index");

            uint idxs = (uint)(PackedValue >> 32);
            for (int i = 0; i < 16; i++)
            {
                indices[index + i] = (int)(idxs & 0x3);
                idxs >>= 2;
            }
        }

        /// <summary>
        /// Gets the color index of a given pixel (linear address).
        /// </summary>
        /// <param name="index">The index of the pixel.</param>
        /// <returns>
        /// If <seealso cref="HasTransparentValues"/> is true, index <c>3</c>
        /// is the transparent index.
        /// </returns>
        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= 16)
                    throw new ArgumentOutOfRangeException("index");

                var idx = (int)(PackedValue >> (32 + index * 2));

                return idx;
            }

            set
            {
                if (index < 0 || index >= 16)
                    throw new ArgumentOutOfRangeException("index");
                if (value < 0 || value > 3)
                    throw new ArgumentOutOfRangeException("value");

                int shift = 32 + index * 2;

                PackedValue = (PackedValue & ~(0x3UL << index)) |
                    ((ulong)value << index);
            }
        }

        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= 4)
                    throw new ArgumentOutOfRangeException("x");
                if (y < 0 || y >= 4)
                    throw new ArgumentOutOfRangeException("x");

                return this[y * 4 + x];
            }

            set
            {
                if (x < 0 || x >= 4)
                    throw new ArgumentOutOfRangeException("x");
                if (y < 0 || y >= 4)
                    throw new ArgumentOutOfRangeException("x");

                this[y * 4 + x] = value;
            }
        }
    }


    internal static class Helpers
    {
        public static sbyte FloatToSNorm(float v)
        {
            //clamp

            if (v > 1) v = 1;
            else if (v < -1) v = -1;

            //scale and bias (so the rounding is correct)

            v *= 127;
            v += v >= 0 ? 0.5F : -0.5F;

            //round and return

            return (sbyte)v;
        }

        public static byte FloatToUNorm(float v)
        {
            //clamp

            if (v > 1) return 255;
            if (v < 0) return 0;

            //scale and truncate
            return (byte)(v * 255);
        }
    }

    [System.Runtime.InteropServices.StructLayout(
        System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct RgbF32
    {
        public float R, G, B;

        public RgbF32(float r, float g, float b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public static void Lerp(out RgbF32 o, RgbF32 a, RgbF32 b, float t)
        {
            o.R = a.R + t * (b.R - a.R);
            o.G = a.G + t * (b.G - a.G);
            o.B = a.B + t * (b.B - a.B);
        }

    }


    [Serializable]
    public struct Rgb565
    {
        public ushort PackedValue;

        public Rgb565(ushort packedValue)
        {
            this.PackedValue = packedValue;
        }

        public Rgb565(int r, int g, int b)
        {
            PackedValue = (ushort)((r << 11) | (g << 5) | b);
        }

        public int R
        {
            get { return PackedValue >> 11; }
            set { PackedValue = (ushort)((PackedValue & 0x07FF) | (value << 11)); }
        }

        public int G
        {
            get { return (PackedValue >> 5) & 0x3F; }
            set { PackedValue = (ushort)((PackedValue & 0xF81F) | ((value & 0x3F) << 5)); }
        }

        public int B
        {
            get { return PackedValue & 0x1F; }
            set { PackedValue = (ushort)((PackedValue & 0xFFE0) | (value & 0x1F)); }
        }

        public float RF
        {
            get { return R / 31F; }
            set
            {
                int iv;

                if (value <= 0) iv = 0;
                else if (value >= 1) iv = 31;
                else iv = (int)(value * 31 + 0.5F);

                R = iv;
            }
        }

        public float GF
        {
            get { return G / 63F; }
            set
            {
                int iv;

                if (value <= 0) iv = 0;
                else if (value >= 1) iv = 63;
                else iv = (int)(value * 63 + 0.5F);

                G = iv;
            }
        }

        public float BF
        {
            get { return B / 31F; }
            set
            {
                int iv;

                if (value <= 0) iv = 0;
                else if (value >= 1) iv = 31;
                else iv = (int)(value * 31 + 0.5F);

                B = iv;
            }
        }

        public static Rgb565 Pack(float r, float g, float b)
        {
            int ir, ig, ib;

            if (r <= 0) ir = 0;
            else if (r >= 1) ir = 31;
            else ir = (int)(r * 31 + 0.5F);

            if (g <= 0) ig = 0;
            else if (g >= 1) ig = 63;
            else ig = (int)(g * 63 + 0.5F);

            if (b <= 0) ib = 0;
            else if (b >= 1) ib = 31;
            else ib = (int)(b * 31 + 0.5F);

            return new Rgb565(ir, ig, ib);
        }

        internal static Rgb565 Pack(RgbF32 cl)
        {
            return Pack(cl.R, cl.G, cl.B);
        }

        internal void Unpack(out RgbF32 cl)
        {
            cl.R = RF;
            cl.G = GF;
            cl.B = BF;
        }
    }
}