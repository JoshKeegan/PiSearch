using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Infrastructure.Config
{
    public class StringSearchConfig
    {
        public string RootPath { get; set; }
        public string RelativeDigitsPath { get; set; }
        public string RelativeSuffixArrayPath { get; set; }
        public string RelativePrecomputedResultsDirPath { get; set; }
        public string PrecomputedResultsFileExtension { get; set; }

        public string AbsoluteDigitsPath => Path.Combine(RootPath, RelativeDigitsPath);
        public string AbsoluteSuffixArrayPath => Path.Combine(RootPath, RelativeSuffixArrayPath);
        public string AbsolutePrecomputedResultsDirPath => Path.Combine(RootPath, RelativePrecomputedResultsDirPath);
    }
}
