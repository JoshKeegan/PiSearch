/*
 * String Search Library
 * Compression class
 * By Josh Keegan 06/11/2014
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch
{
    public class Compression
    {
        /*
         * Compress
         */
        public static void CompressFileLZMA(string inPath, string outPath)
        {
            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            FileStream inStream = new FileStream(inPath, FileMode.Open);
            FileStream outStream = new FileStream(outPath, FileMode.Create);

            // Write the encoder properties
            encoder.WriteCoderProperties(outStream);

            // Write the decompressed file size.
            outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

            // Encode the file.
            encoder.Code(inStream, outStream, inStream.Length, -1, null);
            inStream.Close();
            outStream.Flush();
            outStream.Close();
        }

        public static void CompressFile4BitDigit(string inPath, string outPath)
        {
            StreamReader reader = new StreamReader(inPath);
            FileStream outStream = new FileStream(outPath, FileMode.Create);

            //Read in 2 characters that will be outputted as 8 bits = 1 byte (4 bits per char)
            char[] charBuffer = new char[2];
            int charsRead;
            while(!reader.EndOfStream)
            {
                charsRead = reader.Read(charBuffer, 0, 2);

                //Convert each char to a byte
                byte char1 = byte.Parse(charBuffer[0].ToString());
                byte char2 = 15; //1111 => null

                if(charsRead == 2)
                {
                    char2 = byte.Parse(charBuffer[1].ToString());
                }

                byte combined = (byte)(char1 << 4);
                combined = (byte)(combined | char2);

                outStream.WriteByte(combined);
            }

            //Clean up
            reader.Close();
            outStream.Close();
        }

        /*
         * Decompress
         */
        public static string ReadStringLZMA(string filePath)
        {
            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            FileStream inStream = new FileStream(filePath, FileMode.Open);

            //Read the decoder properties
            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            //Read in the decompressed file size
            byte[] fileSizeBytes = new byte[8];
            inStream.Read(fileSizeBytes, 0, 8);
            long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

            //Store the output into a memory stream
            MemoryStream outStream = new MemoryStream();

            decoder.SetDecoderProperties(properties);
            decoder.Code(inStream, outStream, inStream.Length, fileSize, null);

            //Clean up the open file
            inStream.Close();

            //Read the output stream from the beginning into a string
            outStream.Position = 0;
            StreamReader reader = new StreamReader(outStream);
            string toRet = reader.ReadToEnd();

            //Clean Up
            reader.Close();
            outStream.Close();

            return toRet;
        }

        public static string ReadStringNoCompression(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);

            //Read in the string 1024 characters at a time
            StringBuilder builder = new StringBuilder();

            char[] buffer = new char[1024];
            int charsRead;
            while(!reader.EndOfStream)
            {
                charsRead = reader.Read(buffer, 0, 1024);

                builder.Append(buffer, 0, charsRead);
            }

            //Clean up
            reader.Close();

            return builder.ToString();
        }

        public static Stream ReadStreamNoComression(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            MemoryStream memStream = new MemoryStream();

            fileStream.CopyTo(memStream);

            //Clean up
            fileStream.Close();

            return memStream;
        }
    }
}
