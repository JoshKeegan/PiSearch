using System;
using System.IO;

namespace StringSearch.LegacyWrappers
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
