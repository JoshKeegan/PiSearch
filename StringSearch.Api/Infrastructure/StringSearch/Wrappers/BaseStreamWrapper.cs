using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Infrastructure.StringSearch.Wrappers
{
    public abstract class BaseStreamWrapper : IDisposable
    {
        public readonly Stream Stream;

        public BaseStreamWrapper(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Dispose()
        {
            Stream.Dispose();
            GC.SuppressFinalize(this);
        }

        ~BaseStreamWrapper()
        {
            Dispose();
        }
    }
}
