using System;
using System.IO;
using System.Linq;
using onmov200.parser;
using onmov200.gpx;
using onmov200.model;


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
            var header = new ActivityHeader(omh);

            

            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMD                    ___");
            Console.WriteLine("______________________________");

            stream = File.Open($@"{root}\{activity}.OMD", FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                var datas = parser.Parse(stream, header.DateTime);
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