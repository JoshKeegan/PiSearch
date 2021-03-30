using System;

namespace StringSearch.IndexGenerator
{
    public class Config
    {
        private const string ENV_INPUT_DIR = "INPUT_DIGITS_DIR";
        private const string ENV_OUTPUT_DIR = "OUTPUT_DIR";

        public readonly string InputDir;
        public readonly string OutputDir;

        public Config(string inputDir, string outputDir)
        {
            InputDir = inputDir;
            OutputDir = outputDir;
        }

        public static bool TryLoad(out Config config)
        {
            config = null;

            string inputDir = Environment.GetEnvironmentVariable(ENV_INPUT_DIR);
            string outputDir = Environment.GetEnvironmentVariable(ENV_OUTPUT_DIR);

            if (string.IsNullOrWhiteSpace(inputDir))
            {
                Console.Error.WriteLine($"{ENV_INPUT_DIR} environment variable not set");
                return false;
            }

            if (string.IsNullOrWhiteSpace(outputDir))
            {
                Console.Error.WriteLine($"{ENV_OUTPUT_DIR} environment variable not set");
                return false;
            }

            config = new Config(inputDir, outputDir);
            return true;
        }
    }
}
