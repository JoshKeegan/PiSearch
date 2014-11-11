using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch
{
    public class SearchString
    {
        public static int[] Search(string toSearch, string lookFor)
        {
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

                //for each of the remaining characters in the string to be searched
                for (int i = lookFor.Length; i < toSearch.Length; i++)
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

                    if(match)
                    {
                        foundIdxs.Add(i - lookFor.Length); //Start position
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

        public static int[] Search(int[] suffixArray, FourBitDigitArray digitArray, string lookFor)
        {
            byte[] byteArrLookFor = new byte[lookFor.Length];

            for (int i = 0; i < byteArrLookFor.Length; i++)
            {
                byteArrLookFor[i] = byte.Parse(lookFor[i].ToString());
            }

            return Search(suffixArray, digitArray, byteArrLookFor);
        }

        public static int[] Search(int[] suffixArray, FourBitDigitArray digitArray, byte[] lookFor)
        {
            int matchingPosition = binarySearchForPrefix(suffixArray, digitArray, lookFor, 0, suffixArray.Length - 1);

            //If there were no matches
            if(matchingPosition == -1)
            {
                return new int[0];
            }
            else //Otherwise match found, look for more
            {
                int min = matchingPosition;
                int max = matchingPosition;

                while(min > 0 && doesStartWithSuffix(digitArray, lookFor, min - 1) == 0)
                {
                    min--;
                }

                while(max < digitArray.Length - 1 && doesStartWithSuffix(digitArray, lookFor, max + 1) == 0)
                {
                    max++;
                }

                int[] toRet = new int[max - min + 1];
                for(int i = min; i <= max; i++)
                {
                    toRet[i - min] = i;
                }
                return toRet;
            }
        }

        private static int binarySearchForPrefix(int[] suffixArray, FourBitDigitArray digitArray, byte[] findPrefix, int min, int max)
        {
            int range = max - min;

            if(range == 0)
            {
                //Only one possible value left, check it
                if(doesStartWithSuffix(digitArray, findPrefix, min) == 0)
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
                int idx = min + (range / 2);

                int hit = doesStartWithSuffix(digitArray, findPrefix, suffixArray[idx]);

                //If this is the answer
                if(hit == 0)
                {
                    return suffixArray[idx];
                }
                //Otherwise if we're too high in the array
                else if(hit == 1)
                {
                    return binarySearchForPrefix(suffixArray, digitArray, findPrefix, min, idx - 1);
                }
                //Otherwise we're too low in the array
                else // hit == 0
                {
                    return binarySearchForPrefix(suffixArray, digitArray, findPrefix, idx + 1, max);
                }
            }
        }

        private static int doesStartWithSuffix(FourBitDigitArray digitArray, byte[] findPrefix, int startIdx)
        {
            for(int i = 0; i < findPrefix.Length; i++)
            {
                byte findPrefixByte = findPrefix[i];
                byte actualByte = digitArray[startIdx + i];

                if (findPrefixByte < actualByte)
                {
                    return 1; //Searching too high
                }
                else if(findPrefixByte > actualByte)
                {
                    return -1; //Searching too low
                }
            }
            return 0; //Jackpot
        }

        public static int FindNextOccurrence(string toSearch, string lookFor, int fromIdx)
        {
            if(fromIdx <= toSearch.Length - lookFor.Length)
            {
                LinkedList<char> prevChars = new LinkedList<char>();

                //Read in the first N chars
                int i = fromIdx;
                for(int j = 0; j < lookFor.Length; i++, j++)
                {
                    prevChars.AddLast(toSearch[i]);
                }

                //For each of the remaining characters in the string to be searched
                for(; i < toSearch.Length; i++)
                {
                    //If we currently have the string being searched for
                    bool match = true;
                    for (int j = 0; j < lookFor.Length; j++)
                    {
                        if (prevChars.ElementAt(j) != lookFor[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if(match)
                    {
                        return i - lookFor.Length; //Start position
                    }

                    //Update prevChars
                    prevChars.RemoveFirst();
                    prevChars.AddLast(toSearch[i]);
                }

                //Didn't match anything
                return -1;
            }
            else
            {
                return -1;
            }
        }

        public static int FindNextOccurrence4BitDigit(Stream searchStream, string lookFor, int fromIdx)
        {
            byte[] byteArrLookFor = new byte[lookFor.Length];

            for(int i = 0; i < byteArrLookFor.Length; i++)
            {
                byteArrLookFor[i] = byte.Parse(lookFor[i].ToString());
            }

            return FindNextOccurrence4BitDigit(searchStream, byteArrLookFor, fromIdx);
        }

        public static int FindNextOccurrence4BitDigit(Stream searchStream, byte[] lookFor, int fromIdx)
        {
            //Set the stream position (in bytes)
            searchStream.Position = fromIdx / 2;

            //Keep track of the index in the stream ready to return if we hit a result
            int idx = fromIdx;

            int rawByteRead;
            byte thisByte = byte.MaxValue;

            //If this is an odd index, read in the first byte in the stream so thisByte has a value (as it will expect it to have alreayd been read)
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
            LinkedList<byte> prev = new LinkedList<byte>();

            //Have one big loop for both fill & search modes
            while(true)
            {
                byte digit;

                //If we're at an even index, read in the next byte
                if(idx % 2 == 0)
                {
                    rawByteRead = searchStream.ReadByte();
                    if(rawByteRead == -1)
                    {
                        return -1;
                    }
                    thisByte = (byte)rawByteRead;

                    //Get the first half of the byte as this digit
                    digit = (byte)(thisByte >> 4);
                }
                else //Otherwise we already have this byte read in
                {
                    //Get the second half of the byte as this digit
                    digit = (byte)(thisByte & 15); //mask 0000 1111 to get the last 4 bits

                    //If this is (0000) 1111 => 15 then this is the end but the byte had to be padded
                    if(digit == 15)
                    {
                        return -1;
                    }
                }

                //If filling up the previous chars
                if(fillingPrev)
                {
                    prev.AddLast(digit);

                    //Update fillingPrev
                    fillingPrev = prev.Count != lookFor.Length;
                }
                else //Otherwise we're actually searching
                {
                    //If we currently have the digits being searched for
                    bool match = true;
                    int i = 0;
                    foreach(byte prevEl in prev)
                    {
                        if(lookFor[i] != prevEl)
                        {
                            match = false;
                            break;
                        }

                        i++;
                    }

                    if(match)
                    {
                        return idx - lookFor.Length; //Start idx
                    }

                    //Update prev
                    prev.RemoveFirst();
                    prev.AddLast(digit);
                }

                //Update the current idx for next iter
                idx++;
            }
        }
    }
}
