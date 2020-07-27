using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
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

        public const string HeaderExtension = "OMH";

        public const string DataExtension = "OMD";

        private string DataRoot => RootDirectory != null ? Path.Combine(RootDirectory, DataDirName) : DataDirName;

        private string EpoFile => RootDirectory != null ? Path.Combine(RootDirectory, EpoFileName) : EpoFileName;

        private string CustomSettingFile => RootDirectory != null ? Path.Combine(RootDirectory, CustomSettingsFileName) : CustomSettingFile;

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


        public OnMov200()
        {
            OutputDirectory = null;
            RootDirectory = null;
        }
        
        public OnMov200(string outputDirectory)
        {
            DetectDevice();
            OutputDirectory = outputDirectory;
        }


        public OnMov200(string rootDirectory, string outputDirectory, bool initialize = true)
        {
            if (initialize)
            {
                Initialize(rootDirectory, outputDirectory);
            }
        }


        public void Initialize(string rootDirectory, string outputDirectory = null)
        {
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
            var files = Directory.GetFiles(DataRoot, $"*.{HeaderExtension}");
            var headers = new List<ActivityHeader>();

            foreach (var file in files)
            {
                FileInfo headerFile = new FileInfo(file);
                if (File.Exists(headerFile.FullName.Replace(HeaderExtension, DataExtension)))
                {
                    var header = GetHeader(new FileInfo(file));
                    headers.Add(header);
                }
            }

            return headers;
        }


      

        public List<Either<Unit,OMError>> ExtractAll(List<ActivityHeader> activities)
        {
            var result = activities.Select(activity =>
            {
                return ExtractActivity(activity);
            }).ToList();
            return result;
        }

        public List<Either<Unit,OMError>> ExtractAll()
        {
            //List<Either<Unit,OMError>> results = new 
            
            var files = Directory.GetFiles(DataRoot, "*.OMD");
            ;

            var results = files.Map(file =>
            {
                FileInfo info = new FileInfo(file);
                string activity = info.Name.Replace(".OMD", "");
                return ExtractActivity(activity);
            }).ToList();
            
            // foreach (var file in files)
            // {
            //     FileInfo info = new FileInfo(file);
            //     string activity = info.Name.Replace(".OMD", "");
            //     ExtractActivity(activity);
            // }

            return results;
        }
        
        public ActivityHeader GetHeader(string name, Stream stream)
        {
            Dictionary<string, object> omh;
            using (stream)
            {
                omh = OnMov200Schemas.OMH.Read(stream);
            }

            var header = new ActivityHeader(omh, name.Replace($".{HeaderExtension}", ""));
            return header;
        }
        

        public ActivityHeader GetHeader(FileInfo file)
        {
            ActivityHeader header = null; 
            using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read))
            {
                header = GetHeader(file.Name, stream);
            }

            return header;
            // Dictionary<string, object> omh;
            // using (var stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read))
            // {
            //     omh = OnMov200Schemas.OMH.Read(stream);
            // }
            //
            // var header = new ActivityHeader(omh, file.Name.Replace(".OMH", ""));
            // return header;
        }

        private ActivityHeader GetHeader(string omhName)
        {
            FileInfo file = new FileInfo(Path.Combine(DataRoot, omhName + $".{HeaderExtension}"));

            return GetHeader(file);
        }


        public Either<(ActivityHeader activity, string gpx),OMError> ToGpx(ActivityHeader activity, Stream omdStream)
        {
            if (activity != null)
            {
                

                using (omdStream)
                {
                    OMDParser parser = new OMDParser();
                    try
                    {
                        var datas = parser.Parse(activity, omdStream);
                        if (datas.IsRight)
                        {
                            return datas.IfLeft(() => new OMError(activity, "no error"));
                        }
                        if (datas.IsLeft && datas.Any())
                        {
                            string gpx = GpxSerializer.Serialize(datas.IfRight(() =>
                            {
                                return new List<WayPoint>();
                            }));
                            return (activity,gpx);
                        }
                    }
                    catch (Exception e)
                    {
                        return  new OMError(activity,$"ERROR on activity {activity} : {e.Message}");
                    }
                }
            }
            return new OMError(activity,"no activity provided");
        }

        public Either<Unit, OMError> ExtractActivity(ActivityHeader activity, string outputDirectory = null)
        {
            if (activity != null)
            {
                string gpxFileName = activity.GpxFileName;

                using (var stream = File.Open(Path.Combine(DataRoot, $"{activity.Name}.OMD"), FileMode.Open,
                    FileAccess.Read))
                {
                    var result = ToGpx(activity, stream);


                    if (result.IsLeft)
                    {
                        var res = result.IfRight(x => (activity, "empty"));
                        string filename = Path.Combine(outputDirectory ?? OutputDirectory, gpxFileName);
                        File.WriteAllText(filename, res.gpx);
                        return new Unit();
                    }
                    else
                    {
                        var res = result.IfLeft(() => new OMError(activity, "no error."));
                        return res;
                    }
                }


            }
            return new OMError(activity,"no activity provided.");
        }


        public Either<Unit,OMError> ExtractActivity(string activity, string outputDirectory = null)
        {
            var header = GetHeader(activity);
            return ExtractActivity(header, outputDirectory);
        }

        static readonly HttpClient client = new HttpClient();

        public async Task UpDateFastFixIfNeeded(bool force = false)
        {
            if (force || NeedFastFixUpdate())
            {
                try
                {
                    var wc = new WebClient();
                    wc.DownloadFileAsync(new Uri(EpoUrl),EpoFile);
                    
                    long newDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                    CustomSettings.updateEPODate = newDate;
                    await WriteCustomSettings();
                }
                catch (Exception e)
                {
                    throw new Exception($"error during fastfix update : {e.Message}");
                }
            }
            else
            {
                Console.WriteLine("no fastfix update needed");
            }
        }


        public async void UpdateFastFix()
        {
            if (File.Exists(EpoFile))
            {
                File.Delete(EpoFile);
            }

            var client = new WebClient();
            await client.DownloadFileTaskAsync(EpoUrl, EpoFile);
            long newDate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            CustomSettings.updateEPODate = newDate;
            await WriteCustomSettings();
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

        private async Task WriteCustomSettings()
        {
            var settings = JsonConvert.SerializeObject(CustomSettings);
            await File.WriteAllTextAsync(CustomSettingFile, settings);
        }
    }
}