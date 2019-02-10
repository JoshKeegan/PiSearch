using System;
using System.IO;

namespace StringSearch.NamedDigits
{
    public class ObjectWithStream<T> : IDisposable
    {
        private Stream stream;

        public readonly T Object;

        public ObjectWithStream(T o, Stream stream)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            Object = o;
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Dispose()
        {
            stream?.Dispose();
            stream = null;
        }

        ~ObjectWithStream()
        {
            Dispose();
        }
    }
}
