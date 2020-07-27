using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        [HttpPost("/activities/upload")]
        public async Task<IActionResult> Upload()
        {
            var files = HttpContext.Request.Form.Files.ToList();
            var onmov = new OnMov200();
            var validFiles = files.Where(f =>
                f.FileName.EndsWith(OnMov200.HeaderExtension) || f.FileName.EndsWith(OnMov200.DataExtension));
            if (validFiles.Count() % 2 != 0)
            {
                return BadRequest("even file number expected");
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
                                    countOk++;
                                }
                                else
                                {
                                    countKo++;
                                    var error = result.IfLeft(() => new OMError(header, "no error"));
                                    if (groupedActivitiesFiles.Count == 1)
                                    {
                                        return BadRequest(error.ErrorMessage);
                                    }
                                    else
                                    {
                                        errors[header.Name] = error.ErrorMessage;
                                    }
                                }
                            }

                        }
                        else
                        {
                            countKo++;
                            if (groupedActivitiesFiles.Count == 1)
                            {
                                return BadRequest("unable to find data file.");
                            }
                            else
                            {
                                errors[header.Name] = "data file not found";
                                countKo++;
                            }
                        }
                    }
                    else
                    {
                        countKo++;
                        if (groupedActivitiesFiles.Count == 1)
                        {
                            return BadRequest($"{headerFile.FileName} : bad header file");
                        }
                        else
                        {
                            errors[header.Name] = $"{headerFile.FileName} : bad header file";
                            countKo++;
                        }
                    }

                }
                else
                {
                    countKo++;
                    if (groupedActivitiesFiles.Count == 1)
                    {
                        return BadRequest("unable to find header file");
                    }
                    else
                    {
                        errors[activityFiles.Key] = $"{headerFile.FileName} : unable to find header file";
                    }
                }
            }




            var returnValue = ZipActivities(extracted, errors);
            return File(returnValue.bytes, returnValue.mimeType, returnValue.name);
            

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