using System.IO;

namespace StringSearch.Config
{
    public class DigitsConfig
    {
        public string RootPath { get; set; }
        public string RelativeDigitsPath { get; set; }
        public string RelativeSuffixArrayPath { get; set; }
        public string RelativePrecomputedResultsDirPath { get; set; }
        public string PrecomputedResultsFileExtension { get; set; }

        public string AbsoluteDigitsPath => Path.Combine(RootPath, RelativeDigitsPath);
        public string AbsoluteSuffixArrayPath => Path.Combine(RootPath, RelativeSuffixArrayPath);
        public string AbsolutePrecomputedResultsDirPath => RelativePrecomputedResultsDirPath != null
            ? Path.Combine(RootPath, RelativePrecomputedResultsDirPath)
            : null;
    }
}
