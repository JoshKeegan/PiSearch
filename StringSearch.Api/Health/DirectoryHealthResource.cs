using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Health
{
    public class DirectoryHealthResource : BaseDirectoryHealthResource
    {
        public DirectoryHealthResource(string name, bool critical, string path) 
            : base(name, critical, path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
        }
    }
}
