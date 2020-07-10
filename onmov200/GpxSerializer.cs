using System.Collections.Generic;
using System.IO;

namespace onmov200
{
    public class GpxSerializer
    {
        public static void Serialize(List<WayPoint> wayPoints, string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(@"<gpx creator=""olivier"" version=""1.0"">
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