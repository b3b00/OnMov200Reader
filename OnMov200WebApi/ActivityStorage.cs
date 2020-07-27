using System.Collections.Generic;
using onmov200.model;

namespace OnMov200WebApi
{
    public class ActivityStorage
    {
        private static int idcounter = 0;
        
        public static Dictionary<string,(ActivityHeader,string)> Data = new Dictionary<string, (ActivityHeader,string)>();

        public static string AddContent(ActivityHeader activity,string content)
        {
            idcounter++;
            Data[idcounter.ToString()] = (activity,content);
            return idcounter.ToString();
        }

        public static (ActivityHeader,string) getContent(string id)
        {
             (ActivityHeader activity, string content) c = (null,null);
            Data.TryGetValue(id, out c);
            return c;
        }
        
    }
}