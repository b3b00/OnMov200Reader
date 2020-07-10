using System;
using System.IO;
using System.Linq;
using ByteReader;

namespace porgram
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var t = CommonReaders.ToFloat(new byte[] {0x64, 0x00, 0x00, 0x00});
            ;
            
            
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ USER                ___");
            Console.WriteLine("______________________________");
            
            Stream stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\USER.BIN",FileMode.Open);
            var result = OnMov200Schemas.USER.Read(stream);
            foreach (var r in result)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }
            stream.Close();
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMH                    ___");
            Console.WriteLine("______________________________");
            
            stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\DATA\ACT_0003.OMH",FileMode.Open);
            result = OnMov200Schemas.OMH.Read(stream);
            foreach (var r in result)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMD                    ___");
            Console.WriteLine("______________________________");
            
            stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\DATA\ACT_0003.OMD",FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                var datas = parser.Parse(stream);
                if (datas != null && datas.Any())
                {
                    GpxSerializer.Serialize(datas,"./gpx.gpx");
                }
            }
            catch (Exception e)
            {
                ;
            }

            ;
        }
    }
}