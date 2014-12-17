/*
 * String Search Library
 * Compression class
 * By Josh Keegan 06/11/2014
 * Last Edit 17/12/2014
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
