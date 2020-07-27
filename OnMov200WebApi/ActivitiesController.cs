using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using onmov200;
using onmov200.model;

namespace OnMov200WebApi
{
    [ApiController]
    public class ActivitiesController : Controller
    {


        [HttpGet("/activities/say")]
        public string Say()
        {
            return "activities controller is working";
        }
        
        [HttpGet("/activities/dl/{id}")]
        public async Task<IActionResult> GetActivity(string id)
        {
            var (activity,content) = ActivityStorage.getContent(id);
            var summaries = new List<ExtractionSummary>();
            var sum = new ExtractionSummary(activity,id);
            summaries.Add(sum);
            var returnValue = ZipActivities(summaries);
            return File(returnValue.bytes, returnValue.mimeType, returnValue.name);
            ;
            return Ok();
        }
        

        [HttpPost("/activities/sumup")]
        public async Task<List<ExtractionSummary>> SumUp()
        {
            var files = HttpContext.Request.Form.Files.ToList();
            var summaries = ProcessActivities(files);
            return summaries;
        }


        private List<ExtractionSummary> ProcessActivities(List<IFormFile> files)
        {
            List<ExtractionSummary> summary = new List<ExtractionSummary>();
            
            var onmov = new OnMov200();
            var validFiles = files.Where(f =>
                f.FileName.EndsWith(OnMov200.HeaderExtension) || f.FileName.EndsWith(OnMov200.DataExtension));
            if (validFiles.Count() % 2 != 0)
            {
                throw new Exception("even file number expected");
            }

            var groupedActivitiesFiles = validFiles.GroupBy(f => f.NameWithoutExtension()).ToList();

            int countOk = 0;
            var extracted = new Dictionary<ActivityHeader, string>();
            int countKo = 0;
            var errors = new Dictionary<string,string>();

            foreach (var activityFiles in groupedActivitiesFiles)
            {
                var headerFile = activityFiles.FirstOrDefault(f => f.Extension() == $".{OnMov200.HeaderExtension}");
                if (headerFile != null)
                {
                    string name = headerFile.NameWithoutExtension();
                    ActivityHeader header = null;
                    using (var stream = headerFile.OpenReadStream())
                    {
                        header = onmov.GetHeader(name, stream);
                    }

                    if (header != null)
                    {
                        var dataFile = activityFiles.FirstOrDefault(f => f.Extension() == $".{OnMov200.DataExtension}");
                        if (dataFile != null)
                        {
                            using (var stream = dataFile.OpenReadStream())
                            {
                                var result = onmov.ToGpx(header, stream);
                                if (result.IsLeft)
                                {
                                    var res = result.IfRight(() => (header, null));
                                    extracted[res.activity] = res.gpx;
                                    string id = ActivityStorage.AddContent(res.activity,res.gpx);
                                    var sum = new ExtractionSummary(res.activity, id);
                                    summary.Add(sum);
                                    countOk++;
                                }
                                else
                                {
                                    countKo++;
                                    var error = result.IfLeft(() => new OMError(header, "no error"));

                                    var sum = new ExtractionSummary(header.Name, error.ErrorMessage);
                                    summary.Add(sum);

                                }
                            }
                        }
                        else
                        {
                            countKo++;
                            
                            var sum = new ExtractionSummary(header.Name, "unable to find data file.");
                            summary.Add(sum);
                        }
                    }
                    else
                    {
                        countKo++;

                        var sum = new ExtractionSummary(header.Name, " bad header file.");
                        summary.Add(sum);

                    }

                }
                else
                {
                    countKo++;
                    var sum = new ExtractionSummary(activityFiles.Key, " unable to find header file.");
                    
                }
            }
            
            
            
            return summary;
        }


        [HttpPost("/activities/upload")]
        public async Task<IActionResult> Upload()
        {
            var files = HttpContext.Request.Form.Files.ToList();


            var summaries = ProcessActivities(files);

            var returnValue = ZipActivities(summaries);
            return File(returnValue.bytes, returnValue.mimeType, returnValue.name);

        }

        
        private (byte[] bytes, string mimeType, string name) ZipActivities(List<ExtractionSummary> summary)
        {
            if (summary.Count > 1)
            {

                MemoryStream zipStream = new MemoryStream();

                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (var activity in summary)
                    {
                        if (activity.Ok)
                        {
                            ZipArchiveEntry gpxEntry = archive.CreateEntry(activity.Activity.GpxFileName);
                            using (StreamWriter writer = new StreamWriter(gpxEntry.Open()))
                            {
                                writer.Write(ActivityStorage.getContent(activity.Id));
                            }
                        }
                        else
                        {
                            ZipArchiveEntry gpxEntry = archive.CreateEntry(activity.Name + ".error.log");
                            using (StreamWriter writer = new StreamWriter(gpxEntry.Open()))
                            {
                                writer.Write(activity.Summary);
                            }
                        }
                    }
                }

                if (zipStream.CanSeek)
                {
                    zipStream.Position = 0;
                }

                return (zipStream.ToArray(),"application/zip","gpx.zip");
            }
            else
            {
                if (summary.Count == 1)
                {
                    var gpx = summary.First();
                    var (activity,content) = ActivityStorage.getContent(gpx.Id);
                    var bytes = Encoding.UTF8.GetBytes(content);
                    return (bytes,"application/gpx+xml",gpx.Activity.GpxFileName);
                }
            }

            return (new byte[]{}, "text/plain", "no.txt");
        }
        

        private (byte[] bytes, string mimeType, string name) ZipActivities(Dictionary<ActivityHeader, string> extracted, Dictionary<string,string> errors)
        {
            if (extracted.Count + errors.Count > 1)
            {

                MemoryStream zipStream = new MemoryStream();

                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (var activity in extracted)
                    {
                        ZipArchiveEntry gpxEntry = archive.CreateEntry(activity.Key.GpxFileName);
                        using (StreamWriter writer = new StreamWriter(gpxEntry.Open()))
                        {
                            writer.Write(activity.Value);
                        }
                    }

                    foreach (var error in errors)
                    {
                        ZipArchiveEntry gpxEntry = archive.CreateEntry(error.Key+".error.log");
                        using (StreamWriter writer = new StreamWriter(gpxEntry.Open()))
                        {
                            writer.Write(error.Value);
                        }
                    }
                }

                if (zipStream.CanSeek)
                {
                    zipStream.Position = 0;
                }

                return (zipStream.ToArray(),"application/zip","gpx.zip");
            }
            else
            {
                if (extracted.Count == 1)
                {
                    var gpx = extracted.First();
                    var bytes = Encoding.UTF8.GetBytes(gpx.Value);
                    return (bytes,"application/gpx+xml",gpx.Key.GpxFileName);
                }
            }

            return (new byte[]{}, "text/plain", "no.txt");
        }

    }
}