using System;
using System.Collections.Generic;

namespace onmov200.model
{
    public class ActivityHeader
    {
        
        
        public long Distance { get; set; }
        
        public int Duration { get; set; }
        
        public double AverageSpeed { get; set; }
        
        public double MaxSpeed { get; set; }
        
        public int Energy { get; set; }
        
        public int AverageHeartRate { get; set; }
        
        public int MaxHearRate { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public int ElevationPlus{ get; set; }
        
        public int ElevationMinus{ get; set; }

        public ActivityHeader(Dictionary<string, object> data)
        {
            Distance = (int) data["distance"];
            DateTime = new DateTime(2000 + (int) data["year"], (int) data["month"], (int) data["day"],
                (int) data["hour"], (int) data["min"], 0);
            Duration = (int) data["duration"];
            AverageSpeed = (double) data["avgSpeed"];
            MaxSpeed = (double) data["maxSpeed"];
            AverageHeartRate = (int) data["avgHR"];
            MaxHearRate = (int) data["maxHR"];
            ElevationPlus = (int) data["D+"];
            ElevationMinus = (int) data["D-"];
        }
        
        
    }
}