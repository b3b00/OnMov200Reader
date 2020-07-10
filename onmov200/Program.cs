using System;
using System.IO;
using ByteReader;

namespace onmov200
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var t = CommonReaders.ToFloat(new byte[] {0x64, 0x00, 0x00, 0x00});
            ;
            
            var user = new Schema("user");
            user = user.AddByte("gender")
            .AddByte("age")
            .AddByte("height")
            .AddByte("weight")
            .AddByte("hrRest")
            .AddByte("hrMax")
            .AddReserved("userReserved",14);
            
            var omh = new Schema("OMH");
            omh = omh.AddLong("distance")
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
            
            // Console.WriteLine("______________________________");
            // Console.WriteLine("___ USER                ___");
            // Console.WriteLine("______________________________");
            //
            // Stream stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\USER.BIN",FileMode.Open);
            // var result = user.Read(stream);
            // foreach (var r in result)
            // {
            //     Console.WriteLine($"{r.Key} : {r.Value}");
            // }
            // stream.Close();
            
            Console.WriteLine("______________________________");
            Console.WriteLine("___ OMH                    ___");
            Console.WriteLine("______________________________");
            
            var stream = File.Open(@"C:\Users\olduh\Desktop\perso\onmov200\DATA\ACT_0003.OMH",FileMode.Open);
            var result = omh.Read(stream);
            foreach (var r in result)
            {
                Console.WriteLine($"{r.Key} : {r.Value}");
            }
        }
    }
}