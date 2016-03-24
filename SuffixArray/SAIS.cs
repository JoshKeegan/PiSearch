/*
    PiSearch
    SAIS - Implements the IS based linear suffix array consturction algorithm described in the paper:
        Ge Nong, Sen Zhang and Wai Hong Chan
        Two Efficient Algorithms for Linear Suffix Array Construction
        2008
        IEEE Xplore: http://ieeexplore.ieee.org/xpls/abs_all.jsp?arnumber=5582081
        Hong Kong Institute of Education: http://libir1.ied.edu.hk/pubdata/ir/link/pub/TC-2009-04-0165-Final.pdf
    Originally by Yuta Mori <yuta.256@gmail.com> (2010)
    Implemented an array of 4 bit unsigned integers (with one reserved value) for use with the PiSearch project by Josh Keegan, 2014
    Moved into main PiSearch project by Josh Keegan 24/03/2016. Separate respository still exists at https://github.com/JoshKeegan/SAIS-CSharp

    This file is under the license specified by Yuta Mori (below), not the same license as is used elsewhere in the PiSearch project.
*/

/*
 * SAIS.cs for SAIS-CSharp
 * Copyright (c) 2010 Yuta Mori. All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.IO;

using StringSearch.Collections;

namespace SuffixArray
{

    internal interface BaseArray
    {
        long this[long i]
        {
            set;
            get;
        }
    }

    internal class FourBitDigitStreamArray : BaseArray
    {
        private FourBitDigitBigArray m_array;
        private long m_pos;

        public FourBitDigitStreamArray(FourBitDigitBigArray array, long pos)
        {
            m_array = array;
            m_pos = pos;
        }

        ~FourBitDigitStreamArray() { m_array = null; }

        public long this[long i]
        {
            get
            {
                return (long)m_array[i + m_pos];
            }
            set
            {
                m_array[i + m_pos] = (byte)value;
            }
        }
    }

    internal class LongArray : BaseArray
    {
        //Use a ulong array internally, as it will never contain -ve values
        private BigArray<ulong> m_array;
        private long m_pos;

        public LongArray(BigArray<ulong> array, long pos)
        {
            m_array = array;
            m_pos = pos;
        }

        public LongArray(LongArray array, long pos)
        {
            m_array = array.m_array;
            m_pos = array.m_pos + pos;
        }

        ~LongArray() { m_array = null; }

        public long this[long i]
        {
            get
            {
                return (long)m_array[i + m_pos];
            }
            set
            {
                m_array[i + m_pos] = (ulong)value;
            }
        }
    }

    internal class StringArray : BaseArray
    {
        private string m_array;
        private int m_pos;

        public StringArray(string array, long pos)
        {
            if (pos > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("StringArray only supports up to int.MaxValue elements");
            }

            m_array = array;
            m_pos = (int)pos;
        }

        ~StringArray() { m_array = null; }

        public long this[long i]
        {
            get
            {
                return (long)m_array[(int)i + m_pos];
            }
            set { }
        }
    }

    /// <summary>
    /// An implementation of the induced sorting based suffix array construction algorithm.
    /// </summary>
    public static class SAIS
    {
        private const long MINBUCKETSIZE = 256;

        private static
        void
        getCounts(BaseArray T, BaseArray C, long n, long k)
        {
            long i;
            for (i = 0; i < k; ++i) { C[i] = 0; }
            for (i = 0; i < n; ++i) { C[T[i]] = C[T[i]] + 1; }
        }
        private static
        void
        getBuckets(BaseArray C, BaseArray B, long k, bool end)
        {
            long i, sum = 0;
            if (end != false) { for (i = 0; i < k; ++i) { sum += C[i]; B[i] = sum; } }
            else { for (i = 0; i < k; ++i) { sum += C[i]; B[i] = sum - C[i]; } }
        }

        /* sort all type LMS suffixes */
        private static
        void
        LMSsort(BaseArray T, LongArray SA, BaseArray C, BaseArray B, long n, long k)
        {
            long b, i, j;
            long c0, c1;
            /* compute SAl */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, false); /* find starts of buckets */
            j = n - 1;
            b = B[c1 = T[j]];
            --j;
            SA[b++] = (T[j] < c1) ? ~j : j;
            for (i = 0; i < n; ++i)
            {
                if (0 < (j = SA[i]))
                {
                    if ((c0 = T[j]) != c1) { B[c1] = b; b = B[c1 = c0]; }
                    --j;
                    SA[b++] = (T[j] < c1) ? ~j : j;
                    SA[i] = 0;
                }
                else if (j < 0)
                {
                    SA[i] = ~j;
                }
            }
            /* compute SAs */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, true); /* find ends of buckets */
            for (i = n - 1, b = B[c1 = 0]; 0 <= i; --i)
            {
                if (0 < (j = SA[i]))
                {
                    if ((c0 = T[j]) != c1) { B[c1] = b; b = B[c1 = c0]; }
                    --j;
                    SA[--b] = (T[j] > c1) ? ~(j + 1) : j;
                    SA[i] = 0;
                }
            }
        }
        private static
        long
        LMSpostproc(BaseArray T, LongArray SA, long n, long m)
        {
            long i, j, p, q, plen, qlen, name;
            long c0, c1;
            bool diff;

            /* compact all the sorted substrings into the first m items of SA
                2*m must be not larger than n (proveable) */
            for (i = 0; (p = SA[i]) < 0; ++i) { SA[i] = ~p; }
            if (i < m)
            {
                for (j = i, ++i; ; ++i)
                {
                    if ((p = SA[i]) < 0)
                    {
                        SA[j++] = ~p; SA[i] = 0;
                        if (j == m) { break; }
                    }
                }
            }

            /* store the length of all substrings */
            i = n - 1; j = n - 1; c0 = T[n - 1];
            do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
            for (; 0 <= i;)
            {
                do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) <= c1));
                if (0 <= i)
                {
                    SA[m + ((i + 1) >> 1)] = j - i; j = i + 1;
                    do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                }
            }

            /* find the lexicographic names of all substrings */
            for (i = 0, name = 0, q = n, qlen = 0; i < m; ++i)
            {
                p = SA[i]; plen = SA[m + (p >> 1)]; diff = true;
                if ((plen == qlen) && ((q + plen) < n))
                {
                    for (j = 0; (j < plen) && (T[p + j] == T[q + j]); ++j) { }
                    if (j == plen) { diff = false; }
                }
                if (diff != false) { ++name; q = p; qlen = plen; }
                SA[m + (p >> 1)] = name;
            }

            return name;
        }

        /* compute SA and BWT */
        private static
        void
        induceSA(BaseArray T, LongArray SA, BaseArray C, BaseArray B, long n, long k)
        {
            long b, i, j;
            long c0, c1;
            /* compute SAl */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, false); /* find starts of buckets */
            j = n - 1;
            b = B[c1 = T[j]];
            SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
            for (i = 0; i < n; ++i)
            {
                j = SA[i]; SA[i] = ~j;
                if (0 < j)
                {
                    if ((c0 = T[--j]) != c1) { B[c1] = b; b = B[c1 = c0]; }
                    SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
                }
            }
            /* compute SAs */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, true); /* find ends of buckets */
            for (i = n - 1, b = B[c1 = 0]; 0 <= i; --i)
            {
                if (0 < (j = SA[i]))
                {
                    if ((c0 = T[--j]) != c1) { B[c1] = b; b = B[c1 = c0]; }
                    SA[--b] = ((j == 0) || (T[j - 1] > c1)) ? ~j : j;
                }
                else
                {
                    SA[i] = ~j;
                }
            }
        }
        private static
        long
        computeBWT(BaseArray T, LongArray SA, BaseArray C, BaseArray B, long n, long k)
        {
            long b, i, j, pidx = -1;
            long c0, c1;
            /* compute SAl */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, false); /* find starts of buckets */
            j = n - 1;
            b = B[c1 = T[j]];
            SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
            for (i = 0; i < n; ++i)
            {
                if (0 < (j = SA[i]))
                {
                    SA[i] = ~(c0 = T[--j]);
                    if (c0 != c1) { B[c1] = b; b = B[c1 = c0]; }
                    SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
                }
                else if (j != 0)
                {
                    SA[i] = ~j;
                }
            }
            /* compute SAs */
            if (C == B) { getCounts(T, C, n, k); }
            getBuckets(C, B, k, true); /* find ends of buckets */
            for (i = n - 1, b = B[c1 = 0]; 0 <= i; --i)
            {
                if (0 < (j = SA[i]))
                {
                    SA[i] = (c0 = T[--j]);
                    if (c0 != c1) { B[c1] = b; b = B[c1 = c0]; }
                    SA[--b] = ((0 < j) && (T[j - 1] > c1)) ? ~((int)T[j - 1]) : j;
                }
                else if (j != 0)
                {
                    SA[i] = ~j;
                }
                else
                {
                    pidx = i;
                }
            }
            return pidx;
        }

        /* find the suffix array SA of T[0..n-1] in {0..k-1}^n
           use a working space (excluding T and SA) of at most 2n+O(1) for a constant alphabet */
        private static
        long
        sais_main(BaseArray T, LongArray SA, long fs, long n, long k, bool isbwt)
        {
            BaseArray C, B, RA;
            long i, j, b, m, p, q, name, pidx = 0, newfs;
            long c0, c1;
            ulong flags = 0;

            if (k <= MINBUCKETSIZE)
            {
                C = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0);
                if (k <= fs) { B = new LongArray(SA, n + fs - k); flags = 1; }
                else { B = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0); flags = 3; }
            }
            else if (k <= fs)
            {
                C = new LongArray(SA, n + fs - k);
                if (k <= (fs - k)) { B = new LongArray(SA, n + fs - k * 2); flags = 0; }
                else if (k <= (MINBUCKETSIZE * 4)) { B = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0); flags = 2; }
                else { B = C; flags = 8; }
            }
            else
            {
                C = B = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0);
                flags = 4 | 8;
            }

            /* stage 1: reduce the problem by at least 1/2
               sort all the LMS-substrings */
            getCounts(T, C, n, k); getBuckets(C, B, k, true); /* find ends of buckets */
            for (i = 0; i < n; ++i) { SA[i] = 0; }
            b = -1; i = n - 1; j = n; m = 0; c0 = T[n - 1];
            do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
            for (; 0 <= i;)
            {
                do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) <= c1));
                if (0 <= i)
                {
                    if (0 <= b) { SA[b] = j; }
                    b = --B[c1]; j = i; ++m;
                    do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                }
            }
            if (1 < m)
            {
                LMSsort(T, SA, C, B, n, k);
                name = LMSpostproc(T, SA, n, m);
            }
            else if (m == 1)
            {
                SA[b] = j + 1;
                name = 1;
            }
            else
            {
                name = 0;
            }

            /* stage 2: solve the reduced problem
               recurse if names are not yet unique */
            if (name < m)
            {
                if ((flags & 4) != 0) { C = null; B = null; }
                if ((flags & 2) != 0) { B = null; }
                newfs = (n + fs) - (m * 2);
                if ((flags & (1 | 4 | 8)) == 0)
                {
                    if ((k + name) <= newfs) { newfs -= k; }
                    else { flags |= 8; }
                }
                for (i = m + (n >> 1) - 1, j = m * 2 + newfs - 1; m <= i; --i)
                {
                    if (SA[i] != 0) { SA[j--] = SA[i] - 1; }
                }
                RA = new LongArray(SA, m + newfs);
                sais_main(RA, SA, newfs, m, name, false);
                RA = null;

                i = n - 1; j = m * 2 - 1; c0 = T[n - 1];
                do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                for (; 0 <= i;)
                {
                    do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) <= c1));
                    if (0 <= i)
                    {
                        SA[j--] = i + 1;
                        do { c1 = c0; } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                    }
                }

                for (i = 0; i < m; ++i) { SA[i] = SA[m + SA[i]]; }
                if ((flags & 4) != 0) { C = B = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0); }
                if ((flags & 2) != 0) { B = new LongArray(new MemoryEfficientByteAlignedBigULongArray(k), 0); }
            }

            /* stage 3: induce the result for the original problem */
            if ((flags & 8) != 0) { getCounts(T, C, n, k); }
            /* put all left-most S characters into their buckets */
            if (1 < m)
            {
                getBuckets(C, B, k, true); /* find ends of buckets */
                i = m - 1; j = n; p = SA[m - 1]; c1 = T[p];
                do
                {
                    q = B[c0 = c1];
                    while (q < j) { SA[--j] = 0; }
                    do
                    {
                        SA[--j] = p;
                        if (--i < 0) { break; }
                        p = SA[i];
                    } while ((c1 = T[p]) == c0);
                } while (0 <= i);
                while (0 < j) { SA[--j] = 0; }
            }
            if (isbwt == false) { induceSA(T, SA, C, B, n, k); }
            else { pidx = computeBWT(T, SA, C, B, n, k); }
            C = null; B = null;
            return pidx;
        }

        /*- Suffixsorting -*/
        /* 4-bits per digit */
        /// <summary>
        /// Constructs the suffix array of a given string in linear time.
        /// </summary>
        /// <param name="T">input string</param>
        /// <param name="SA">output suffix array</param>
        /// <param name="n">length of the given string</param>
        /// <returns>0 if no error occurred, -1 or -2 otherwise</returns>
        public static
        long
        sufsort(FourBitDigitBigArray T, BigArray<ulong> SA, long n)
        {
            if ((T == null) || (SA == null) ||
              (SA.Length < n) || (T.Length < n))
            {
                return -1;
            }

            return sais_main(new FourBitDigitStreamArray(T, 0), new LongArray(SA, 0), 0, n, 10, false); //k => 10, not the maximum of this datatype but the only reasonable reason to use it (that it's designed for) is for digits
        }

        /* string */
        /// <summary>
        /// Constructs the suffix array of a given string in linear time.
        /// </summary>
        /// <param name="T">input string</param>
        /// <param name="SA">output suffix array</param>
        /// <param name="n">length of the given string</param>
        /// <returns>0 if no error occurred, -1 or -2 otherwise</returns>
        public static
        long
        sufsort(string T, BigArray<ulong> SA, int n)
        {
            if ((T == null) || (SA == null) ||
            (T.Length < n) || (SA.Length < n)) { return -1; }
            if (n <= 1) { if (n == 1) { SA[0] = 0; } return 0; }
            return sais_main(new StringArray(T, 0), new LongArray(SA, 0), 0, n, 65536, false);
        }

    }
}
