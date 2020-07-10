using ByteReader;

namespace onmov200.parser
{
    public class OnMov200Schemas
    {
        public static Schema USER = new Schema("user");
        
        public static Schema OMH = new Schema("OMH");
        
        public static Schema OMD_GPS = new Schema("OMD_GPS");
        public static Schema OMD_CURVE = new Schema("OMD_CURVE");

        static OnMov200Schemas()
        {
            USER = USER.AddByte("gender")
                .AddByte("age")
                .AddByte("height")
                .AddByte("weight")
                .AddByte("hrRest")
                .AddByte("hrMax")
                .AddReserved("userReserved",14);

            OMH = OMH.AddLong("distance")
                    .AddInt("duration")
                    .AddFloat2("avgSpeed")
                    .AddFloat2("maxSpeed")
                    .AddInt("Cal")
                    .AddByte("avgHR")
                    .AddByte("maxHR")
                    .AddByte("year")
                    .AddByte("month")
                    .AddByte("day")
                    .AddByte("hour")
                    .AddByte("min")
                    .AddByte("activityId")

                    .AddInt("pointsCount")
                    .AddByte("indoor")
                    .AddByte("hrSensor")
                    .AddInt("D+")
                    .AddInt("D-")
                    .AddByte("sportType")
                    .AddReserved("r", 9)
                    .AddByte("activityId2")
                    .AddByte("dataId");

            OMD_GPS.AddLongGPS("latitude") //0
                .AddLongGPS("longitude") //4
                .AddLong("distance") //8 
                .AddInt("stopwatch") //12
                .AddByte("gpsStatus") // 14
                .AddReserved("r", 4) // 15
                .AddByte("dataId"); //19

            OMD_CURVE.AddInt("stopwatch")
                .AddFloat2("speed")
                .AddInt("kcal")
                .AddByte("hr")
                .AddByte("lap")
                .AddByte("cad")
                .AddByte("padByte")
                .AddInt("stopwatch2")
                .AddFloat2("speed2")
                .AddInt("kcal2")
                .AddByte("hr2")
                .AddByte("lap2")
                .AddByte("cad2")
                .AddByte("dataId");


        }
        
            
        
       
    }
}