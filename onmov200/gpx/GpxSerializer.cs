using System.Collections.Generic;
using System.IO;
using onmov200.model;


namespace onmov200.gpx
{
    public class GpxSerializer
    {

        public static string Serialize(List<WayPoint> wayPoints)
        {
            StringWriter writer = null;
            using (writer = new StringWriter())
            {
                writer.WriteLine(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<gpx creator=""olivier"" version=""1.0"">
<trk>
<trkseg>");
                foreach (var wayPoint in wayPoints)
                {
                    writer.WriteLine(WayPointXML(wayPoint));
                }
                writer.WriteLine(@"</trkseg>
</trk>
</gpx>");
            }

            return writer?.ToString();
        }
        
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
                        writer.WriteLine(WayPointXML(wayPoint));
                    }
                    writer.WriteLine(@"</trkseg>
</trk>
</gpx>");
                }
            }
        }


        public static string WayPointXML(WayPoint wayPoint)
        {
            
        return  $@"<trkpt lat=""{wayPoint.Latitude.ToString().Replace(",",".")}"" lon=""{wayPoint.Longitude.ToString().Replace(",",".")}"">
            <time>{wayPoint.Time.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")}</time>
            <ele>{wayPoint.Elevation}</ele>
            <extensions>                
                <gpxtpx:TrackPointExtension>
                        <gpxtpx:hr>{wayPoint.HR}</gpxtpx:hr>
                    </gpxtpx:TrackPointExtension>
            </extensions>
        </trkpt>";
        }
    }
}