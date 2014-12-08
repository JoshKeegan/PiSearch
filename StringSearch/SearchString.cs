using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSearch.Collections;

namespace StringSearch
{
    public class SearchString
    {
        public static int[] Search(string toSearch, string lookFor)
        {
            //Validation
            if(lookFor == "")
            {
                throw new ArgumentException("lookFor cannot be String.Empty");
            }

            //The string to be searched must be longer than or of equal length to the string being searched for
            if(toSearch.Length >= lookFor.Length)
            {
                List<int> foundIdxs = new List<int>();

                LinkedList<char> prevChars = new LinkedList<char>();

                //Read in the first N chars
                for(int i = 0; i < lookFor.Length; i++)
                {
                    prevChars.AddLast(toSearch[i]);
                }

                //for each of the remaining characters in the string to be searched (and one more to check the last char)
                for (int i = lookFor.Length; true; i++)
                {
                    //If we currently have the string being searched for
                    bool match = true;
                    for(int j = 0; j < lookFor.Length; j++)
                    {
                        if(prevChars.ElementAt(j) != lookFor[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    //Add this match to this list of found matches
                    if(match)
                    {
                        foundIdxs.Add(i - lookFor.Length); //Start position
                    }

                    //If at the end of the string, break
                    if(i == toSearch.Length)
                    {
                        break;
                    }

                    //Update prevChars
                    prevChars.RemoveFirst();
                    prevChars.AddLast(toSearch[i]);
                }

                return foundIdxs.ToArray();
            }
            else //Otherwise the string being seacrhed is shorter than the string being searched for, there cannot be any matches
            {
                return new int[0];
            }
        }

        public static long[] Search(BigArray<ulong> suffixArray, FourBitDigitBigArray digitArray, string lookFor)
        {
            byte[] byteArrLookFor = new byte[lookFor.Length];

            for (int i = 0; i < byteArrLookFor.Length; i++)
            {
                byteArrLookFor[i] = byte.Parse(lookFor[i].ToString());
            }

            return Search(suffixArray, digitArray, byteArrLookFor);
        }

        public static long[] Search(BigArray<ulong> suffixArray, FourBitDigitBigArray digitArray, byte[] lookFor)
        {
            //Validation
            if(lookFor.Length == 0)
            {
                throw new ArgumentException("lookFor must contain at least 1 digit");
            }

            if(digitArray.Length == 0)
            {
                return new long[0];
            }

            if(suffixArray.Length != digitArray.Length)
            {
                throw new ArgumentException("Suffix Array must be the same length as the Digit Array. This is not the correct suffix array for this digit array");
            }

            long matchingPosition = binarySearchForPrefix(suffixArray, digitArray, lookFor, 0, suffixArray.Length - 1);

            //If there were no matches
            if(matchingPosition == -1)
            {
                return new long[0];
            }
            else //Otherwise match found, look for more
            {
                long min = matchingPosition;
                long max = matchingPosition;

                while(min > 0 && doesStartWithSuffix(digitArray, lookFor, (long)suffixArray[min - 1]) == 0)
                {
                    min--;
                }

                while(max < digitArray.Length - 1 && doesStartWithSuffix(digitArray, lookFor, (long)suffixArray[max + 1]) == 0)
                {
                    max++;
                }

                long[] toRet = new long[max - min + 1];
                for(long i = min; i <= max; i++)
                {
                    toRet[i - min] = (long)suffixArray[i];
                }

                //Sort the array of string indices (Array.Sort implements Quicksort)
                Array.Sort(toRet);

                return toRet;
            }
        }

        internal static long binarySearchForPrefix(BigArray<ulong> suffixArray, FourBitDigitBigArray digitArray, byte[] findPrefix, long min, long max)
        {
            long range = max - min;

            if(range == 0)
            {
                //Only one possible value left, check it
                if(doesStartWithSuffix(digitArray, findPrefix, (long)suffixArray[min]) == 0)
                {
                    return min;
                }
                else //No matches
                {
                    return -1;
                }
            }
            else
            {
                long idx = min + (range / 2);

                int hit = doesStartWithSuffix(digitArray, findPrefix, (long)suffixArray[idx]);

                //If this is the answer
                if(hit == 0)
                {
                    return idx;
                }
                //Otherwise if we're too high in the array
                else if(hit == 1)
                {
                    return binarySearchForPrefix(suffixArray, digitArray, findPrefix, min, idx - 1);
                }
                //Otherwise we're too low in the array
                else // hit == -1
                {
                    return binarySearchForPrefix(suffixArray, digitArray, findPrefix, idx + 1, max);
                }
            }
        }

        internal static int doesStartWithSuffix(FourBitDigitBigArray digitArray, byte[] findPrefix, long startIdx)
        {
            for(int i = 0; i < findPrefix.Length; i++)
            {
                byte findPrefixByte = findPrefix[i];
                byte actualByte = digitArray[startIdx + i];

                if (findPrefixByte < actualByte)
                {
                    return 1; //Searching too high (in the array)
                }
                else if(findPrefixByte > actualByte)
                {
                    return -1; //Searching too low (in the array)
                }
            }
            return 0; //Jackpot
        }

        public static int FindNextOccurrence(string toSearch, string lookFor, int fromIdx)
        {
            //Validation
            if(toSearch.Length == 0)
            {
                throw new ArgumentException("toSearch cannot be empty");
            }

            if(lookFor.Length == 0)
            {
                throw new ArgumentException("lookFor cannot be empty");
            }

            if(fromIdx <= toSearch.Length - lookFor.Length)
            {
                FixedLengthQueue<char> prevChars = new FixedLengthQueue<char>(lookFor.Length);

                //Read in the first N chars
                int i = fromIdx;
                for(int j = 0; j < lookFor.Length; i++, j++)
                {
                    prevChars.Enqueue(toSearch[i]);
                }

                //For each of the remaining characters in the string to be searched
                for(; true; i++)
                {
                    //If we currently have the string being searched for
                    bool match = true;
                    for (int j = 0; j < lookFor.Length; j++)
                    {
                        if (prevChars[j] != lookFor[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if(match)
                    {
                        return i - lookFor.Length; //Start position
                    }

                    //If at the end of the string, break
                    if(i == toSearch.Length)
                    {
                        break;
                    }

                    //Update prevChars
                    prevChars.Enqueue(toSearch[i]);
                }

                //Didn't match anything
                return -1;
            }
            else
            {
                return -1;
            }
        }

        public static long FindNextOccurrence4BitDigit(Stream searchStream, string lookFor, long fromIdx)
        {
            byte[] byteArrLookFor = new byte[lookFor.Length];

            for(int i = 0; i < byteArrLookFor.Length; i++)
            {
                byteArrLookFor[i] = byte.Parse(lookFor[i].ToString());
            }

            return FindNextOccurrence4BitDigit(searchStream, byteArrLookFor, fromIdx);
        }

        public static long FindNextOccurrence4BitDigit(Stream searchStream, byte[] lookFor, long fromIdx)
        {
            //Validation
            if (searchStream.Length == 0)
            {
                throw new ArgumentException("toSearch cannot be empty");
            }

            if (lookFor.Length == 0)
            {
                throw new ArgumentException("lookFor cannot be empty");
            }

            //Set the stream position (in bytes)
            searchStream.Position = fromIdx / 2;

            //Keep track of the index in the stream ready to return if we hit a result
            long idx = fromIdx;

            int rawByteRead;
            byte thisByte = byte.MaxValue;

            //If this is an odd index, read in the first byte in the stream so thisByte has a value (as it will expect it to have already been read)
            if(idx % 2 == 1)
            {
                rawByteRead = searchStream.ReadByte();
                if (rawByteRead == -1)
                {
                    return -1;
                }
                thisByte = (byte)rawByteRead;
            }

            //Read in the first N chars
            bool fillingPrev = true;
            bool filledPrevThisIter = false;
            bool eos = false;
            FixedLengthQueue<byte> prev = new FixedLengthQueue<byte>(lookFor.Length);
            byte digit;

            //Have one big loop for both fill & search modes
            while(true)
            {
                //If we're at an even index, read in the next byte
                if(idx % 2 == 0)
                {
                    rawByteRead = searchStream.ReadByte();
                    if(rawByteRead == -1)
                    {
                        eos = true; //End of stream
                        digit = 15;
                    }
                    else //Otherwise read a value
                    {
                        thisByte = (byte)rawByteRead;

                        //Get the first half of the byte as this digit
                        digit = (byte)(thisByte >> 4);
                    }
                }
                else //Otherwise we already have this byte read in
                {
                    //Get the second half of the byte as this digit
                    digit = (byte)(thisByte & 15); //mask 0000 1111 to get the last 4 bits

                    //If this is (0000) 1111 => 15 then this is the end but the byte had to be padded
                    if(digit == 15)
                    {
                        eos = true; //End of stream
                    }
                }

                //If filling up the previous chars
                if(fillingPrev)
                {
                    prev.Enqueue(digit);

                    //Update fillingPrev
                    if(prev.Count == lookFor.Length)
                    {
                        fillingPrev = false;
                        idx++; //Also increment the index as we'll jump straight into the match tests on this iteration
                        filledPrevThisIter = true;
                    }
                }
                
                //If not filling up the previous chars (not equivelant to else because fillingPrev can be set in the previous if)
                if(!fillingPrev) 
                {
                    //If we currently have the digits being searched for
                    bool match = true;
                    for (int i = 0; i < prev.Length; i++ )
                    {
                        if (lookFor[i] != prev[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if(match)
                    {
                        return idx - lookFor.Length; //Start idx
                    }
                    else if(eos) //Else if we haven't found a match & we're at the end of the stream, report back that nothing was found
                    {
                        return -1;
                    }

                    //Update prev (if we haven't filled prev on this iter)
                    if(!filledPrevThisIter)
                    {
                        prev.Enqueue(digit);
                    }
                }

                //Update the current idx for next iter (if we haven't filled prev on this iter, as then it will have already been incremented)
                if(!filledPrevThisIter)
                {
                    idx++;
                }
                else //Otherwise update filledPrevThisIter
                {
                    filledPrevThisIter = false;
                }
            }
        }
    }
}
