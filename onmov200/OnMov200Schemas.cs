using ByteReader;

namespace onmov200
{
    public class OnMov200Schemas
    {
        public static Schema USER = new Schema("user");
        
        public static Schema OMH = new Schema("OMH");

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
                .AddByte("activityId");
        }
        
            
        
       
    }
}