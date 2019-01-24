using System;

namespace StringSearch.Infrastructure.Health
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
