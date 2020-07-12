using System.Diagnostics;
using onmov200.model;

namespace avaTodo.Models
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