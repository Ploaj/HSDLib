using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modlee.Rendering.Tristripper
{
    public class RadixSort
    {

        private ulong[] mHistogram;                 // Counters for each byte
        private ulong[] mOffset;                    // Offsets (nearly a cumulative distribution function)

        private ulong mCurrentSize;                // Current size of the indices list
        private ulong[] mIndices;                   // Two lists, swapped each pass
        private ulong[] mIndices2;

        public ulong[] GetIndices()
        {
            return mIndices;
        }

        public RadixSort()
        {
            // Initialize
            mIndices = null;
            mIndices2 = null;
            mCurrentSize = 0;

            // Allocate input-independent ram
            mHistogram = new ulong[256 * 4];
            mOffset = new ulong[256];

            // Initialize indices
            ResetIndices();
        }

        public RadixSort ResetIndices()
        {
            for (ulong i = 0; i < mCurrentSize; i++)
            {
                mIndices[i] = i;
            }
            return this;
        }

        public RadixSort Sort(ulong[] input, ulong nb, bool signedvalues = true)
        {
            // Resize lists if needed
            if (nb > mCurrentSize)
            {
                // Get some fresh one
                mIndices = new ulong[nb];
                mIndices2 = new ulong[nb];
                mCurrentSize = nb;

                // Initialize indices so that the input buffer is read in sequential order
                ResetIndices();
            }

            // Create histograms (counters). Counters for all passes are created in one run.
            // Pros:	read input buffer once instead of four times
            // Cons:	mHistogram is 4Kb instead of 1Kb
            // We must take care of signed/unsigned values for temporal coherence.... I just
            // have 2 code paths even if just a single opcode changes. Self-modifying code, someone?

            // Temporal coherence
            bool AlreadySorted = true;                      // Optimism...
            ulong[] Indices = mIndices;
            int IndIndex = 0;
            // Prepare to count
            ulong p = 0;//ubyte* p = (ubyte*)input;
            ulong pe = input[(int)p + (int)nb];//ubyte* pe = &p[nb * 4];
            ulong h0 = 0;                        // Histogram for first pass (LSB)
            ulong h1 = 256;                  // Histogram for second pass
            ulong h2 = 512;                  // Histogram for third pass
            ulong h3 = 768;                  // Histogram for last pass (MSB)
            if (!signedvalues)
            {
                // Temporal coherence
                ulong PrevVal = input[mIndices[0]];

                while (p != pe)
                {
                    // Temporal coherence
                    ulong Val = input[IndIndex++];              // Read input buffer in previous sorted order
                    if (Val < PrevVal) AlreadySorted = false;       // Check whether already sorted or not
                    PrevVal = Val;                              // Update for next iteration

                    // Create histograms
                    mHistogram[(int)(h0 + input[(int)p++])]++;
                    mHistogram[(int)(h1 + input[(int)p++])]++;
                    mHistogram[(int)(h2 + input[(int)p++])]++;
                    mHistogram[(int)(h3 + input[(int)p++])]++;
                }
            }
            else
            {
                // Temporal coherence
                long PrevVal = (long)input[mIndices[0]];

                while (p != pe)
                {
                    // Temporal coherence
                    long Val = (long)input[Indices[IndIndex++]];     // Read input buffer in previous sorted order
                    if (Val < PrevVal) AlreadySorted = false;       // Check whether already sorted or not
                    PrevVal = Val;                              // Update for next iteration

                    // Create histograms
                    //h0[*p++]++; h1[*p++]++; h2[*p++]++; h3[*p++]++;

                    mHistogram[(int)(h0 + input[(int)p++])]++;
                    mHistogram[(int)(h1 + input[(int)p++])]++;
                    mHistogram[(int)(h2 + input[(int)p++])]++;
                    mHistogram[(int)(h3 + input[(int)p++])]++;
                }
            }

            // If all input values are already sorted, we just have to return and leave the previous list unchanged.
            // That way the routine may take advantage of temporal coherence, for example when used to sort transparent faces.
            if (AlreadySorted) return this;

            // Compute #negative values involved if needed
            ulong NbNegativeValues = 0;
            if (signedvalues)
            {
                // An efficient way to compute the number of negatives values we'll have to deal with is simply to sum the 128
                // last values of the last histogram. Last histogram because that's the one for the Most Significant Byte,
                // responsible for the sign. 128 last values because the 128 first ones are related to positive numbers.
                h3 = 768;
                for (ulong i = 128; i < 256; i++) NbNegativeValues += mHistogram[h3 + i];    // 768 for last histogram, 128 for negative part
            }

            // Radix sort, j is the pass number (0=LSB, 3=MSB)
            for (ulong j = 0; j < 4; j++)
            {
                // Shortcut to current counters
                ulong CurCount = j << 8;

                // Reset flag. The sorting pass is supposed to be performed. (default)
                bool PerformPass = true;

                // Check pass validity [some cycles are lost there in the generic case, but that's ok, just a little loop]
                for (ulong i = 0; i < 256; i++)
                {
                    // If all values have the same byte, sorting is useless. It may happen when sorting bytes or words instead of dwords.
                    // This routine actually sorts words faster than dwords, and bytes faster than words. Standard running time (O(4*n))is
                    // reduced to O(2*n) for words and O(n) for bytes. Running time for floats depends on actual values...
                    if (mHistogram[CurCount + i] == nb)
                    {
                        PerformPass = false;
                        break;
                    }
                    // If at least one count is not null, we suppose the pass must be done. Hence, this test takes very few CPU time in the generic case.
                    if (mHistogram[CurCount + i] == 0) break;
                }

                // Sometimes the fourth (negative) pass is skipped because all numbers are negative and the MSB is 0xFF (for example). This is
                // not a problem, numbers are correctly sorted anyway.
                if (PerformPass)
                {
                    // Should we care about negative values?
                    if (j != 3 || !signedvalues)
                    {
                        // Here we deal with positive values only

                        // Create offsets
                        mOffset[0] = 0;
                        for (ulong i = 1; i < 256; i++) mOffset[i] = mOffset[i - 1] + mHistogram[CurCount + i - 1];
                    }
                    else
                    {
                        // This is a special case to correctly handle negative integers. They're sorted in the right order but at the wrong place.

                        // Create biased offsets, in order for negative numbers to be sorted as well
                        mOffset[0] = NbNegativeValues;                                              // First positive number takes place after the negative ones
                        for (ulong i = 1; i < 128; i++) mOffset[i] = mOffset[i - 1] + mHistogram[CurCount + i - 1];  // 1 to 128 for positive numbers

                        // Fixing the wrong place for negative values
                        mOffset[128] = 0;
                        for (int i = 129; i < 256; i++) mOffset[i] = mOffset[i - 1] + mHistogram[(int)CurCount + i - 1];
                    }

                    // Perform Radix Sort
                    {
                        int InputBytes = 0;// ubyte* InputBytes = (ubyte*)input;
                        IndIndex = 0;// ulong[] Indices = mIndices;
                        ulong IndicesEnd = mIndices[nb];
                        InputBytes += (int)j;
                        while (IndIndex != (int)IndicesEnd)
                        {
                            ulong id = Indices[IndIndex++];
                            mIndices2[mOffset[input[(int)InputBytes + (int)(id << 2)]]++] = id;
                        }
                    }

                    // Swap pointers for next pass
                    ulong[] Tmp = mIndices;
                    mIndices = mIndices2;
                    mIndices2 = Tmp;
                }
            }
            return this;
        }
    }
}
