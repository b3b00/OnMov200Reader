using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteReader;
using onmov200.model;

namespace onmov200.parser
{
    public class OMDParser
    {

        private const int GPS_DATA_ID = 0xF1;

        private const int CURVE_DATA_ID = 0xF2;

        private const int CHUNK_SIZE = 20;

        private Dictionary<int, Schema> Schemas = new Dictionary<int, Schema>()
        {
            {GPS_DATA_ID, OnMov200Schemas.OMD_GPS},
            {CURVE_DATA_ID, OnMov200Schemas.OMD_CURVE}
        };
        
        public OMDParser()
        {
            
        }

        
        
        public List<WayPoint> Parse(Stream stream, DateTime startTime)
        {
            long length = stream.Length;
            long frameCount = length / CHUNK_SIZE;

            List<WayPoint> points = new List<WayPoint>();
            
            for (int i = 0; i < frameCount; i++)
            {
                long dataIdPosition = i * CHUNK_SIZE + (CHUNK_SIZE-1);
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
                    var wp = new WayPoint(data);
                    points.Add(wp);
                }
                else
                {
                    int hr1 = (int) data["hr"];
                    int hr2 = (int) data["hr2"];
                    int sw1 = (int) data["stopwatch"];
                    int sw2 = (int) data["stopwatch2"];
                    
                    // todo set HR to last 2 points
                    points.Last().HR = hr2;
                    DateTime time2 = startTime.AddSeconds(sw2);
                    points.Last().Time = time2;
                    
                    DateTime time1 = startTime.AddSeconds(sw1);
                    points[^2].Time = time1;
                    points[^2].HR = hr1;

                }
            }

            return points;
        }
        
        
    }
}