using System;
using System.Collections.Generic;

namespace onmov200.model
{
    public class WayPoint
    {

        public WayPoint(Dictionary<string, object> data)
        {
            /// 1000000.0d;

            Latitude = (int) data["latitude"] / 1000000.0d;
            Longitude = (int) data["longitude"] / 1000000.0d;
        }
        
        // public WayPoint(double latitude, double longitude, long elevation = 0)
        // {
        //     Latitude = latitude;
        //     Longitude = longitude;
        //     Elevation = elevation;
        // }
        
        public DateTime Time { get; set; }
        
        public int HR { get; set; }
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public long Elevation { get; set; }
        
       
        
        public string ElevatedXml => $@"<wpt lat=""{Latitude.ToString().Replace(",",".")}"" lon=""{Longitude.ToString().Replace(",",".")}""><ele>{Elevation}</ele></wpt>";
    }
}

