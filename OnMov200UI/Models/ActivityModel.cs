using System.Diagnostics;
using onmov200.model;

namespace onmov200.Models
{
    public class ActivityModel
    {
        
        public ActivityHeader Activity { get; set; }
        
        public bool Checked { get; set; }

        public string Label => Activity.ToString();
        
        public ActivityModel(ActivityHeader activity)
        {
            Activity = activity;
        }
        
    }
}