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
            if (args.Length < 1)
            {
                Environment.Exit(1);
            }
            
            

            string root = args[0];

            var files = Directory.GetFiles(root, "*.OMD");
            ;
            

            foreach (var file in files)
            {
                FileInfo info = new FileInfo(file);
                string activity = info.Name.Replace(".OMD", "");
                ExtractActivity(root, activity);
            }
            
            
            

            ;
        }

        private static void ExtractActivity(string root, string activity)
        {
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMH                    ___");
            Console.WriteLine("______________________________");

            var stream = File.Open($@"{root}\{activity}.OMH", FileMode.Open);
            var omh = OnMov200Schemas.OMH.Read(stream);
            foreach (var r in omh)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }

            DateTime startTime = new DateTime(2000 + (int) omh["year"], (int) omh["month"], (int) omh["day"],
                (int) omh["hour"], (int) omh["min"], 0);

            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMD                    ___");
            Console.WriteLine("______________________________");

            stream = File.Open($@"{root}\{activity}.OMD", FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                var datas = parser.Parse(stream, startTime);
                if (datas != null && datas.Any())
                {
                    GpxSerializer.Serialize(datas, $"./{activity}.gpx");
                }
            }
            catch (Exception e)
            {
                ;
            }
        }
    }
}