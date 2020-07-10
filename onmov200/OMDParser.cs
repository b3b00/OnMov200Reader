using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteReader;

namespace onmov200
{
    public class OMDParser
    {

        private const int GPS_DATA_ID = 0xF1;

        private const int CURVE_DATA_ID = 0xF2;

        private Dictionary<int, Schema> Schemas = new Dictionary<int, Schema>()
        {
            {GPS_DATA_ID, OnMov200Schemas.OMD_GPS},
            {CURVE_DATA_ID, OnMov200Schemas.OMD_CURVE}
        };
        
        public OMDParser()
        {
            
        }

        
        
        public List<WayPoint> Parse(Stream stream)
        {
            long length = stream.Length;
            long frameCount = length / 20;

            List<WayPoint> points = new List<WayPoint>();
            List<Curve> curves = new List<Curve>();
            
            
            for (int i = 0; i < frameCount; i++)
            {
                long dataIdPosition = i * 20 + 19;
                long currentPosition = stream.Position;
                stream.Position = dataIdPosition;
                int dataId = stream.ReadByte();
                stream.Position = currentPosition;
                // get schema
                Schema schema = Schemas[dataId];
                // read schema
                var data = schema.Read(stream);
                if (dataId == GPS_DATA_ID)
                {
                    var wp = new WayPoint((double) data["latitude"], (double) data["longitude"]);
                    points.Add(wp);
                }
                else
                {
                    Curve curve = new Curve((int) data["hr"], (int) data["hr2"]);
                    curves.Add(curve);
                    
                    // todo set HR to last 2 points
                    points.Last().HR = curve.HR2;
                    points[^2].HR = curve.HR1;

                }
            }

            return points;
        }
        
        
    }
}