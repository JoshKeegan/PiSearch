using System;
using System.IO;
using System.Linq;

namespace StringSearch
{
    public class PrecomputedSearchResultsFilePaths
    {
        public readonly string[] Paths;

        public PrecomputedSearchResultsFilePaths(string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }
            Paths = paths.OrderBy(p => int.Parse(Path.GetFileNameWithoutExtension(p))).ToArray();
        }
    }
}
