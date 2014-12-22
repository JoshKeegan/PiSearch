using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSearch;
using StringSearch.Collections;
using SuffixArray;

namespace StringSearchConsole
{
    public class Program
    {
        //Variables
        private static string workingDirectory = "";
        private static string loadedString = null;
        private static Stream loaded4BitDigitStream = null;
        private static FourBitDigitBigArray fourBitDigitArray = null;
        private static BigArray<ulong> suffixArray = null;
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
                "2.\tLoad text file (one continuous string of digits)\n" + 
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
                "17.\tConvert y-cruncher output to raw decimal places of pi\n" +
                "18.\tTake first n digits from compressed 4-bit digit file\n" +
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

            suffixArray = new MemoryEfficientBigULongArray(len, (uint)len);

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

                    loaded4BitDigitStream = Compression.ReadStreamNoComression(workingDirectory + fileName);

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

                long[] foundIdxs = SearchString.Search(suffixArray, fourBitDigitArray, toFind);
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

        private static void subGenerateSuffixArray()
        {
            //Initialise the array that will hold the suffix array
            MemoryEfficientBigULongArray suffixArray = new MemoryEfficientBigULongArray(fourBitDigitArray.Length);

            //Calculate the suffix array
            long status = SAIS.sufsort(fourBitDigitArray, suffixArray, fourBitDigitArray.Length);

            if(status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
            else
            {
                Console.WriteLine("Using generated Suffix Array");
                Program.suffixArray = suffixArray;
            }
        }

        private static void subGenerateSuffixArrayFromLoadedString()
        {
            throw new NotImplementedException("SAIS.sufsort needs implementing again for Strings");
            /*//Initialise the aray that will hold the suffix array
            int[] suffixArray = new int[loadedString.Length];

            //Calculate the suffix array
            int status = SAIS.sufsort(loadedString, suffixArray, loadedString.Length);

            if (status != 0)
            {
                Console.WriteLine("Error occurred whilst generating the suffix array: {0}", status);
            }
            else
            {
                Console.WriteLine("Converting generated int[] suffix array to BigArray<ulong>");
                Program.suffixArray = convertIntArrayToBigUlongArray(suffixArray);
            }*/
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

        internal static BigArray<ulong> convertIntArrayToBigUlongArray(int[] arr)
        {
            BigArray<ulong> toRet = new MemoryEfficientBigULongArray(arr.Length, (uint)arr.Length);

            for(int i = 0; i < arr.Length; i++)
            {
                toRet[i] = (uint)arr[i];
            }

            return toRet;
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
