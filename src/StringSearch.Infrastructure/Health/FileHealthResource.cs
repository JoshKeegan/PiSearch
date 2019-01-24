using System;
using System.IO;
using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch.Infrastructure.Health
{
    public class FileHealthResource : IHealthResource
    {
        private readonly string path;
        private readonly FileMode fileMode;
        private readonly FileAccess fileAccess;

        public string Name { get; }
        public bool Critical { get; }

        public FileHealthResource(string name, bool critical, string path, FileMode fileMode = FileMode.Open,
            FileAccess fileAccess = FileAccess.Read)
        {
            Name = name;
            Critical = critical;
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.fileMode = fileMode;
            this.fileAccess = fileAccess;
        }
        
        public async Task<HealthState> CheckState()
        {
            // Open the file as part of the health check.
            //    Relying upon existence alone may miss problems with permissions, or other oddities
            bool healthy = true;
            string message = null;
            try
            {
                using (Stream s = new FileStream(path, fileMode, fileAccess, FileShare.ReadWrite, 1, true))
                {
                    await s.ReadAsync(new byte[1], 0, 1);
                }
            }
            catch (Exception e)
            {
                healthy = false;
                message = e.Message;
            }
            return new HealthState(healthy, message);
        }
    }
}