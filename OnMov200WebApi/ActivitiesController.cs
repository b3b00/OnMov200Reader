using System;
using System.Collections.Generic;
using System.Linq;
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
                                    var sum = new ExtractionSummary(res.activity, res.gpx);
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


       

        
        
        

        

    }
}