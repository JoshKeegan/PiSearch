using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StringSearch;
using StringSearch.Legacy;

namespace UnitTests.TestObjects.Extensions
{
    public static class StringToStreamExtensions
    {
        public static Stream ToFourBitDigitStream(this string str)
        {
            // Write string to stream
            using (MemoryStream uncompressedStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(uncompressedStream);
                writer.Write(str);

                // Just flush the writer here to ensure all the data is written to the stream.
                //  It cannot be closed/disposed as that will also close the underlying stream
                writer.Flush();

                // Reset the stream position back to the beginning, as the compressor will run from
                //  the current position
                uncompressedStream.Position = 0;

                // Stream to hold compressed 4 bit digit output
                MemoryStream outStream = new MemoryStream();

                Compression.CompressStream4BitDigit(uncompressedStream, outStream);
                return outStream;
            }
        }
    }
}
