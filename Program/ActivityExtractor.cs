using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using onmov200.gpx;
using onmov200.model;
using onmov200.parser;

namespace program
{
    public class ActivityExtractor
    {

        public static void PrintSummary(string root)
        {
            var files = Directory.GetFiles(root, "*.OMH");
            ;
            
            foreach (var file in files)
            {
                var header = GetHeader(file);
                FileInfo f = new FileInfo(file);
                Console.WriteLine($"{f.Name.Replace(".OMH","")} {header.ToString()}");
            }
        }

        public static void ExtractAll(string root)
        {
            var files = Directory.GetFiles(root, "*.OMD");
            ;
            
            foreach (var file in files)
            {
                FileInfo info = new FileInfo(file);
                string activity = info.Name.Replace(".OMD", "");
                ExtractActivity(root, activity);
            }
        }

        public static ActivityHeader GetHeader(string fileName)
        {
            Dictionary<string, object> omh;
            using (var stream = File.Open(fileName, FileMode.Open))
            {
                omh = OnMov200Schemas.OMH.Read(stream);
            }

            var header = new ActivityHeader(omh);
            return header;
        }
        public static void ExtractActivity(string root, string activity)
        {
            var header = GetHeader(Path.Combine(root, $"{activity}.OMH"));

            var stream = File.Open(Path.Combine(root,$"{activity}.OMD"), FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                var datas = parser.Parse(stream, header.DateTime);
                if (datas != null && datas.Any())
                {
                    GpxSerializer.Serialize(datas, Path.Combine(root,$"{activity}.gpx"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR on activity {activity} : {e.Message}");
            }
        }
    }
}