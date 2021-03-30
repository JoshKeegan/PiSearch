using System;
using System.IO;
using dotenv.net;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;
using SuffixArray;

namespace StringSearch.IndexGenerator
{
    public static class Program
    {
        public static int Main()
        {
            DotEnv.Load(new DotEnvOptions(probeForEnv: true));

            if (!Config.TryLoad(out Config config))
            {
                return -1;
            }
            if (Directory.Exists(config.OutputDir) && Directory.GetFiles(config.OutputDir).Length > 0)
            {
                Console.Error.WriteLine("Output directory must be empty");
                return -1;
            }

            string[] inputFiles = Directory.GetFiles(config.InputDir, "*.txt");
            Console.WriteLine($"Found {inputFiles.Length} input digits files");

            foreach (string inputFile in inputFiles)
            {
                processFile(inputFile, config);
            }

            return 0;
        }

        private static void processFile(string inputFile, Config config)
        {
            string digitName = Path.GetFileNameWithoutExtension(inputFile);
            Console.WriteLine($"Processing {digitName} . . .");

            string outputDir = Path.Combine(config.OutputDir, digitName);
            Directory.CreateDirectory(outputDir);

            // TODO: Handle the leading 3. in the y-cruncher output (currently manually removed)

            generateFourBitDigitFile(inputFile, digitName, outputDir);

            // Load the digits back into memory
            // TODO: Could keep a copy of the 4bit digit file in memory so we don't need to immediately read it back
            Stream streamDigits =
                Compression.ReadStreamNoCompression(Path.Combine(outputDir, digitName + ".4bitDigit"));
            FourBitDigitBigArray digits = new FourBitDigitBigArray(streamDigits);

            IBigArray<ulong> suffixArray = generateSuffixArray(digitName, outputDir, digits);

            // TODO: Generate precomputed search results
        }

        private static void generateFourBitDigitFile(string inputFile, string digitName, string outputDir)
        {
            Console.WriteLine($"Generating compressed 4-Bit Digit File for {digitName}");
            string fourBitDigitFile = Path.Combine(outputDir, digitName + ".4bitDigit");
            Compression.CompressFile4BitDigit(inputFile, fourBitDigitFile);
        }

        private static IBigArray<ulong> generateSuffixArray(string digitName, string outputDir, FourBitDigitBigArray digits)
        {
            Console.WriteLine($"Generating suffix array for {digitName} . . .");
            MemoryEfficientBigULongArray suffixArray = new MemoryEfficientBigULongArray(digits.Length);

            // For generation, the algorithm will generate intermediate values outside of the bounds we will store in the 
            //  final suffix array. Since we keep the number of bits per digit to a minimum in the generated suffix 
            //  array, it would overflow.
            // So to ger around this for generation, we use the complement array rather than just making the suffix array
            //  have to have 64 bits per value as this is far more memory efficient.
            // The complements can (and will) be thrown away after the suffix array is generated, leaving the generated suffix array
            //  in the memory efficient format we want.
            IBigArray<bool> underlyingComplementArray = new BigBoolArray(digits.Length);
            IBigArray<ulong> workingSuffixArray = new MemoryEfficientComplementBigULongArray(digits.Length,
                (ulong)digits.Length, suffixArray, underlyingComplementArray);

            long status = SAIS.sufsort(digits, workingSuffixArray, digits.Length);

            if (status != 0)
            {
                throw new Exception($"An error occurred whilst generating the suffix array. Status {status}");
            }

            // TODO: Verify suffix array (see StringSearchConsole.Program:589)

            Console.WriteLine("Suffix array generated, writing to file . . .");
            Compression.WriteStreamNoCompression(suffixArray.Stream,
                Path.Combine(outputDir, digitName + ".suffixArray.bitAligned"));

            return suffixArray;
        }
    }
}
