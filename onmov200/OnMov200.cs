using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using onmov200.gpx;
using onmov200.model;
using onmov200.parser;

namespace onmov200
{
    public class OnMov200
    {
        public const string EpoUrl = "https://s3-eu-west-1.amazonaws.com/ephemeris/epo.7";

        public const string CustomSettingsFileName = "ONCONNECT.JSON";

        public const int MaxDays = 7;

        public const string DataDirName = "DATA";

        public const string EpoFileName = "epo.7";

        private string DataRoot => Path.Combine(RootDirectory, DataDirName);

        private string EpoFile => Path.Combine(RootDirectory, EpoFileName);

        private string CustomSettingFile => Path.Combine(RootDirectory, CustomSettingsFileName);

        public string RootDirectory { get; private set; }

        public string OutputDirectory { get; set; }

        private CustomSettings CustomSettings;

        public void DetectDevice(string rootDir = null)
        {
            if (string.IsNullOrEmpty(rootDir))
            {
                var drives = DriveInfo.GetDrives();
                var drive = Array.Find(drives, x =>
                    x.VolumeLabel.Contains("onmove-200", StringComparison.InvariantCultureIgnoreCase));
                if (drive != null)
                {
                    RootDirectory = drive.RootDirectory.FullName;
                    OutputDirectory = Environment.CurrentDirectory;
                }
                else
                {
                    // TODO 
                }
            }
            else
            {
                RootDirectory = rootDir;
            }
        }

        public string Detect(int tries)
        {
            string directory = null;
            int i = 0;
            while (i < tries && directory == null)
            {
                var drives = DriveInfo.GetDrives();
                var drive = Array.Find(drives, x =>
                    x.VolumeLabel.Contains("onmove-200", StringComparison.InvariantCultureIgnoreCase));
                if (drive != null)
                {
                    directory = drive.RootDirectory.FullName;
                }
                Thread.Sleep(500);
                i++;
            }
            return directory;
        }

        public OnMov200(string outputDirectory)
        {
            DetectDevice();
            OutputDirectory = outputDirectory;
        }


        public OnMov200(string rootDirectory, string outputDirectory, bool initialize = true)
        {
            if (initialize) {
                Initialize(rootDirectory,outputDirectory);
            }
        }


        public void Initialize(string rootDirectory, string outputDirectory = null) {

            DetectDevice(rootDirectory);

            if (string.IsNullOrEmpty(outputDirectory))
                {
                    OutputDirectory = Environment.CurrentDirectory;
                }
                else
                {
                    OutputDirectory = outputDirectory;
                }
                CustomSettings = ReadCustomSettings();
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


        public void ExtractAll(List<ActivityHeader> activities)
        {
            foreach (var activity in activities)
            {
                ExtractActivity(activity);
            }
        }

        public void ExtractAll()
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
            using (var stream = File.Open(file.FullName, FileMode.Open,FileAccess.Read))
            {
                omh = OnMov200Schemas.OMH.Read(stream);
            }

            var header = new ActivityHeader(omh, file.Name.Replace(".OMH", ""));
            return header;
        }

        private ActivityHeader GetHeader(string omhName)
        {
            FileInfo file = new FileInfo(Path.Combine(DataRoot, omhName + ".OMH"));

            return GetHeader(file);
        }

        public void ExtractActivity(ActivityHeader activity, string outputDirectory = null)
        {
            if (activity != null)
            {
                string name = activity.DateTime.ToString("yyyyMmddhhmm");

                using (var stream = File.Open(Path.Combine(DataRoot, $"{activity.Name}.OMD"), FileMode.Open,FileAccess.Read))
                {
                    OMDParser parser = new OMDParser();
                    try
                    {
                        var datas = parser.Parse(stream, activity.DateTime);
                        if (datas != null && datas.Any())
                        {
                            GpxSerializer.Serialize(datas,
                                Path.Combine(outputDirectory ?? OutputDirectory, $"{name}.gpx"));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR on activity {activity} : {e.Message}");
                    }
                }
            }
        }

        public void ExtractActivity(string activity, string outputDirectory = null)
        {
            var header = GetHeader(activity);
            ExtractActivity(header,outputDirectory);
            
        }

        static readonly HttpClient client = new HttpClient();

        public async Task UpDateFastFixIfNeeded(bool force = false)
        {
            if (force || NeedFastFixUpdate())
            {

                try
                {

                    var wc = new WebClient();
                    wc.DownloadFile(EpoUrl, EpoFile);


                    long newDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                    CustomSettings.updateEPODate = newDate;
                    WriteCustomSettings();
                    // }
                    // }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error during fastfix update : " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("no fastfix update needed");
            }

        }


        public void UpdateFastFix()
        {
            if (File.Exists(EpoFile))
            {
                File.Delete(EpoFile);
            }

            var client = new WebClient();
            client.DownloadFile(EpoUrl, EpoFile);
            long newDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            CustomSettings.updateEPODate = newDate;
            WriteCustomSettings();
        }

        public bool NeedFastFixUpdate()
        {
            var now = DateTime.UtcNow;
            long milliseconds = new DateTimeOffset(now).ToUnixTimeMilliseconds();
            if (CustomSettings != null)
            {
                long diff = milliseconds - CustomSettings.updateEPODate;

                long maxMillis = 60 * 60 * 1000 * 24 * MaxDays;

                bool tooLong = diff > maxMillis;

                return tooLong;
            }

            return true;
        }

        private CustomSettings DefaultSettings()
        {
            return new CustomSettings()
            {
                updateEPODate = 0
            };
        }

        private CustomSettings ReadCustomSettings()
        {
            if (File.Exists(CustomSettingFile))
            {
                try
                {
                    string content = File.ReadAllText(CustomSettingFile);
                    if (!string.IsNullOrEmpty(content))
                    {
                        var settings = JsonConvert.DeserializeObject<CustomSettings>(content);
                        return settings;
                    }
                    else
                    {
                        return DefaultSettings();
                    }
                }
                catch (Exception e)
                {
                    return DefaultSettings();
                }
            }
            else
            {
                return DefaultSettings();
            }
        }

        private void WriteCustomSettings()
        {
            var settings = JsonConvert.SerializeObject(CustomSettings);
            File.WriteAllText(CustomSettingFile, settings);
        }


    }
}