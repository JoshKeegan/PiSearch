/*
 * PiSearch
 * UnderlyingStream - Abstract class for classes using an underlying stream that they want to expose.
 *  Having them all implement this means they can have common methods for saving or loading
 *  the underlying stream
 * By Josh Keegan 30/12/2014
 */

using System.IO;

namespace StringSearch.Legacy.Collections
{
    public abstract class UnderlyingStream
    {
        public Stream Stream { get; set; }
    }
}
