using System.Collections.Generic;
using System.IO;
using onmov200.model;


namespace onmov200.gpx
{
    public class GpxSerializer
    {
        public static void Serialize(List<WayPoint> wayPoints, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<gpx creator=""olivier"" version=""1.0"">
<trk>
<trkseg>");
                    foreach (var wayPoint in wayPoints)
                    {
                        writer.WriteLine(wayPoint.Xml);
                    }
                    writer.WriteLine(@"</trkseg>
</trk>
</gpx>");
                }
            }
        }
    }
}