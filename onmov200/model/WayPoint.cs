namespace onmov200.model
{
    public class WayPoint
    {
        public WayPoint(double latitude, double longitude, long elevation = 0)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }
        
        
        public int HR { get; set; }
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public long Elevation { get; set; }
        
        public string Xml => $@"<trkpt lat=""{Latitude.ToString().Replace(",",".")}"" lon=""{Longitude.ToString().Replace(",",".")}"">
            <extensions>
                <gpxtpx:TrackPointExtension>
                        <gpxtpx:hr>{HR}</gpxtpx:hr>
                    </gpxtpx:TrackPointExtension>
            </extensions>
        </trkpt>";
        
        public string ElevatedXml => $@"<wpt lat=""{Latitude.ToString().Replace(",",".")}"" lon=""{Longitude.ToString().Replace(",",".")}""><ele>{Elevation}</ele></wpt>";
    }
}

