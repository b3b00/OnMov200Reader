using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;

namespace ByteReader
{
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
        
        public Schema AddLongGPS(string name)
        {
            Readers.Add((name,CommonReaders.ReadGPS));
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