using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using onmov200.gpx;
using onmov200.model;
using onmov200.parser;

namespace onmov200
{
    public class OnMov200
    {

        private string Root;

        private string OutputDir;

        private string DataRoot => Path.Combine(Root, "DATA");

        private string EPOFile => Path.Combine(Root, "epo.7");
        
        
        public OnMov200(string root, string outputDir)
        {
            Root = root;
            OutputDir = outputDir;
        }

        public void PrintSummary()
        {
            var headers = GetHeaders();
            foreach (var header in headers)
            {
                Console.WriteLine($"{header.Name} {header.ToString()}");
            }
        }

        public List<ActivityHeader> GetHeaders()
        {
            var files = Directory.GetFiles(DataRoot, "*.OMH");
            var headers = new List<ActivityHeader>();
            
            foreach (var file in files)
            {
                var header = GetHeader(new FileInfo(file));
                headers.Add(header);
            }

            return headers;
        }
        

        public  void ExtractAll()
        {
            var files = Directory.GetFiles(DataRoot, "*.OMD");
            ;
            
            foreach (var file in files)
            {
                FileInfo info = new FileInfo(file);
                string activity = info.Name.Replace(".OMD", "");
                ExtractActivity(activity);
            }
        }

        private ActivityHeader GetHeader(FileInfo file)
        {
            Dictionary<string, object> omh;
            using (var stream = File.Open(file.FullName, FileMode.Open))
            {
                omh = OnMov200Schemas.OMH.Read(stream);
            }

            var header = new ActivityHeader(omh, file.Name.Replace(".OMH",""));
            return header;
        }

        private  ActivityHeader GetHeader(string omhName)
        {
            FileInfo file = new FileInfo(Path.Combine(DataRoot,omhName+".OMH"));
            
            return GetHeader(file);
        }
        public void ExtractActivity(string activity)
        {
            var header = GetHeader(activity);

            var stream = File.Open(Path.Combine(DataRoot,$"{activity}.OMD"), FileMode.Open);
            OMDParser parser = new OMDParser();
            try
            {
                var datas = parser.Parse(stream, header.DateTime);
                if (datas != null && datas.Any())
                {
                    GpxSerializer.Serialize(datas, Path.Combine(OutputDir,$"{activity}.gpx"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR on activity {activity} : {e.Message}");
            }
        }
    }
}