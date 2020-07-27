using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
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
            var validFiles = files.Where(f => f.FileName.EndsWith(OnMov200.HeaderExtension) || f.FileName.EndsWith(OnMov200.DataExtension));
            if (validFiles.Count() % 2 != 0)
            {
                return BadRequest("even file number expected");
            }
            
            var groupedActivitiesFiles = validFiles.GroupBy(f => f.NameWithoutExtension()).ToList();

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
                                    var memStream = new MemoryStream();

                                    using (var writer = new StringWriter())
                                    {
                                        await writer.WriteAsync(res.gpx);
                                    }

                                    //}
                                    if (memStream.CanSeek)
                                    {
                                        memStream.Position = 0;
                                    }

                                    var encoding = Encoding.UTF8;
                                    var bytes = encoding.GetBytes(res.gpx);

                                    return File(bytes, "application/gpx+xml", res.activity.GpxFileName);
                                }
                                else
                                {
                                    var error = result.IfLeft(() => new OMError(header, "no error"));
                                    return BadRequest(error.ErrorMessage);
                                }
                            }

                        }
                        else
                        {
                            return BadRequest("unable to find data file.");
                        }
                    }
                    else
                    {
                        return BadRequest($"{headerFile.FileName} : bad header file");
                    }
                    
                }
                else
                {
                    return BadRequest("unable to find header file");
                }
            }
            
            // build a FileStreamResult
            return Ok("coming soon");
            
        }

    }
}