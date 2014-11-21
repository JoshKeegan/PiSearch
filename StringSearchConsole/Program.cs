using StringSearch;
using SuffixArray;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearchConsole
{
    class Program
    {
        //Constants
        const string COMPRESSED_FILE_EXTENSION = ".7z";

        //Variables
        private static string workingDirectory = "";
        private static string loadedString = null;
        private static Stream loaded4BitDigitStream = null;
        private static FourBitDigitArray fourBitDigitArray = null;
        private static int[] suffixArray = null;
        private static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
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
                "1.\tCompress File\n" +
                "2.\tLoad File\n" + 
                "3.\tSearch loaded string\n" + 
                "4.\tSet working directory\n" + 
                "5.\tConvert PiFast43 output to raw decimal places of pi\n" +
                "6.\tSearch loaded string for next occurrence\n" + 
                "7.\t4-bit digit compress file\n" + 
                "8.\tLoad compressed 4-bit digit file\n" + 
                "9.\tSearch loaded 4-bit compressed stream\n" +
                "10.\tGenerate suffix array from loaded 4-bit compressed stream\n" + 
                "11.\tSave suffix array\n" +
                "12.\tLoad suffix array\n" +
                "13.\tSearch loaded suffix array\n" + 
                "14.\tGenerate suffix array from loaded string\n" +
                "15.\tPrint Suffix Array\n" + 
                "16.\tPrint 4-bit digit array\n" + 
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
                    case "1": //Compress file
                        subCompressFile();
                        break;
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

        private static void subCompressFile()
        {
            while(true)
            {
                Console.Write("File to compress: ");
                string fileName = Console.ReadLine();

                if(File.Exists(workingDirectory + fileName))
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    CompressIfNotExists(workingDirectory + fileName);
                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
            }
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

            foreach(int i in suffixArray)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                fs.Write(bytes, 0, 4);
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

            int len = (int)(fs.Length / 4);

            suffixArray = new int[len];

            byte[] bytes = new byte[4];
            int state = 4;
            int i = 0;
            while(true)
            {
                state = fs.Read(bytes, 0, 4);

                if(state == 4)
                {
                    suffixArray[i] = BitConverter.ToInt32(bytes, 0);
                    i++;
                }
                else
                {
                    break;
                }
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

                //Check if the name of a compressed file has been entered
                bool compressedFile = fileName.Length >= COMPRESSED_FILE_EXTENSION.Length && fileName.Substring(fileName.Length - COMPRESSED_FILE_EXTENSION.Length) == COMPRESSED_FILE_EXTENSION;

                //If we've been given the name of an uncompressed file, check if there is a compressed version
                if(!compressedFile)
                {
                    if(File.Exists(workingDirectory + fileName + COMPRESSED_FILE_EXTENSION))
                    {
                        compressedFile = true;
                        fileName += COMPRESSED_FILE_EXTENSION;
                    }
                }

                //Check that this file exists
                if(File.Exists(workingDirectory + fileName))
                {
                    Console.WriteLine("Loading {0}compressed file \"{1}\"", (compressedFile ? "" : "un"), fileName);

                    //Release the currently loaded string for garbage collection
                    loadedString = null;

                    if(compressedFile)
                    {
                        loadedString = Compression.ReadStringLZMA(workingDirectory + fileName);
                    }
                    else
                    {
                        loadedString = Compression.ReadStringNoCompression(workingDirectory + fileName);
                    }
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

                    loaded4BitDigitStream = Compression.ReadStreamNoComression(workingDirectory + fileName);

                    //Now wrap it in a FourBitDigitArray
                    fourBitDigitArray = new FourBitDigitArray(loaded4BitDigitStream);

                    break;
                }
                else
                {
                    Console.WriteLine("File not found \"{0}\"", fileName);
                }
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

                int nextOccurrence = SearchString.FindNextOccurrence4BitDigit(loaded4BitDigitStream, toFind, startIdx);
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

                int[] foundIdxs = SearchString.Search(suffixArray, fourBitDigitArray, toFind);
                Console.WriteLine("Found {0} results", foundIdxs.Length);
                foreach (int idx in foundIdxs)
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

        private static void subGenerateSuffixArray()
        {
            //Calculate the number of digits in the loaded 4 bit digit stream
            int length = (int)loaded4BitDigitStream.Length * 2;

            //Check if the last item in the stream is empty
            loaded4BitDigitStream.Position = loaded4BitDigitStream.Length - 1;
            int lastByte = loaded4BitDigitStream.ReadByte();
            int right = lastByte & 15; // mask 0000 1111

            if(right == 15)
            {
                length--;
            }

            //Initialise the array that will hold the suffix array
            suffixArray = new int[length];

            //Calculate the suffix array
            int status = SAIS.sufsort(loaded4BitDigitStream, suffixArray, length);

            if(status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
        }

        private static void subGenerateSuffixArrayFromLoadedString()
        {
            //Initialise the aray that will hold the suffix array
            suffixArray = new int[loadedString.Length];

            //Calculate the suffix array
            int status = SAIS.sufsort(loadedString, suffixArray, loadedString.Length);

            if (status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
        }

        private static void subPrintSuffixArray()
        {
            for(int i = 0; i < suffixArray.Length; i++)
            {
                Console.WriteLine("{0}: {1}\t\t\tdigit: {2}", i, suffixArray[i], fourBitDigitArray[suffixArray[i]]);
            }
        }

        private static void subPrintFourBitDigitArray()
        {
            for(int i = 0; i < fourBitDigitArray.Length; i++)
            {
                Console.WriteLine("{0}:\t\t\t\t{1}", i, fourBitDigitArray[i]);
            }
        }

        private static void CompressIfNotExists(string filePath)
        {
            string compressedFilePath = filePath + COMPRESSED_FILE_EXTENSION;

            if(!File.Exists(compressedFilePath))
            {
                Console.WriteLine("Compressed version of {0} doesn't exist, compressing now for future runs . . .", filePath);

                Compression.CompressFileLZMA(filePath, compressedFilePath);
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
    }
}
