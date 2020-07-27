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

        public ExtractionSummary(ActivityHeader activity, string id)
        {
            Ok = true;
            Summary = activity.ToString();
            Name = activity.Name;
            Id = id;
            Activity = activity;
        }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        
        [JsonProperty("summary")]
        public string Summary { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonIgnore]
        public ActivityHeader Activity { get; set; }
    }
}