using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;

namespace ByteReader
{
    public class CommonReaders 
    {

        public static float ToFloat(byte[] input)
        {
            byte[] newArray = new[] { input[2], input[3], input[0], input[1] };
            var f =  BitConverter.ToSingle(newArray, 0);
            var f2 =  BitConverter.ToSingle(input, 0);
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
            int Int = buffer[0] | ( (int)buffer[1] << 8 );
            return Int;
        }
        
        public static  object ReadInt4(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            int Int = buffer[0] | ( (int)buffer[1] << 8 ) | ( (int)buffer[2] << 16 ) | ( (int)buffer[3] << 24 );
            return Int;
        }
        
        public static  object ReadInt1(Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            int Int = buffer[0];
            return Int;
        }

        public static Schema.Reader Reserved(int length)
        {
            return (Stream stream) =>
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
    
   
    
    
    
    public class Schema
    {
        public delegate object Reader(Stream stream);

        public List<(string name, Reader reader)> Readers;
        
        public string Name { get; set; }

        public Schema(string name)
        {
            name = name;
            Readers = new List<(string name, Reader reader)>();
        }
        public Schema AddLong(string name)
        {
            Readers.Add((name,CommonReaders.ReadInt4));
            return this;
        }
        
        
        public Schema AddInt(string name)
        {
            Readers.Add((name,CommonReaders.ReadInt2));
            return this;
        } 
        
        
        
        public Schema AddByte(string name)
        {
            Readers.Add((name,CommonReaders.ReadInt1));
            return this;
        }
        
        public Schema AddFloat(string name)
        {
            Readers.Add((name,CommonReaders.ReadFloat));
            return this;
        }
        
        public Schema AddFloat2(string name)
        {
            Readers.Add((name,CommonReaders.ReadFloat2));
            return this;
        }

        public Schema AddReserved(string name, int length)
        {
            Readers.Add((name,CommonReaders.Reserved(length)));
            return this;
        }

        public Dictionary<string, object> Read(Stream stream)
        {
            var result = new Dictionary<string, object>();
            foreach (var reader in Readers)
            {
                // long position = stream.Position;
                //     Console.Write($"{reader.name} @{position}");
                var value = reader.reader(stream);
                // Console.WriteLine($" :: {value}");
                result[reader.name] = value;
            }

            return result;
        }
    }
}