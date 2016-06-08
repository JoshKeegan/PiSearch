/*
 * Program entry point for the String Search Console application, the development interface for the PiSearch project
 * By Josh Keegan 07/11/2014
 * Last Edit 08/06/2016
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSearch;
using StringSearch.Collections;
using StringSearch.IO;
using SuffixArray;

namespace StringSearchConsole
{
    public static class Program
    {
        //Constants
        private const string PRECOMPUTED_SEARCH_RESULTS_FILE_EXTENSION = "precomputed";

        //Variables
        private static string workingDirectory = "";
        private static string loadedString = null;
        private static Stream loaded4BitDigitStream = null;
        private static FourBitDigitBigArray fourBitDigitArray = null;
        private static IBigArray<ulong> suffixArray = null;
        private static IBigArray<ulong> singleLengthPrecomputedSearchResults = null;
        private static IBigArray<PrecomputedSearchResult>[] precomputedSearchResults = null;
        private static readonly Stopwatch stopwatch = new Stopwatch();
        private static Type suffixArrayType = typeof(MemoryEfficientBigULongArray); // Note: Byte Aligned better for generation (fater), non-byte aligned better for searching (due to better memory efficiency). However, depends on hardware available
        private static Type suffixArrayStreamType = null; //Null for store in memory (uses the default underlying store of the BigArray<ulong> implementation that is being used
        private static string suffixArrayFileName = null; //When using FileStream for the suffix array
        private static int suffixArrayFileStreamBufferSize = 4096; //Default as specified at http://msdn.microsoft.com/en-us/library/f20d7x6t%28v=vs.110%29.aspx
        private static int suffixArrayFastFileStreamIoBufferSize = 8;

        public static void Main(string[] args)
        {
            while(true)
            {
                bool quit = menu();

                if(quit)
                {
                    break;
                }
            }
        }

        //Returns whether to quit the program
        private static bool menu()
        {
            string menuTopLine = "String Search Main Menu";
            string divider = new String('-', menuTopLine.Length);

            Console.WriteLine(divider + '\n' + menuTopLine + '\n' + divider + '\n' + 
                "2.\tLoad text file (one continuous string of digits)\n" + 
                "3.\tSearch loaded string\n" + 
                "4.\tSet working directory\n" + 
                "5.\tConvert PiFast43 output to raw decimal places of pi\n" +
                "6.\tSearch loaded string for next occurrence\n" + 
                "7.\t4-bit digit compress file\n" + 
                "8.\tLoad compressed 4-bit digit file (into memory)\n" + 
                "9.\tSearch loaded 4-bit compressed stream\n" +
                "10.\tGenerate suffix array from loaded 4-bit compressed stream\n" + 
                "11.\tSave suffix array (64-bit)\n" +
                "12.\tLoad suffix array (64-bit)\n" +
                "13.\tSearch loaded suffix array\n" + 
                "14.\tGenerate suffix array from loaded string\n" +
                "15.\tPrint Suffix Array\n" + 
                "16.\tPrint 4-bit digit array\n" + 
                "17.\tConvert y-cruncher output to raw decimal places of pi\n" +
                "18.\tTake first n digits from compressed 4-bit digit file\n" +
                "19.\tSet Suffix Array Data Type\n" + 
                "20.\tSave suffix array's underlying stream\n" + 
                "21.\tSet Suffix Array memory location\n" +
                "22.\tUse previous file system suffix array file\n" + 
                "23.\tUse compressed 4-bit digit file straight from file system\n" + 
                "24.\tSet Suffix Array File Stream Buffer Size\n" + 
                "25.\tVerify Suffix Array\n" +
                "26.\tPrecompute suffix array indices for search strings of a specified length\n" + 
                "27.\tSave underlying stream of precomputed suffix array search indices for a specified length\n" + 
                "28.\tPrint precomputed suffix array indices\n" +
                "29.\tLoad suffix array (of selected suffix array data type)\n" + 
                "30.\tLoad precomputed search results\n" + 
                "31.\tFind first value (lexicographically ordered) not in pi\n" +
                "q.\tQuit");

            bool quit = false;

            while(true)
            {
                Console.Write("Please enter your selection: ");
                string selection = Console.ReadLine();

                bool validSelection = true;

                //Time each operation
                stopwatch.Start();

                switch(selection)
                {
                    case "2": //Load file
                        subLoadFile();
                        break;
                    case "3": //Search loaded string
                        subSearchLoadedString();
                        break;
                    case "4": //Set working directory
                        subSetWorkingDir();
                        break;
                    case "5": //Convert PiFast43 output to raw decimal places of pi
                        subProcessPiFast43Output();
                        break;
                    case "6": //Search next from loaded string
                        subSearchLoadedStringForNextOccurrence();
                        break;
                    case "7": //4-bit digit compress file
                        subFourBitDigitCompressFile();
                        break;
                    case "8": //Load a compressed 4-bit digit file
                        subLoad4BitDigitFile();
                        break;
                    case "9": //Search next from loaded 4-bit digit stream
                        subSearchLoaded4BitDigitStreamForNextOccurrence();
                        break;
                    case "10": //Generate suffix array from loaded 4-bit compressed stream
                        subGenerateSuffixArray();
                        break;
                    case "11": //Save suffix array
                        subSaveSuffixArray();
                        break;
                    case "12": //Load suffix array
                        subLoadSuffixArray();
                        break;
                    case "13": //Search loaded suffix array
                        subSearchLoadedSuffixArray();
                        break;
                    case "14": //Generate suffix array from loaded string
                        subGenerateSuffixArrayFromLoadedString();
                        break;
                    case "15": //Print suffix array
                        subPrintSuffixArray();
                        break;
                    case "16": //Print 4-bit digit file
                        subPrintFourBitDigitArray();
                        break;
                    case "17": //Convert y-cruncher output to raw decimal places of pi
                        subProcessYCruncherOutput();
                        break;
                    case "18": //Take first n digits from a compress-ed 4-bit digit file
                        subTakeFirstNDigitsFrom4BitDigitFile();
                        break;
                    case "19": //Set suffix array data type
                        subSetSuffixArrayDataType();
                        break;
                    case "20": //Save suffix array's underlying stream
                        subSaveSuffixArraysUnderlyingStream();
                        break;
                    case "21": //Set suffix array memory location
                        subSetSuffixArrayMemoryLocation();
                        break;
                    case "22": //Use previous file system suffix array file
                        subUsePreviousFileSystemSuffixArrayFile();
                        break;
                    case "23": //Use compressed 4-bit digit file straight from the file system
                        subUse4BitDigitFileStraightFromFileSystem();
                        break;
                    case "24": //Set suffix array file stream buffer size
                        subSetSuffixArrayFileStreamBufferSize();
                        break;
                    case "25": //Verify suffix array
                        subVerifySuffixArray();
                        break;
                    case "26": //Precompute suffix array indices for search strings of a specified length
                        subPrecomputeSuffixArrayIndices();
                        break;
                    case "27": //Save underlying stream of precomputed suffix array search indices for a specified length
                        subSaveSingleLengthPrecomputedSearchResultsUnderlyingStream();
                        break;
                    case "28": //Print precomputed suffix array indices
                        subPrintSingleLengthPrecomputedSearchResults();
                        break;
                    case "29": //Load suffix array (of selected suffix array data type)
                        subLoadSuffixArraySelectedSuffixArrayDataType();
                        break;
                    case "30": //Load precomputed search results
                        subLoadPrecomputedSearchResults();
                        break;
                    case "31": //Find first value (lexicographically ordered) not in pi
                        subFindFirstValueNotInPi();
                        break;
                    case "q": //Quit
                        quit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default: //Selection not valid
                        validSelection = false;
                        Console.WriteLine("Selection Invalid");
                        break;
                }

                if(validSelection)
                {
                    stopwatch.Stop();
                    Console.WriteLine("Operation completed in: {0}", stopwatch.Elapsed);

                    break;
                }
            }
            return quit;
        }

        private static void subFourBitDigitCompressFile()
        {
            string fileIn;
            while(true)
            {
                Console.Write("File to compress: ");
                fileIn = Console.ReadLine();

                if(File.Exists(workingDirectory + fileIn))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileIn);
                }
            }

            Console.Write("File out: ");
            string fileOut = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            Compression.CompressFile4BitDigit(workingDirectory + fileIn, workingDirectory + fileOut);
        }

        private static void subSaveSuffixArray()
        {
            Console.Write("File out: ");
            string fileName = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            FileStream fs = new FileStream(workingDirectory + fileName, FileMode.Create);

            foreach (ulong i in suffixArray)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                fs.Write(bytes, 0, 8);
            }

            fs.Close();
        }

        private static void subLoadSuffixArray()
        {
            string fileName;

            while(true)
            {
                Console.Write("Load suffix array file: ");
                fileName = Console.ReadLine();

                if(File.Exists(workingDirectory + fileName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }

            stopwatch.Reset();
            stopwatch.Start();

            FileStream fs = new FileStream(workingDirectory + fileName, FileMode.Open);

            int len = (int)(fs.Length / 8);

            suffixArray = createBigArrayFromSettings(len, (uint)len);

            byte[] bytes = new byte[8];
            int state = 4;
            int i = 0;
            while(true)
            {
                state = fs.Read(bytes, 0, 8);

                if(state == 8)
                {
                    suffixArray[i] = BitConverter.ToUInt64(bytes, 0);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        private static void subLoadSuffixArraySelectedSuffixArrayDataType()
        {
            //Must have a four bit digit array loaded
            if(fourBitDigitArray == null)
            {
                Console.WriteLine("Must have a 4 bit digit array loaded");
                return;
            }

            string fileName;

            while (true)
            {
                Console.Write("Load suffix array file: ");
                fileName = Console.ReadLine();

                if (File.Exists(workingDirectory + fileName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }

            stopwatch.Reset();
            stopwatch.Start();

            Stream memStream = Compression.ReadStreamNoCompression(workingDirectory + fileName);

            suffixArray = createBigArrayFromSettings(fourBitDigitArray.Length, (ulong)fourBitDigitArray.Length, memStream);
        }

        private static void subLoadPrecomputedSearchResults()
        {
            //Must have a four bit digit array loaded
            if(fourBitDigitArray == null)
            {
                Console.WriteLine("Must have a 4 bit digit array loaded");
                return;
            }

            //Must have a suffix array loaded
            if(suffixArray == null)
            {
                Console.WriteLine("Must have a suffix array loaded");
                return;
            }

            string dirName;

            while(true)
            {
                Console.Write("Load precomputed search results from directory: ");
                dirName = Console.ReadLine();

                if(Directory.Exists(workingDirectory + dirName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Directory not found \"{0}\"", dirName);
                }
            }

            //TODO: Further options
            //  Use from file system or load into memory
            //  If file system which stream to use?
            //  BigArray<ulong> data type

            stopwatch.Reset();
            stopwatch.Start();

            string[] filePaths = Directory.GetFiles(workingDirectory + dirName, "*." + PRECOMPUTED_SEARCH_RESULTS_FILE_EXTENSION);

            //If there are n .precomputed files then they should be for the digits 1-n
            foreach(string filePath in filePaths)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                int searchStringLength;
                try
                {
                    searchStringLength = int.Parse(fileNameWithoutExtension);
                }
                catch
                {
                    Console.WriteLine("Files must be names n." + PRECOMPUTED_SEARCH_RESULTS_FILE_EXTENSION);
                    return;
                }

                if(searchStringLength < 1)
                {
                    Console.WriteLine("All search string lengths must be >= 1");
                }
                if(searchStringLength > filePaths.Length)
                {
                    Console.WriteLine("Cannot miss a precomputed file. i.e. if you have 1 and 3 you must have 2");
                }
            }

            //We have all the files {1-filePaths.Length}.precomputed in this directory. 
            precomputedSearchResults = new IBigArray<PrecomputedSearchResult>[filePaths.Length];

            for(int i = 0; i < precomputedSearchResults.Length; i++)
            {
                int searchStringLength = i + 1;

                Stream s = new FileStream(
                    workingDirectory + dirName + "/" + searchStringLength + "." + PRECOMPUTED_SEARCH_RESULTS_FILE_EXTENSION, 
                    FileMode.Open, FileAccess.Read);

                IBigArray<ulong> underlyingArray = new MemoryEfficientBigULongArray(
                    PrecomputeSearchResults.NumPrecomputedResults(searchStringLength) * 2, (ulong)fourBitDigitArray.Length, s);

                IBigArray<PrecomputedSearchResult> singleLengthPrecomputedSearchResults = new BigPrecomputedSearchResultsArray(underlyingArray);
                precomputedSearchResults[i] = singleLengthPrecomputedSearchResults;
            }
        }

        private static void subLoadFile()
        {
            while(true)
            {
                Console.Write("Load File: ");
                string fileName = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                //Check that this file exists
                if(File.Exists(workingDirectory + fileName))
                {
                    Console.WriteLine("Loading file \"{0}\"", fileName);

                    //Release the currently loaded string for garbage collection
                    loadedString = null;

                    loadedString = Compression.ReadStringNoCompression(workingDirectory + fileName);
                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }
        }

        private static void subLoad4BitDigitFile()
        {
            while(true)
            {
                Console.Write("Load 4-bit digit file: ");
                string fileName = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                //Check that this file exists
                if(File.Exists(workingDirectory + fileName))
                {
                    Console.WriteLine("Loading 4-bit digit compressed file \"{0}\"", fileName);

                    //Release any currently loaded stream for garbage collection
                    if(loaded4BitDigitStream != null)
                    {
                        loaded4BitDigitStream.Close();
                        loaded4BitDigitStream = null;
                    }

                    loaded4BitDigitStream = Compression.ReadStreamNoCompression(workingDirectory + fileName);

                    //Now wrap it in a FourBitDigitArray
                    fourBitDigitArray = new FourBitDigitBigArray(loaded4BitDigitStream);

                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }
        }

        private static void subUse4BitDigitFileStraightFromFileSystem()
        {
            //First find out which type of Stream to use when accessing the file system
            Console.WriteLine("What type of File Stream would you like to use to access the 4-bit digit file on the File System");
            Console.WriteLine("1.\tFileStream (default)\n" +
                "2.\tFastFileStream (Windows Only)");

            Type streamType = null; //Must have default value, will never actually be used

            while(true)
            {
                Console.Write("Selection: ");
                string selection = Console.ReadLine();

                bool validSelection = true;

                switch(selection)
                {
                    case "1":
                        streamType = typeof(FileStream);
                        break;
                    case "2":
                        streamType = typeof(FastFileStream);
                        break;
                    default:
                        validSelection = false;
                        break;
                }

                if(validSelection)
                {
                    break;
                }
            }

            while(true)
            {
                Console.Write("Use 4-bit digit file: ");
                string fileName = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                //Check that this file exists
                if(File.Exists(workingDirectory + fileName))
                {
                    Console.WriteLine("Using 4-bit digit compressed file \"{0}\" straight from the file system", fileName);

                    //Release any currently loaded stream for garbage collection
                    if(loaded4BitDigitStream != null)
                    {
                        loaded4BitDigitStream.Close();
                        loaded4BitDigitStream = null;
                    }

                    if(streamType == typeof(FileStream))
                    {
                        loaded4BitDigitStream = new FileStream(workingDirectory + fileName, FileMode.Open);
                    }
                    else //FastFileStream
                    {
                        //Open with random access file flag specified.
                        //  Note that this is only optimal if using as part of suffix array generation or search, a 
                        //      sequantial search would be best done without specifying a flag, or perhaps SEQUENTIAL_SCAN (testing needed to determine best selection)
                        //TODO: Add option to choose the Flags * Attributes passed to the file rather than just always using Random Access
                        loaded4BitDigitStream = new FastFileStream(workingDirectory + fileName, FileAccess.Read, 1, WinFileFlagsAndAttributes.FILE_FLAG_RANDOM_ACCESS);
                    }
                    

                    //Now wrap it in a FourBitDigitArray
                    fourBitDigitArray = new FourBitDigitBigArray(loaded4BitDigitStream);

                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }
        }

        private static void subSetSuffixArrayFileStreamBufferSize()
        {
            while(true)
            {
                Console.Write("Suffix Array File Stream Buffer Size (bytes): ");

                string strBufferSize = Console.ReadLine();

                try
                {
                    suffixArrayFileStreamBufferSize = int.Parse(strBufferSize);

                    if(suffixArrayFileStreamBufferSize > 0)
                    {
                        break;
                    }
                }
                catch {  }
            }
        }

        private static void subVerifySuffixArray()
        {
            //A suffix array is valid if for each consecutive pair of values the first points to a lexicographically
            //  smaller string in the digits than the second
            //  Note: This will be extremely expensive to compute for large numbers of digits

            int prevCurrPercent = -1;
            bool valid = true;
            for(long i = 0; i < suffixArray.Length - 1; i++)
            {
                //Percentage progress indicator
                int currPercent = (int)((i * 100) / suffixArray.Length);
                if(currPercent != prevCurrPercent)
                {
                    Console.WriteLine("{0}% complete", currPercent);
                    prevCurrPercent = currPercent;
                }

                //Get the two suffix array values (digits indices) to compare
                long a = (long)suffixArray[i];
                long b = (long)suffixArray[i + 1];

                //Check that a points to a lexicographically smaller string than b
                bool aSmaller = false;
                for(long j = a, k = b; true; j++, k++)
                {
                    //Bounds check
                    if(j >= fourBitDigitArray.Length)
                    {
                        aSmaller = true;
                        break;
                    }
                    if(k >= fourBitDigitArray.Length)
                    {
                        aSmaller = false;
                        break;
                    }

                    byte valA = fourBitDigitArray[j];
                    byte valB = fourBitDigitArray[k];

                    if (valA < valB)
                    {
                        aSmaller = true;
                        break;
                    }
                    else if(valA > valB)
                    {
                        aSmaller = false;
                        break;
                    }
                }

                if(!aSmaller)
                {
                    valid = false;
                    break;
                }
            }

            Console.WriteLine("Suffix Array is {0}valid", valid ? "" : "in");
        }

        private static void subPrecomputeSuffixArrayIndices()
        {
            if(fourBitDigitArray != null && suffixArray != null)
            {
                int stringLength = -1;

                while(stringLength <= 0 || stringLength >= 10)
                {
                    Console.Write("Precompute results for strings of length: ");
                    string strStringLength = Console.ReadLine().Trim();

                    try
                    {
                        stringLength = int.Parse(strStringLength);
                    }
                    catch {  }

                    if(stringLength <= 0 || stringLength >= 10)
                    {
                        Console.WriteLine("Length must be > 0 and < 10");
                    }
                }

                Console.WriteLine("Precomputing suffix array results for strings of length {0}", stringLength);
                stopwatch.Reset();
                stopwatch.Start();

                //TODO: option to specify which implementation of BigArray<ulong> is to be used
                singleLengthPrecomputedSearchResults = PrecomputeSearchResults.GenerateSearchResults(
                    fourBitDigitArray, suffixArray, stringLength);
            }
            else
            {
                Console.WriteLine("Must have a 4 bit digit stream and suffix array loaded");
            }
        }

        private static void subPrintSingleLengthPrecomputedSearchResults()
        {
            if(singleLengthPrecomputedSearchResults != null)
            {
                for(long i = 0; i < singleLengthPrecomputedSearchResults.Length; i += 2)
                {
                    ulong min = singleLengthPrecomputedSearchResults[i];
                    ulong max = singleLengthPrecomputedSearchResults[i + 1];

                    Console.WriteLine("Min: {0}, Max: {1}", min, max);
                }
            }
            else
            {
                Console.WriteLine("Must have a single length precomputed search results file loaded");
            }
        }

        private static void subFindFirstValueNotInPi()
        {
            //Require precomputed search results
            if(precomputedSearchResults == null)
            {
                Console.WriteLine("Must have some precomputed search results loaded");
            }

            bool found = false;

            //Note: Assumes Precomputed search results are loaded into the array in the correct order (1, 2, 3, ...)
            for(int i = 0; i < precomputedSearchResults.Length && !found; i++)
            {
                Console.WriteLine("Searching results of length {0}", i + 1);
                IBigArray<PrecomputedSearchResult> precomputedResults = precomputedSearchResults[i];

                for(long j = 0; j < precomputedResults.Length; j++)
                {
                    PrecomputedSearchResult result = precomputedResults[j];

                    //The stored maximum is exclusive, so if min == max there was no results found
                    if(result.MinSuffixArrayIdx == result.MaxSuffixArrayIdx)
                    {
                        string searched = j.ToString("D" + (i + 1));
                        Console.WriteLine("{0} does not occur in the first {1} digits", searched, fourBitDigitArray.Length);
                        found = true;
                        break;
                    }
                }
            }

            if(!found)
            {
                Console.WriteLine("Could not find a string or digits that doesn't occur in the first {0} digits of pi, up to length {1}",
                    fourBitDigitArray.Length, precomputedSearchResults.Length);
            }
        }

        private static void subSearchLoadedString()
        {
            if(loadedString != null)
            {
                Console.Write("String to find: ");
                string toFind = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                int[] foundIdxs = SearchString.Search(loadedString, toFind);
                Console.WriteLine("Found {0} results", foundIdxs.Length);
                foreach(int idx in foundIdxs)
                {
                    Console.WriteLine(idx);
                }
            }
            else
            {
                Console.WriteLine("Must load a string in order to search it");
            }
        }

        private static void subSearchLoadedStringForNextOccurrence()
        {
            if(loadedString != null)
            {
                Console.Write("String to find: ");
                string toFind = Console.ReadLine();

                int startIdx = -1;
                while(startIdx < 0) //TODO: Clean up. Thsi while does nothing (tryparse gives 0 if invalid input)
                {
                    Console.Write("Start index: ");
                    string strInput = Console.ReadLine();

                    int.TryParse(strInput, out startIdx);
                }

                stopwatch.Reset();
                stopwatch.Start();

                int nextOccurrence = SearchString.FindNextOccurrence(loadedString, toFind, startIdx);
                Console.WriteLine(nextOccurrence);
            }
            else
            {
                Console.WriteLine("Must load a string in order to search it");
            }
        }

        private static void subSearchLoaded4BitDigitStreamForNextOccurrence()
        {
            if(loaded4BitDigitStream != null)
            {
                Console.Write("String of Digits to find: ");
                string toFind = Console.ReadLine();

                Console.Write("Start index: ");
                string strInput = Console.ReadLine();

                int startIdx;
                int.TryParse(strInput, out startIdx);

                stopwatch.Reset();
                stopwatch.Start();

                long nextOccurrence = SearchString.FindNextOccurrence4BitDigit(loaded4BitDigitStream, toFind, startIdx);
                Console.WriteLine(nextOccurrence);
            }
            else
            {
                Console.WriteLine("Must load a 4-bit digit stream in order to search it");
            }
        }

        private static void subSearchLoadedSuffixArray()
        {
            if(suffixArray != null && loaded4BitDigitStream != null)
            {
                Console.Write("String of Digits to find: ");
                string toFind = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, toFind, precomputedSearchResults);
                long[] foundIdxs = suffixArrayRange.SortedValues;
                Console.WriteLine("Found {0} results", foundIdxs.Length);
                foreach (long idx in foundIdxs)
                {
                    Console.WriteLine(idx);
                }
            }
            else
            {
                Console.WriteLine("Must have a suffix array and a 4-bit digit stream loaded");
            }
        }

        private static void subSetWorkingDir()
        {
            while(true)
            {
                Console.Write("New Working Directory: ");
                string newDir = Console.ReadLine();

                stopwatch.Reset();
                stopwatch.Start();

                if(Directory.Exists(newDir))
                {
                    workingDirectory = newDir;

                    //Ensure that the directory path ends in a / or \
                    char last = workingDirectory[workingDirectory.Length - 1];
                    if(last != '/' && last != '\\')
                    {
                        workingDirectory += "/";
                    }

                    break;
                }
                else
                {
                    Console.WriteLine("Directory not found \"{0}\"", newDir);
                }
            }
        }

        private static void subProcessPiFast43Output()
        {
            string fileInName;
            while(true)
            {
                Console.Write("PiFast43 txt output file name: ");
                fileInName = Console.ReadLine();

                if(File.Exists(workingDirectory + fileInName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Couldn't find file \"{0}\"", fileInName);
                }
            }

            Console.Write("Output file name: ");
            string fileOutName = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            convertPiFast43File(fileInName, fileOutName);
        }

        private static void subProcessYCruncherOutput()
        {
            string fileInName;
            while (true)
            {
                Console.Write("y-cruncher txt output file name (decimal, uncompressed): ");
                fileInName = Console.ReadLine();

                if (File.Exists(workingDirectory + fileInName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Couldn't find file \"{0}\"", fileInName);
                }
            }

            Console.Write("Output file name: ");
            string fileOutName = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            convertYCruncherFile(fileInName, fileOutName);
        }

        private static void subTakeFirstNDigitsFrom4BitDigitFile()
        {
            string fileInName;
            while(true)
            {
                Console.Write("4-bit digit file in: ");
                fileInName = Console.ReadLine();

                if(File.Exists(workingDirectory + fileInName))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Couldn't find file \"{0}\"", fileInName);
                }
            }

            //Work out the number of digits in this file
            FileStream stream = new FileStream(workingDirectory + fileInName, FileMode.Open);
            long numDigits = stream.Length * 2;
            
            //If the last bits are 1111 (15), then length is odd (one less)
            stream.Position = stream.Length - 1;
            int lastByte = stream.ReadByte();
            if((lastByte & 15) == 15)
            {
                numDigits--;
            }

            stream.Close();

            Console.Write("Output file name: ");
            string fileOutName = Console.ReadLine();

            long takeNumDigits;
            while(true)
            {
                Console.Write("Number of digits to take: ");
                long.TryParse(Console.ReadLine(), out takeNumDigits);

                if(takeNumDigits <= 0)
                {
                    Console.WriteLine("takeNumDigits must be > 0");
                }
                else if(takeNumDigits > numDigits)
                {
                    Console.WriteLine("takeNumDigits must be less than numDigits ({0})", numDigits);
                }
                else
                {
                    break;
                }
            }

            stopwatch.Reset();
            stopwatch.Start();

            takeFirstDigitsFrom4BitDigitFile(fileInName, fileOutName, takeNumDigits);
        }

        private static void subSetSuffixArrayDataType()
        {
            Console.WriteLine("1.\tMemory Efficient Byte-Aligned Big ULong Array (uses a bit less CPU than default, but a bit more memory)\n" +
                "2.\tMemory Efficient Big ULong Array (default)");

            while(true)
            {
                Console.Write("Selection: ");
                string selection = Console.ReadLine();

                bool validSelection = true;

                switch(selection)
                {
                    case "1":
                        suffixArrayType = typeof(MemoryEfficientByteAlignedBigULongArray);
                        break;
                    case "2":
                        suffixArrayType = typeof(MemoryEfficientBigULongArray);
                        break;
                    default:
                        validSelection = false;
                        break;
                }

                if(validSelection)
                {
                    break;
                }
            }
        }

        private static void subSaveSuffixArraysUnderlyingStream()
        {
            Console.Write("output file name: ");
            string fileName = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            UnderlyingStream suffixArray = (UnderlyingStream)Program.suffixArray;

            Compression.WriteStreamNoCompression(suffixArray, workingDirectory + fileName);            
        }

        private static void subSaveSingleLengthPrecomputedSearchResultsUnderlyingStream()
        {
            Console.Write("output file name: ");
            string fileName = Console.ReadLine();

            stopwatch.Reset();
            stopwatch.Start();

            UnderlyingStream precomputed = (UnderlyingStream)Program.singleLengthPrecomputedSearchResults;

            Compression.WriteStreamNoCompression(precomputed, workingDirectory + fileName);
        }

        private static void subSetSuffixArrayMemoryLocation()
        {
            Console.WriteLine("1.\tMemory (RAM) (default)\n" +
                "2.\tFile System (for big computations)");

            while(true)
            {
                Console.Write("Selection: ");
                string selection = Console.ReadLine();

                bool validSelection = true;

                switch(selection)
                {
                    case "1":
                        suffixArrayStreamType = null; //Use the default storage defained by the BigArray<ulong> implementation being used
                        suffixArrayFileName = null;
                        break;
                    case "2":
                        //Ask the user which Stream they'd like to use to access the FileSystem
                        subSetSuffixArrayFileStreamType();

                        //Get the name of the file that will be used to store the suffix array
                        Console.Write("File Name: ");
                        suffixArrayFileName = Console.ReadLine();
                        break;
                    default:
                        validSelection = false;
                        break;
                }

                if(validSelection)
                {
                    break;
                }
            }
        }

        private static void subSetSuffixArrayFileStreamType()
        {
            Console.WriteLine("What File Stream type would you like to use to access the FileSystem?");
            Console.WriteLine("1.\tFileStream (default)\n" +
                "2.\tFastFileStream (Windows only)");

            while(true)
            {
                Console.Write("Selection: ");
                string selection = Console.ReadLine();

                bool validSelection = true;

                switch(selection)
                {
                    case "1":
                        suffixArrayStreamType = typeof(FileStream);
                        break;
                    case "2":
                        suffixArrayStreamType = typeof(FastFileStream);
                        break;
                    default:
                        validSelection = false;
                        break;
                }

                if(validSelection)
                {
                    break;
                }
            }
        }

        private static void subUsePreviousFileSystemSuffixArrayFile()
        {
            //Check that the suffix array stream type is set to be stored on the File System
            if(suffixArrayStreamType != typeof(FileStream) && suffixArrayStreamType != typeof(FastFileStream))
            {
                Console.WriteLine("Suffix array memory location is not set to use the File System");
                return;
            }

            //Check that a four bit digit array has been loaded
            //TODO: String support?? (seems pointless at this stage)
            if(fourBitDigitArray == null)
            {
                Console.WriteLine("Must have a 4-bit digit array loaded");
                return;
            }

            //Check that the selected suffix array file exists
            if(!File.Exists(workingDirectory + suffixArrayFileName))
            {
                Console.WriteLine("The selected suffix array file \"{0}\" doesn't exist", suffixArrayFileName);
                return;
            }

            //Can now load up the file as the FileStream underlying the selected BigArray<ulong> implementation
            suffixArray = createBigArrayFromSettings(fourBitDigitArray.Length, (ulong)fourBitDigitArray.Length);

            Console.WriteLine("Now using previous file system suffix array file");
        }

        private static void subGenerateSuffixArray()
        {
            //Initialise the array that will hold the suffix array
            IBigArray<ulong> underlyingSuffixArray = createBigArrayFromSettings(fourBitDigitArray.Length, (ulong)fourBitDigitArray.Length);
            
            //Now we use the complement array rather than just making the suffix array have to have 64 bits per value,
            //  this is far more memory efficient
            // The complements can (and will) be thrown away after the suffix array is generated, leaving the generated suffix array
            //  in the format specified by the settings
            //TODO: Settings for the underlying complement array stream type. SO can store on disk
            IBigArray<bool> underlyingComplementArray = new BigBoolArray(fourBitDigitArray.Length);
            IBigArray<ulong> suffixArray = new MemoryEfficientComplementBigULongArray(fourBitDigitArray.Length,
                (ulong)fourBitDigitArray.Length, underlyingSuffixArray, underlyingComplementArray);

            //Calculate the suffix array
            long status = SAIS.sufsort(fourBitDigitArray, suffixArray, fourBitDigitArray.Length);

            if(status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
            else
            {
                Console.WriteLine("Using generated Suffix Array");
                Program.suffixArray = underlyingSuffixArray;
            }
        }

        private static void subGenerateSuffixArrayFromLoadedString()
        {
            //Initialise the aray that will hold the suffix array
            IBigArray<ulong> suffixArray = createBigArrayFromSettings(loadedString.Length);

            //Calculate the suffix array
            long status = SAIS.sufsort(loadedString, suffixArray, loadedString.Length);

            if (status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
            else
            {
                Console.WriteLine("Using generated suffix array");
                Program.suffixArray = suffixArray;
            }
        }

        private static void subPrintSuffixArray()
        {
            for(int i = 0; i < suffixArray.Length; i++)
            {
                Console.WriteLine("{0}: {1}\t\t\tdigit: {2}", i, suffixArray[i], fourBitDigitArray[(long)suffixArray[i]]);
            }
        }

        private static void subPrintFourBitDigitArray()
        {
            for(int i = 0; i < fourBitDigitArray.Length; i++)
            {
                Console.WriteLine("{0}:\t\t\t\t{1}", i, fourBitDigitArray[i]);
            }
        }

        private static void convertPiFast43File(string fileInName, string fileOutName)
        {
            StreamReader reader = new StreamReader(workingDirectory + fileInName);
            StreamWriter writer = new StreamWriter(workingDirectory + fileOutName);

            bool startedPi = false;
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                //If we're into the body of Pi
                if(startedPi)
                {
                    if (line != "")
                    {
                        string[] parts = line.Split(' ');

                        foreach(string part in parts)
                        {
                            //If this is the end of the actual digits of pi for this line, stop processing this line
                            if(part == ":")
                            {
                                break;
                            }
                            else //Otherwise this is a block of digits of pi
                            {
                                writer.Write(part);
                            }
                        }
                    }
                }
                else //Otherwise seek the beginning of pi
                {
                    if(line == "Pi = 3.")
                    {
                        startedPi = true;
                    }
                }
            }

            //Clean up
            reader.Close();
            writer.Close();
        }

        private static void convertYCruncherFile(string fileInName, string fileOutName)
        {
            StreamReader reader = new StreamReader(workingDirectory + fileInName);
            StreamWriter writer = new StreamWriter(workingDirectory + fileOutName);

            //Ignore the first 2 chars ("3.")
            reader.Read(new char[2], 0, 2);

            //Read in the file contents 1024 characters at a time
            char[] buffer = new char[1024];
            int charsRead;
            while(!reader.EndOfStream)
            {
                charsRead = reader.Read(buffer, 0, 1024);

                writer.Write(buffer, 0, charsRead);
            }

            //Clean up
            reader.Close();
            writer.Close();
        }

        internal static IBigArray<ulong> convertIntArrayToBigUlongArray(int[] arr)
        {
            IBigArray<ulong> toRet = createBigArrayFromSettings(arr.Length, (uint)arr.Length);

            for(int i = 0; i < arr.Length; i++)
            {
                toRet[i] = (uint)arr[i];
            }

            return toRet;
        }

        private static IBigArray<ulong> createBigArrayFromSettings(long length)
        {
            return createBigArrayFromSettings(length, null);
        }

        private static IBigArray<ulong> createBigArrayFromSettings(long length, ulong? maxValue)
        {
            //First make the stream that will be used as the data structure underlying the BigArray
            Stream stream;

            //If storing in memory, do not supply a stream as then the BigArray<ulong> implementation that is being used will determine how best to store the data in memory
            if(suffixArrayStreamType == null)
            {
                stream = null;
            }
            //Else if storing on the File System and accessing it with the default FileStream
            else if(suffixArrayStreamType == typeof(FileStream))
            {
                //Note: the suffx array file is always opened for Read & Write, even if we only actually need read permission. This could perhaps be improved later
                //On a web server it may be desirable to allow multiple streams to be using the same file, so that requests can be handled in parallel. 
                //  As such FileShare is set to allow other process to both read and write to the file. Both are given even though read is the only one needed because it is always opened
                //  for both read & write
                stream = new FileStream(workingDirectory + suffixArrayFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, suffixArrayFileStreamBufferSize);
            }
            //Else if storing on the File SYstem and accessing it with the FastFileStream
            else if(suffixArrayStreamType == typeof(FastFileStream))
            {
                stream = new FastFileStream(workingDirectory + suffixArrayFileName, FileAccess.ReadWrite, suffixArrayFastFileStreamIoBufferSize, WinFileFlagsAndAttributes.FILE_FLAG_RANDOM_ACCESS);
            }
            else
            {
                throw new ArgumentException("Suffix Array stream type not recognised");
            }

            //Have now constructed the underlying Stream, Construct the Big Array
            return createBigArrayFromSettings(length, maxValue, stream);
        }

        private static IBigArray<ulong> createBigArrayFromSettings(long length, ulong? maxValue, Stream stream)
        {
            List<object> bigArrayArgs = new List<object>();
            bigArrayArgs.Add(length);

            if (maxValue != null)
            {
                bigArrayArgs.Add(maxValue.GetValueOrDefault());
            }

            //If we've made a stream to supply to the constructor, add it to the list of args
            if (stream != null)
            {
                bigArrayArgs.Add(stream);
            }

            object[] arrBigArrayArgs = bigArrayArgs.ToArray();

            IBigArray<ulong> bigArray = (IBigArray<ulong>)Activator.CreateInstance(suffixArrayType, arrBigArrayArgs);

            return bigArray;
        }

        private static void takeFirstDigitsFrom4BitDigitFile(string fileInName, string fileOutName, long numDigits)
        {
            FileStream streamIn = new FileStream(workingDirectory + fileInName, FileMode.Open);
            FileStream streamOut = new FileStream(workingDirectory + fileOutName, FileMode.CreateNew);

            long outLength = numDigits;
            if(outLength % 2 == 1)
            {
                outLength++;
            }
            outLength /= 2;

            streamOut.SetLength(outLength);

            for(long i = 0; i < outLength; i++)
            {
                streamOut.WriteByte((byte)streamIn.ReadByte());
            }

            //If odd, set the last half of the last byte to 1111 (15)
            if(numDigits % 2 == 1)
            {
                streamOut.Position = streamOut.Length - 1;
                byte lastByte = (byte)streamOut.ReadByte();
                lastByte = (byte)(lastByte | 15);
                streamOut.Position = streamOut.Length - 1;
                streamOut.WriteByte(lastByte);
            }

            //Clean up
            streamIn.Close();
            streamOut.Close();
        }
    }
}
