using System;
using System.IO;

namespace ByteReader
{
    public class CommonReaders 
    {

        public static float ToFloat(byte[] input)
        {
            byte[] newArray = new[] { input[2], input[3], input[0], input[1] };
            var f =  BitConverter.ToSingle(newArray, 0);
            return f;
        }
        
        public static object ReadFloat(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            var value =  ToFloat(buffer);
            return value;
        }
        
        public static object ReadFloat2(Stream stream)
        {
            int val = (int)ReadInt2(stream) ;
            return val / 100.0d;
        }
        public static  object ReadInt2(Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            int int2 = buffer[0] |  (buffer[1] << 8);
            return int2;
        }
        
        public static  object ReadInt4(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            int int4 = buffer[0] | ( buffer[1] << 8 ) | ( buffer[2] << 16 ) | ( buffer[3] << 24 );
            return int4;
        }
        
        public static  object ReadInt1(Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            int int1 = buffer[0];
            return int1;
        }

        public static Schema.Reader Reserved(int length)
        {
            return (stream) =>
            {
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, length);
                return null;
            };
        }

        public static object Float(Stream stream)
        {

           
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            var d = BitConverter.ToDouble(buffer, 0);
            return d;
        }
    }
}