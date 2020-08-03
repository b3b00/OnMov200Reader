using Newtonsoft.Json;
using onmov200.model;

namespace OnMov200WebApi
{
    public class ExtractionSummary
    {

        public ExtractionSummary()
        {
            
        }

        public ExtractionSummary(string name, string failure)
        {
            Ok = false;
            Name = name;
            Summary = failure;
        }

        public ExtractionSummary(ActivityHeader activity, string gpx)
        {
            Ok = true;
            Summary = activity.ToString();
            Name = activity.Name;
            Activity = activity;
            Gpx = gpx;
        }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        
        [JsonProperty("summary")]
        public string Summary { get; set; }
        
        [JsonProperty("gpx")]
        public string Gpx { get; set; }
        
        [JsonIgnore]
        public ActivityHeader Activity { get; set; }
    }
}