using System;
using System.IO;
using System.Linq;
using onmov200.parser;
using onmov200.gpx;


namespace porgram
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ USER                ___");
            Console.WriteLine("______________________________");
            
            Stream stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\USER.BIN",FileMode.Open);
            var user = OnMov200Schemas.USER.Read(stream);
            foreach (var r in user)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }
            stream.Close();
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMH                    ___");
            Console.WriteLine("______________________________");
            
            stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\DATA\ACT_0003.OMH",FileMode.Open);
            var omh = OnMov200Schemas.OMH.Read(stream);
            foreach (var r in omh)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }

            DateTime startTime = new DateTime(2000+(int) omh["year"], (int) omh["month"], (int) omh["day"],
                (int) omh["hour"], (int) omh["min"],0);
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMD                    ___");
            Console.WriteLine("______________________________");
            
            stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\DATA\ACT_0003.OMD",FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                
                var datas = parser.Parse(stream, startTime);
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