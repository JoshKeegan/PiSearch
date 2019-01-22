/*
 * PiSearch
 * Compression class
 * By Josh Keegan 06/11/2014
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSearch.IO;
using StringSearch.Collections;

namespace StringSearch
{
    public static class Compression
    {
        //Constants
        private const int STREAM_COPY_BUFFER_SIZE = 16 * 1024 * 1024; //Read/Write 16MiB at a time for quicker copying of large streams (default is 16KiB)

        /*
         * Compress
         */
        public static void CompressFile4BitDigit(string inPath, string outPath)
        {
            using (FileStream inStream = new FileStream(inPath, FileMode.Open, FileAccess.Read))
            using (FileStream outStream = new FileStream(outPath, FileMode.Create))
            {
                CompressStream4BitDigit(inStream, outStream);
            }
        }

        public static void CompressStream4BitDigit(Stream inStream, Stream outStream)
        {
            StreamReader reader = new StreamReader(inStream);
            
            //Read in 2 characters that will be outputted as 8 bits = 1 byte (4 bits per char)
            char[] charBuffer = new char[2];
            while (!reader.EndOfStream)
            {
                int charsRead = reader.Read(charBuffer, 0, 2);

                //Convert each char to a byte
                byte char1 = byte.Parse(charBuffer[0].ToString());
                byte char2 = 15; //1111 => null

                if (charsRead == 2)
                {
                    char2 = byte.Parse(charBuffer[1].ToString());
                }

                byte combined = (byte)(char1 << 4);
                combined = (byte)(combined | char2);

                outStream.WriteByte(combined);
            }

            // Flush data buffer to be written to the stream
            outStream.Flush();
        }

        public static void WriteStreamNoCompression(Stream stream, string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            //Start reading the stream in from the beginning
            stream.Position = 0;

            stream.CopyTo(fileStream, STREAM_COPY_BUFFER_SIZE);

            //Clean up
            fileStream.Close();
        }

        public static void WriteStreamNoCompression(UnderlyingStream obj, string filePath)
        {
            WriteStreamNoCompression(obj.stream, filePath);
        }

        /*
         * Decompress
         */
        public static string ReadStringNoCompression(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);

            //Read in the string 1024 characters at a time
            StringBuilder builder = new StringBuilder();

            char[] buffer = new char[1024];
            while(!reader.EndOfStream)
            {
                int charsRead = reader.Read(buffer, 0, 1024);

                builder.Append(buffer, 0, charsRead);
            }

            //Clean up
            reader.Close();

            return builder.ToString();
        }

        public static Stream ReadStreamNoCompression(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Stream memStream;
            //If this stream being loaded is too large for MemoryStream, use BigMemoryStream
            if(fileStream.Length > int.MaxValue)
            {
                memStream = new BigMemoryStream(fileStream.Length);
            }
            else //Otherwise use the standard MemoryStream implementation which is faster
            {
                memStream = new MemoryStream();
            }

            fileStream.CopyTo(memStream, STREAM_COPY_BUFFER_SIZE);

            //Clean up
            fileStream.Close();

            return memStream;
        }
    }
}
